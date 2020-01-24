using System;
using InputSystem;
using Math;
using UniRx;
using UnityEngine;

namespace Grid.Highlighting {
    internal class GridCellHighlighter : IGridCellHighlighter {
        // For now, just have a default color. We can add it as an argument if needed.
        private static Color HIGHLIGHT_COLOR = new Color(1, 0, 0, 0.4f);

        private readonly IGridInputManager _gridInputManager;
        private readonly IGridCellHighlightPool _gridCellHighlightPool;

        private IGridCellHighlight _gridCellHighlight;

        public GridCellHighlighter(IGridInputManager gridInputManager,
                                   IGridCellHighlightPool gridCellHighlightPool) {
            _gridInputManager = gridInputManager;
            _gridCellHighlightPool = gridCellHighlightPool;
        }

        public IDisposable HighlightCellOnMouseOver(bool stayHighlighted = false) {
            var observer = _gridInputManager.MouseEnteredTile
                                            .Subscribe(HandleMouseOnTile);

            return new HighlightDisposable(observer, !stayHighlighted, this);
        }

        private void HandleMouseOnTile(IntVector2 tileCoords) {
            ClearHighlight();
            _gridCellHighlight = _gridCellHighlightPool.Spawn(tileCoords, HIGHLIGHT_COLOR);
        }

        public void ClearHighlight() {
            if (_gridCellHighlight != null) {
                _gridCellHighlightPool.Despawn(_gridCellHighlight);
                _gridCellHighlight = null;
            }
        }

        private class HighlightDisposable : IDisposable {
            private readonly IDisposable _observer;
            private readonly bool _clearOnDispose;
            private readonly IGridCellHighlighter _gridCellHighlighter;

            public HighlightDisposable(IDisposable observer, bool clearOnDispose,
                                       IGridCellHighlighter gridCellHighlighter) {
                _observer = observer;
                _clearOnDispose = clearOnDispose;
                _gridCellHighlighter = gridCellHighlighter;
            }

            public void Dispose() {
                _observer.Dispose();
                if (_clearOnDispose) {
                    _gridCellHighlighter.ClearHighlight();
                }
            }
        }
    }
}