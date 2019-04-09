using System.Runtime.Serialization;

namespace CommandSystem {
    /// <summary>
    /// Implementation of <see cref="ICommandQueue"/> that instantly runs the given commands.
    /// </summary>
    public class InstantCommandQueue : ICommandQueue {
        public void Enqueue(ICommand command) {
            command.Run();
        }

        public void Enqueue<TData>(ICommand<TData> command, TData data) where TData : ISerializable {
            command.Run(data);
        }
    }
}