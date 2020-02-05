using System.Threading;
using Math;
using UniRx.Async;

namespace Units.Spawning.UI {
    public interface IUnitSpawnViewController {
        UniTask Show(IntVector2 tileCoords, CancellationToken cancellationToken);
    }
}