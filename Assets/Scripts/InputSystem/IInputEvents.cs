using System;
using UniRx;
using UnityEngine;

namespace InputSystem {
    public interface IInputEvents {
        IObservable<Unit> LeftMouseClickStream { get; }
        IObservable<Unit> RightMouseClickStream { get; }
        /// <summary>
        /// Returns a <see cref="MouseDragEvent{T}"/> with values emitted when the mouse is being dragged, as well
        /// as when the mouse button is released.
        /// Applies the given (if any) where statements to filter out the events on click.
        /// This means that no drag values will be emitted at all if the mouse is not initially clicked matching the
        /// where predicates.
        /// The Vector2 position returned is in world coordinates.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        MouseDragEvent<Vector2> GetMouseDragEvent(params Func<Vector2, bool>[] where);

        /// <summary>
        /// Returns a <see cref="MouseDragEvent{T}"/> with values emitted when the mouse is being dragged, as well
        /// as when the mouse button is released.
        /// Applies the given (if any) where statements to filter out the events on click.
        /// This means that no drag values will be emitted at all if the mouse is not initially clicked matching the
        /// where predicates.
        /// Applies a select transform function to all values emitted either on mouse down, mouse up, or drag.
        /// The Vector2 position returned is in world coordinates.
        /// </summary>
        /// <param name="select"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        MouseDragEvent<TReturn> GetMouseDragEvent<TReturn>(Func<Vector2, TReturn> select,
                                                           params Func<TReturn, bool>[] where);
    }
}