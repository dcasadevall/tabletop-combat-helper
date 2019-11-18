namespace Units.Spawning {
    /// <summary>
    /// This interface should only be exposed to actors which are able to modify the unit's transform.
    /// </summary>
    public interface IUnitTransformRegistry {
        ITransformableUnit GetTransformableUnit(UnitId unitId);
    }
}