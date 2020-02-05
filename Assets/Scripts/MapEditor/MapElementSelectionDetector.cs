using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using InputSystem;
using MapEditor.MapElement;
using Math;
using UniRx;
using UnityEngine;
using Zenject;

namespace MapEditor {
    public class MapElementSelectionDetector : IInitializable, IDisposable {
        private readonly IMapElementMenuViewController _mapElementMenuViewController;
        private readonly IGridInputManager _gridInputManager;
        private readonly IInputLock _inputLock;
        private readonly List<IMapEditorTool> _mapEditorTools;
        private readonly List<IDisposable> _observers = new List<IDisposable>();
        private IDisposable _mouseDragLock;
        private IDisposable _mouseUpObserver;

        public MapElementSelectionDetector([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                                           IMapEditorTool sectionTileEditor,
                                           [Inject(Id = MapEditorInstaller.UNIT_TILE_EDITOR_ID)]
                                           IMapEditorTool unitTileEditor,
                                           [Inject(Id = MapEditorInstaller.PLAYER_UNITS_TILE_EDITOR_ID)]
                                           IMapEditorTool playerUnitsTileEditor,
                                           IMapElementMenuViewController mapElementMenuViewController,
                                           IGridInputManager gridInputManager,
                                           IInputLock inputLock) {
            _mapElementMenuViewController = mapElementMenuViewController;
            _gridInputManager = gridInputManager;
            _inputLock = inputLock;

            // TODO: Try to find a way to inject this array even though the implementations are bound with ID.
            _mapEditorTools = new List<IMapEditorTool> {
                sectionTileEditor,
                unitTileEditor,
                playerUnitsTileEditor
            };
        }

        public void Initialize() {
            _gridInputManager.LeftMouseButtonOnTile
                             .Where(_ => !_inputLock.IsLocked)
                             .Subscribe(HandleTileClicked)
                             .AddTo(_observers);

            // We need to handle drag a bit more granularly.
            // 1 -> Subscribe to a mouse down of more than X time on a map element
            // 2 -> Lock all input, then subscribe to drag events
            // 3 -> On mouse up, unlock input and unsubscribe from drag events.
            _gridInputManager.LeftMouseDownOnTile
                             .Select(SelectMapElement)
                             .Where(x => x != null)
                             .Where(_ => !_inputLock.IsLocked)
                             .Select(element => Observable
                                                .Timer(TimeSpan.FromMilliseconds(200))
                                                .CombineLatest(Observable.Return(element), Tuple.Create))
                             .Switch()
                             .Subscribe(x => HandleMouseHeldOnElement(x.Item2))
                             .AddTo(_observers);
        }

        public void Dispose() {
            _mouseDragLock?.Dispose();
            _mouseDragLock = null;
            _observers.ForEach(x => x.Dispose());
            _observers.Clear();
        }

        private async void HandleTileClicked(IntVector2 tileCoords) {
            var mapElement = SelectMapElement(tileCoords);
            if (mapElement == null) {
                return;
            }

            using (_inputLock.Lock()) {
                await _mapElementMenuViewController.Show(tileCoords, mapElement);
            }
        }

        // TODO: Maybe have a "drag" helper which lets us subscribe to the whole mouse down -> drag -> mouse up
        // and encapsulate its lifecycle.
        #region MouseDrag
        private void HandleMouseHeldOnElement(IMapElement mapElement) {
            if (_inputLock.IsLocked) {
                return;
            }
            
            _mouseDragLock = _inputLock.Lock();
            _gridInputManager.LeftMouseDragOnTile.Subscribe(coords => HandleElementDragged(mapElement, coords));
            _mouseUpObserver = Observable.EveryUpdate()
                                         .Where(_ => Input.GetMouseButtonUp(0))
                                         .Subscribe(_ => HandleMouseUp());
        }

        private void HandleElementDragged(IMapElement mapElement, IntVector2 tileCoords) {
            mapElement.HandleDrag(tileCoords);
        }

        private void HandleMouseUp() {
            _mouseDragLock?.Dispose();
            _mouseDragLock = null;
            _mouseUpObserver?.Dispose();
            _mouseUpObserver = null;
        }
        #endregion

        private IMapElement SelectMapElement(IntVector2 tileCoords) {
            return _mapEditorTools.Select(editor => editor.MapElementAtTileCoords(tileCoords))
                                  .FirstOrDefault(x => x != null);
        }
    }
}