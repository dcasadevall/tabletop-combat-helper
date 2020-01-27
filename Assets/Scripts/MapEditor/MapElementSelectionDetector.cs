using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using Grid.Positioning;
using InputSystem;
using MapEditor.MapElement;
using Math;
using UniRx;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using Zenject;

namespace MapEditor {
    public class MapElementSelectionDetector : IInitializable, IDisposable {
        private readonly IMapElementMenuViewController _mapElementMenuViewController;
        private readonly IInputEvents _inputEvents;
        private readonly IGridInputManager _gridInputManager;
        private readonly IGridPositionCalculator _gridPositionCalculator;
        private readonly IInputLock _inputLock;
        private readonly List<IMapEditorTool> _mapEditorTools;
        private readonly List<IDisposable> _observers = new List<IDisposable>();

        public MapElementSelectionDetector([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                                           IMapEditorTool sectionTileEditor,
                                           [Inject(Id = MapEditorInstaller.UNIT_TILE_EDITOR_ID)]
                                           IMapEditorTool unitTileEditor,
                                           IMapElementMenuViewController mapElementMenuViewController,
                                           IInputEvents inputEvents,
                                           IGridInputManager gridInputManager,
                                           IGridPositionCalculator gridPositionCalculator,
                                           IInputLock inputLock) {
            _mapElementMenuViewController = mapElementMenuViewController;
            _inputEvents = inputEvents;
            _gridInputManager = gridInputManager;
            _gridPositionCalculator = gridPositionCalculator;
            _inputLock = inputLock;

            // TODO: Try to find a way to inject this array even though the implementations are bound with ID.
            _mapEditorTools = new List<IMapEditorTool> {
                sectionTileEditor,
                unitTileEditor
            };
        }

        public void Initialize() {
            _gridInputManager.LeftMouseButtonOnTile
                             .Where(_ => !_inputLock.IsLocked)
                             .Subscribe(HandleTileClicked)
                             .AddTo(_observers);

            MouseDragEvent<IMapElement> dragEvent = _inputEvents.GetMouseDragEvent(worldPosition =>
                                                                                       SelectMapElementAtWorldPosition(worldPosition),
                                                                                   mapElement => mapElement != null);
            dragEvent.MouseDragStream.Subscribe(x => HandleMouseDrag(x.Item1, x.Item2)).AddTo(_observers);
            dragEvent.DragEndStream.Subscribe(x => HandleDragEnded(x.Item1, x.Item2)).AddTo(_observers);
        }

        public void Dispose() {
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

        private void HandleMouseDrag(Vector2 worldPosition, IMapElement mapElement) {
            
        }

        private void HandleDragEnded(Vector2 worldPosition, IMapElement mapElement) {
            
        }

        private IMapElement SelectMapElementAtWorldPosition(Vector2 worldPosition) {
            IntVector2? tileCoords = _gridPositionCalculator.GetTileContainingWorldPosition(worldPosition);
            if (tileCoords == null) {
                return null;
            }

            return SelectMapElement(tileCoords.Value);
        }

        private IMapElement SelectMapElement(IntVector2 tileCoords) {
            return _mapEditorTools.Select(editor => editor.MapElementAtTileCoords(tileCoords))
                                  .FirstOrDefault(x => x != null);
        }
    }
}