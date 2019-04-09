namespace CommandSystem {
    public interface ICommandQueue {
        void Enqueue(ICommand command);
        void Enqueue<T>(ICommand<T> command, T data);
    }
}