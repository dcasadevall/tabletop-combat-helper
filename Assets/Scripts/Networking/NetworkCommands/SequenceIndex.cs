namespace Networking.NetworkCommands {
    public class SequenceIndex {
        public uint index;

        public static SequenceIndex Of(uint index) {
            return new SequenceIndex(index);
        }

        private SequenceIndex(uint index) {
            this.index = index;
        }
    }
}