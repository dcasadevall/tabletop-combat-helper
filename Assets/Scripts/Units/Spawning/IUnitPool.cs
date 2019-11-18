using Units.Serialized;

namespace Units.Spawning {
    public interface IUnitPool {
        IUnit Spawn(UnitId unitId, IUnitData unitData, IUnit[] pets);
        void Despawn(UnitId unitId);
    }
}