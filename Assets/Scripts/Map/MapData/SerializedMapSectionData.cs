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
        private Subject<Tuple<IntVector2, ITileMetadata>> _tileMetadataChangeSubject =
            new Subject<Tuple<IntVector2, ITileMetadata>>();
        public IObservable<Tuple<IntVector2, ITileMetadata>> TileMetadataChanged {
            get {
                return _tileMetadataChangeSubject;
            }
        }

        public void AddInitialUnit(IntVector2 tileCoords, IUnitData unit) {
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            tileMetadata.units.Add(UnitData.Clone(unit));
            _tileMetadataChangeSubject.OnNext(new Tuple<IntVector2, ITileMetadata>(tileCoords, tileMetadata));
        }

        public void RemoveInitialUnit(IntVector2 tileCoords, IUnitData unit) {
            // Note: we assume name is a valid unique identifier. This works for now.. but we may want to
            // generate a unique id for units.
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            tileMetadata.units.RemoveAll(x => x.name == unit.Name);
        }

        private Subject<IntVector2?> _playerUnitSpawnPointSubject = new Subject<IntVector2?>();
        public IObservable<IntVector2?> PlayerUnitSpawnPointChanged {
            get {
                return _playerUnitSpawnPointSubject;
            }
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

            playerUnitSpawnPoint = default(Vector2);
            hasPlayerUnitSpawnPoint = false;
            _playerUnitSpawnPointSubject.OnNext(null);
        }

        public void SetSectionConnection(IntVector2 tileCoords, uint sectionIndex) {
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            var oldSectionIndex = tileMetadata.sectionConnection;
            
            tileMetadata.sectionConnection = (int) sectionIndex;
            if (oldSectionIndex != sectionIndex) {
                _tileMetadataChangeSubject.OnNext(new Tuple<IntVector2, ITileMetadata>(tileCoords, tileMetadata));
            }
        }

        public void ClearSectionConnection(IntVector2 tileCoords) {
            TileMetadata.TileMetadata tileMetadata = GetTileMetadata(tileCoords);
            var oldSectionIndex = tileMetadata.sectionConnection;
            tileMetadata.sectionConnection = -1;
            
            // NOTE: When we add more metadata, we will only want to clear if there is no metadata left.
            // Do we maybe want to split the kinds of metadata?
            // Maybe we can have some sort of "default value" list, which we compare to in order to know if we 
            // should remove the metadata or not.
            ClearTileMetadata(tileCoords);

            if (oldSectionIndex != -1) {
                _tileMetadataChangeSubject.OnNext(new Tuple<IntVector2, ITileMetadata>(tileCoords, tileMetadata));
            }
        }

        private TileMetadata.TileMetadata GetTileMetadata(IntVector2 tileCoords) {
            int index = tileMetadataPairs.FindIndex(x => IntVector2.Of(x.tileCoords) == tileCoords);

            if (index == -1) {
                tileMetadataPairs.Add(new TileMetadataPair(new Vector2(tileCoords.x, tileCoords.y)));
                index = tileMetadataPairs.Count - 1;
            }

            return tileMetadataPairs[index].tileMetadata;
        }

        private void ClearTileMetadata(IntVector2 tileCoords) {
            int index = tileMetadataPairs.FindIndex(x => IntVector2.Of(x.tileCoords) == tileCoords);
            if (index == -1) {
                return;
            }

            tileMetadataPairs.RemoveAt(index);
        }
        #endregion
    }
}