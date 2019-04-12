namespace CommandSystem {
    public interface ICommandQueueNotifier {
        void AddListener(ICommandQueueListener listener);
    }
}