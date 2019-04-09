namespace Units.Serialized {
    /// <summary>
    /// Implementors of this interface convert unit data into an index that persists through sessions and allows us
    /// to serialize such unit data using other types of serialization, by storing a list of <see cref="IUnitData"/>s
    /// which can be indexed by this stable index.
    /// </summary>
    public interface IUnitDataIndexResolver {
        /// <summary>
        /// Returns the index associated with the given unit data, or null if such index cannot be resolved.
        /// </summary>
        /// <param name="unitType"></param>
        /// <param name="unitData"></param>
        /// <returns></returns>
        uint? ResolveUnitIndex(UnitType unitType, IUnitData unitData);
        /// <summary>
        /// Returns the unit data associated with the given index, or null if such index cannot be resolved to a unit.
        /// </summary>
        /// <param name="unitIndex"></param>
        /// <returns></returns>
        IUnitData ResolveUnitData(UnitType unitType, uint unitIndex);
    }
}