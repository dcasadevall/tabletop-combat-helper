namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that instantly runs the given commands.
    /// </summary>
    public class InstantCommandQueue : ICommandQueue {
        public void Enqueue(ICommand command) {
            command.Run();
        }

        public void Enqueue<T>(ICommand<T> command, T data) {
            command.Run(data);
        }
    }
}