namespace Units {
    public interface IMutableUnitRegistry : IUnitRegistry {
        void RegisterUnit(IUnit unit);
        void UnregisterUnit(UnitId unitId);
    }
}