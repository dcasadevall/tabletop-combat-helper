
namespace CommandSystem {
    public interface ICommand<TData> {
        void Run(TData data);
    }

    public interface ICommand {
        void Run();
    }
}