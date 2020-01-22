using System;

namespace Map.MapData.TileMetadata {
    [Serializable]
    public class TileMetadata : ITileMetadata {
        /// <summary>
        /// If > 0, this tile has a section connection.
        /// </summary>
        public int sectionConnection = -1;

        /// <summary>
        /// <inheritdoc cref="SectionConnection"/>
        /// </summary>
        public uint? SectionConnection {
            get {
                if (sectionConnection < 0) {
                    return null;
                }

                return (uint) sectionConnection;
            }
        }
    }
}