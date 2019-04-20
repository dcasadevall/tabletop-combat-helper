namespace Units.Spawning {
    public interface IMutableUnitRegistry : IUnitRegistry {
        void RegisterUnit(IUnit unit);
        void UnregisterUnit(UnitId unitId);
    }
}