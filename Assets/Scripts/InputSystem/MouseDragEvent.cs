using System;
using UnityEngine;

namespace InputSystem {
    /// <summary>
    /// A tuple containing observables which can be used to handle a "mouse drag" event.
    /// The value of the data being emitted will depend on how the observable was constructed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MouseDragEvent<T> {
        /// <summary>
        /// Stream emitting values produced as long as the left mouse button is held down and moved.
        /// Values emitted contain the world position of the click, as well as the transformed value of the
        /// selected element on mouse down.
        /// </summary>
        public readonly IObservable<Tuple<Vector2, T>> MouseDragStream;
        /// <summary>
        /// Stream emitting values produced every frame that the left mouse button is released.
        /// It can be subscribed to after a <see cref="MouseDragStream"/> is received in order to know when
        /// the drag has ended.
        /// Values emitted contain the world position of the drag end, as well as the transformed value of the
        /// selected element on mouse down.
        /// </summary>
        public readonly IObservable<Tuple<Vector2, T>> DragEndStream;

        public MouseDragEvent(IObservable<Tuple<Vector2, T>> mouseDragStream, IObservable<Tuple<Vector2, T>> dragEndStream) {
            MouseDragStream = mouseDragStream;
            DragEndStream = dragEndStream;
        }
    }

    public class MouseDragEvent {
        /// <summary>
        /// Stream emitting values produced as long as the left mouse button is held down and moved.
        /// Values emitted contain the world position of the click.
        /// </summary>
        public readonly IObservable<Vector2> MouseDragStream;
        /// <summary>
        /// Stream emitting values produced every frame that the left mouse button is released.
        /// It can be subscribed to after a <see cref="MouseDragStream"/> is received in order to know when
        /// the drag has ended.
        /// Values emitted contain the world position of the drag end.
        /// </summary>
        public readonly IObservable<Vector2> DragEndStream;
        
        public MouseDragEvent(IObservable<Vector2> mouseDragStream, IObservable<Vector2> dragEndStream) {
            MouseDragStream = mouseDragStream;
            DragEndStream = dragEndStream;
        }
    }
}