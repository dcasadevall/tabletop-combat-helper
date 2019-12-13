namespace CommandSystem {
    public interface IPausableCommandQueue {
        void Pause();
        void Resume();
    }
}