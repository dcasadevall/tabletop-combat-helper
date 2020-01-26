using System;
using UniRx;

namespace InputSystem {
    public interface IInputEvents {
        IObservable<Unit> LeftMouseClick { get; }
        IObservable<Unit> RightMouseClick { get; }
    }
}