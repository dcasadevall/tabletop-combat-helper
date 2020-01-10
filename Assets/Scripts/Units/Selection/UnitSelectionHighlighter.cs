using Units.Spawning;

namespace Units.Selection {
    public class UnitSelectionHighlighter {
        private readonly IUnitTransformRegistry _unitTransformRegistry;
        private readonly IUnitRegistry _unitRegistry;

        public UnitSelectionHighlighter(IUnitTransformRegistry unitTransformRegistry, IUnitRegistry unitRegistry) {
            _unitTransformRegistry = unitTransformRegistry;
            _unitRegistry = unitRegistry;
        }

        public void HighlightUnits(IUnit[] units) {
            foreach (var unit in _unitRegistry.GetAllUnits()) {
                // TODO: Maybe expose UnitRenderer only to the interested parties instead of using "GetComponent"
                UnitRenderer unitRenderer = _unitTransformRegistry
                                            .GetTransformableUnit(unit.UnitId).Transform.GetComponent<UnitRenderer>();
                unitRenderer.SetSelected(false); 
            }

            foreach (var unit in units) {
                // TODO: Maybe expose UnitRenderer only to the interested parties instead of using "GetComponent"
                UnitRenderer unitRenderer = _unitTransformRegistry
                                            .GetTransformableUnit(unit.UnitId).Transform.GetComponent<UnitRenderer>();
                unitRenderer.SetSelected(true);
            }
        }

        public void ClearHighlights() {
            foreach (var unit in _unitRegistry.GetAllUnits()) {
                // TODO: Maybe expose UnitRenderer only to the interested parties instead of using "GetComponent"
                UnitRenderer unitRenderer = _unitTransformRegistry
                                            .GetTransformableUnit(unit.UnitId).Transform.GetComponent<UnitRenderer>();
                unitRenderer.SetSelected(true); 
            } 
        }
    }
}