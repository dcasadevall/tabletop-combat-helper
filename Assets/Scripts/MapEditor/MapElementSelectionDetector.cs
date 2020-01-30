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
        private readonly IGridInputManager _gridInputManager;
        private readonly IInputLock _inputLock;
        private readonly List<IMapEditorTool> _mapEditorTools;
        private readonly List<IDisposable> _observers = new List<IDisposable>();

        public MapElementSelectionDetector([Inject(Id = MapEditorInstaller.SECTION_TILE_EDITOR_ID)]
                                           IMapEditorTool sectionTileEditor,
                                           [Inject(Id = MapEditorInstaller.UNIT_TILE_EDITOR_ID)]
                                           IMapEditorTool unitTileEditor,
                                           IMapElementMenuViewController mapElementMenuViewController,
                                           IGridInputManager gridInputManager,
                                           IInputLock inputLock) {
            _mapElementMenuViewController = mapElementMenuViewController;
            _gridInputManager = gridInputManager;
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

            _gridInputManager.LeftMouseDownOnTile
                             // We select only clicks that start on a map element.
                             .Select(SelectMapElement)
                             .Where(x => x != null)
                             .Where(_ => !_inputLock.IsLocked)
                             // Start capturing drag on mouse down.
                             // We "zip" an observable made with the single selected element with
                             // the observable of every emitted drag value.
                             // This results in a stream which emits values every time we drag on to a new tile,
                             // and containing the originally selected map element.
                             .Select(mapElement =>
                                         _gridInputManager.LeftMouseDragOnTile.CombineLatest(new List<IMapElement>
                                                                                                     {mapElement}
                                                                                                 .ToObservable(),
                                                                                             Tuple.Create))
                             // Switch "flattens" the observable of observables
                             .Switch()
                             // Call HandleDrag on the MapElement
                             .Subscribe(x => HandleElementDragged(x.Item2, x.Item1))
                             .AddTo(_observers);
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

        private void HandleElementDragged(IMapElement mapElement, IntVector2 tileCoords) {
            using (_inputLock.Lock()) {
                mapElement.HandleDrag(tileCoords);
            }
        }

        private IMapElement SelectMapElement(IntVector2 tileCoords) {
            return _mapEditorTools.Select(editor => editor.MapElementAtTileCoords(tileCoords))
                                  .FirstOrDefault(x => x != null);
        }
    }
}