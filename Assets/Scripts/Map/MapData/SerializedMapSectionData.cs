using System;
using System.Collections.Generic;
using System.Linq;
using Grid.Serialized;
using Map.MapData.TileMetadata;
using Map.Rendering;
using Math;
using UniRx;
using Units.Serialized;
using UnityEngine;

namespace Map.MapData {
    [Serializable]
    public class SerializedMapSectionData : IMutableMapSectionData {
        public GridData gridData;

        /// <summary>
        /// Section Index set by the containing <see cref="SerializedMapData"/>.
        /// This is not super robust but it does the job.
        /// </summary>
        [HideInInspector]
        public uint sectionIndex;
        public uint SectionIndex {
            get {
                return sectionIndex;
            }
        }

        public IGridData GridData {
            get {
                return gridData;
            }
        }

        public string sectionName;

        public string SectionName {
            get {
                return sectionName;
            }
        }

        public Sprite[] sprites;

        public Sprite[] Sprites {
            get {
                return sprites;
            }
        }

        public MapTileType generationType;

        public MapTileType MapTileType {
            get {
                return generationType;
            }
        }

        public int PixelsPerUnit {
            get {
                return 1;
            }
        }

        public bool hasPlayerUnitSpawnPoint;
        public Vector2 playerUnitSpawnPoint;
        public IntVector2? PlayerUnitSpawnPoint {
            get {
                if (!hasPlayerUnitSpawnPoint) {
                    return null;
                }
                
                return IntVector2.Of(playerUnitSpawnPoint);
            }
        }

        public List<TileMetadataPair> tileMetadataPairs = new List<TileMetadataPair>();
        public Dictionary<IntVector2, ITileMetadata> TileMetadataMap {
            get {
                return tileMetadataPairs.ToDictionary(x => IntVector2.Of(x.tileCoords),
                                                      x => (ITileMetadata) x.tileMetadata);
            }
        }

        #region IMutableMapSectionData
        private Subject<IntVector2?> _playerUnitSpawnPointSubject = new Subject<IntVector2?>();
        public IObservable<IntVector2?> PlayerUnitSpawnPointChanged => _playerUnitSpawnPointSubject;

        private Subject<Tuple<IntVector2, uint?>> _sectionConnectionChangeSubject =
            new Subject<Tuple<IntVector2, uint?>>();
        public IObservable<Tuple<IntVector2, uint?>> SectionConnectionChanged => _sectionConnectionChangeSubject;

        private Subject<Tuple<IntVector2, UnitDataReference>> _unitAddedSubject =
            new Subject<Tuple<IntVector2, UnitDataReference>>();
        public IObservable<Tuple<IntVector2, UnitDataReference>> UnitAdded => _unitAddedSubject;

        private Subject<Tuple<IntVector2, UnitDataReference>> _unitRemovedSubject =
            new Subject<Tuple<IntVector2, UnitDataReference>>();
        public IObservable<Tuple<IntVector2, UnitDataReference>> UnitRemoved => _unitRemovedSubject;
        
        public void AddInitialUnit(IntVector2 tileCoords, UnitDataReference unitDataReference) {
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            tileMetadata.units.Add(unitDataReference);
            _unitAddedSubject.OnNext(Tuple.Create(tileCoords, unitDataReference));
        }

        public void RemoveInitialUnit(IntVector2 tileCoords, UnitDataReference unitDataReference) {
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            tileMetadata.units.RemoveAll(x => x.UnitIndex == unitDataReference.UnitIndex);
            _unitRemovedSubject.OnNext(Tuple.Create(tileCoords, unitDataReference));
            ClearTileMetadataIfNecessary(tileCoords);
        }

        public void SetPlayerUnitSpawnPoint(IntVector2 spawnPoint) {
            IntVector2? oldSpawnPoint = PlayerUnitSpawnPoint;
            playerUnitSpawnPoint = new Vector2(spawnPoint.x, spawnPoint.y);
            hasPlayerUnitSpawnPoint = true;
            
            if (oldSpawnPoint == null || oldSpawnPoint.Value != spawnPoint) {
                _playerUnitSpawnPointSubject.OnNext(spawnPoint);
            }
        }

        public void ClearPlayerUnitSpawnPoint() {
            if (!hasPlayerUnitSpawnPoint) {
                return;
            }

            IntVector2 oldSpawnPoint = IntVector2.Of(playerUnitSpawnPoint);
            playerUnitSpawnPoint = default(Vector2);
            hasPlayerUnitSpawnPoint = false;
            _playerUnitSpawnPointSubject.OnNext(null);
            
            ClearTileMetadataIfNecessary(oldSpawnPoint);
        }

        public void SetSectionConnection(IntVector2 tileCoords, uint sectionIndex) {
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            var oldSectionIndex = tileMetadata.sectionConnection;
            
            tileMetadata.sectionConnection = (int) sectionIndex;
            if (oldSectionIndex != sectionIndex) {
                _sectionConnectionChangeSubject.OnNext(Tuple.Create(tileCoords, tileMetadata.SectionConnection));
            }
        }

        public void ClearSectionConnection(IntVector2 tileCoords) {
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            var oldSectionIndex = tileMetadata.sectionConnection;
            tileMetadata.sectionConnection = -1;
            
            if (oldSectionIndex != -1) {
                _sectionConnectionChangeSubject.OnNext(Tuple.Create(tileCoords, tileMetadata.SectionConnection));
            }
            
            ClearTileMetadataIfNecessary(tileCoords);
        }

        private TileMetadata.TileMetadata GetTileMetadata(IntVector2 tileCoords) {
            int index = tileMetadataPairs.FindIndex(x => IntVector2.Of(x.tileCoords) == tileCoords);

            if (index == -1) {
                tileMetadataPairs.Add(new TileMetadataPair(new Vector2(tileCoords.x, tileCoords.y)));
                index = tileMetadataPairs.Count - 1;
            }

            return tileMetadataPairs[index].tileMetadata;
        }
        
        private void ClearTileMetadataIfNecessary(IntVector2 tileCoords) {
            TileMetadataPair tileMetadataPair =
                tileMetadataPairs.FirstOrDefault(x => IntVector2.Of(x.tileCoords) == tileCoords);
            if (tileMetadataPair == null) {
                return;
            }

            if (!tileMetadataPair.tileMetadata.IsEmpty()) {
                return;
            }
            
            int index = tileMetadataPairs.FindIndex(x => IntVector2.Of(x.tileCoords) == tileCoords);
            tileMetadataPairs.RemoveAt(index);
        }
        #endregion
    }
}