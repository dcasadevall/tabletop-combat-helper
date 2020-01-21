using System;
using System.Collections.Generic;
using System.Linq;
using Grid.Serialized;
using Map.MapData.TileMetadata;
using Map.Rendering;
using Math;
using UniRx;
using UnityEngine;

namespace Map.MapData {
    [Serializable]
    public class MapSectionData : IMutableMapSectionData {
        public GridData gridData;

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