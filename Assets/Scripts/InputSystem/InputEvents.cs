using System;
using CameraSystem;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputSystem {
    /// <summary>
    /// Helper class with a set of input related observables.
    /// </summary>
    public class InputEvents : IInputEvents {
        private const float SELECTION_THRESHOLD_VIEWPORT_SIZE = 0.01f;

        public IObservable<Unit> LeftMouseClickStream { get; }
        public IObservable<Unit> RightMouseClickStream { get; }

        private readonly CameraInput _cameraInput;

        public InputEvents(EventSystem eventSystem, CameraInput cameraInput) {
            _cameraInput = cameraInput;
            LeftMouseClickStream = GetClickStream(0, _ => !eventSystem.IsPointerOverGameObject());
            RightMouseClickStream = GetClickStream(1, _ => !eventSystem.IsPointerOverGameObject());
        }

        private IObservable<Unit> GetClickStream(int button, params Func<long, bool>[] whereStatements) {
            IObservable<long> mouseDownStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(button));
            foreach (var whereStatement in whereStatements) {
                mouseDownStream = mouseDownStream.Where(whereStatement);
            }

            IObservable<long> mouseUpStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(button));
            foreach (var whereStatement in whereStatements) {
                mouseUpStream = mouseUpStream.Where(whereStatement);
            }

            return mouseDownStream.Select(pos => mouseUpStream).Switch().AsUnitObservable();
        }

        public MouseDragEvent<Vector2> GetMouseDragEvent(params Func<Vector2, bool>[] where) {
            return GetMouseDragEvent(x => x, where);
        }

        public MouseDragEvent<TReturn> GetMouseDragEvent<TReturn>(
            Func<Vector2, TReturn> select, params Func<TReturn, bool>[] where) {
            IObservable<TReturn> mouseDownStream = Observable.EveryUpdate()
                                                             .Where(_ => Input.GetMouseButtonDown(0))
                                                             .Select(_ => _cameraInput.MouseWorldPosition)
                                                             .Select(select);
            foreach (var whereStatement in where) {
                mouseDownStream = mouseDownStream.Where(whereStatement);
            }

            IObservable<Vector2> mouseUpStream = Observable.EveryUpdate()
                                                           .Where(__ => Input.GetMouseButtonUp(0))
                                                           .Select(_ => _cameraInput.MouseWorldPosition);

            // Return a drag stream for each time we receive a mouse down.
            // This builds an observable of observables which will only emit as we drag.
            IObservable<Tuple<Vector2, TReturn>> mouseDragStream = mouseDownStream
                                                                   .Select(mouseDownValue =>
                                                                               GetMouseDragStream(mouseDownValue,
                                                                                                  mouseUpStream))
                                                                   // Switch "Flattens" the observable and only emits the drag events.
                                                                   .Switch();

            // Also return a stream that will emit values when the drag is ended.
            IObservable<Tuple<Vector2, TReturn>> dragEndStream = mouseDragStream
                                                                 .Select(pos => mouseUpStream).Switch()
                                                                 .Select(worldPos => Tuple.Create(worldPos,
                                                                                                  select.Invoke(worldPos)));

            return new MouseDragEvent<TReturn>(mouseDragStream, dragEndStream);
        }

        private IObservable<Tuple<Vector2, T>> GetMouseDragStream<T>(T mouseDownValue,
                                                                     IObservable<Vector2> mouseUpStream) {
            return Observable.EveryUpdate()
                             .Where(_ => Input.GetMouseButton(0))
                             .Select(_ => _cameraInput.MouseWorldPosition)
                             .TakeUntil(mouseUpStream)
                             .Pairwise()
                             .Where(mousePos =>
                                        Vector2.Distance(mousePos.Current, mousePos.Previous) >=
                                        SELECTION_THRESHOLD_VIEWPORT_SIZE)
                             .Select(x => Tuple.Create(x.Current, mouseDownValue));
        }
    }
}