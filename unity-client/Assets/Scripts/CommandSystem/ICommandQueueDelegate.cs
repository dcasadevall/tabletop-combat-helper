namespace CommandSystem {
    public interface ICommandQueueListener {
        void HandleCommandQueued(ICommandSnapshot commandSnapshot);
    }
}