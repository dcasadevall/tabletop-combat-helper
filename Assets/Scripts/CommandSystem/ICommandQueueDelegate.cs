using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommandQueueListener {
        void HandleCommandQueued(ISerializable data, IUndoable undoableCommand);
    }
}