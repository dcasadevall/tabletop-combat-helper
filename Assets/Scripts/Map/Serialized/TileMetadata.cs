using System;

namespace Map.Serialized {
    [Serializable]
    public class TileMetadata : ITileMetadata, IMutableTileMetadata {
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
            set {
                if (value == null) {
                    sectionConnection = -1;
                    return;
                }

                sectionConnection = (int) value.Value;
            }
        }
    }
}