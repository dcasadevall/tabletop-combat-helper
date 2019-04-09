using System.Runtime.Serialization;

namespace CommandSystem {
    public interface ICommand<in TData> where TData : ISerializable {
        void Run(TData data);
    }

    public interface ICommand {
        void Run();
    }
}