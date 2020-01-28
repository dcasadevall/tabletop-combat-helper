using System;
using UniRx;
using UnityEngine;

namespace InputSystem {
    public interface IInputEvents {
        /// <summary>
        /// Stream which received values when the left mouse button is pressed (down and up).
        /// </summary>
        IObservable<Unit> LeftMouseClickStream { get; }

        /// <summary>
        /// Stream which received values when the right mouse button is pressed (down and up).
        /// </summary>
        IObservable<Unit> RightMouseClickStream { get; }
    }
}