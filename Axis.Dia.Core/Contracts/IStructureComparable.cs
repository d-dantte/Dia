namespace Axis.Dia.Core.Contracts
{
    /// <summary>
    /// Enables comparing the "structure" of a value-graph. Structure comparison ensures the graphs have the same shape,
    /// and each node has the same <see cref="DiaType"/>. The structure doesn't consider attribtues at the nodes.
    /// </summary>
    public interface IStructureComparable
    {
        /// <summary>
        /// Verify that the structures are the same. Structural equivalence is verified if:
        /// <list type="bullet">
        /// <item>
        ///     vector-types (<see cref="Types.Sequence"/>):
        ///     <list type="number">
        ///     <item>length is identical</item>
        ///     <item>At identical indexes, scalar-type values have identical <see cref="DiaType"/>.</item>
        ///     <item>At identical indexes, <see cref="IStructureComparable"/> values are equivalent.</item>
        ///     </list>
        /// </item>
        /// <item>
        ///     map-types (<see cref="Types.Record"/>):
        ///     <list type="number">
        ///     <item>property names are identical</item>
        ///     <item>At identical properties, scalar-types values have identical <see cref="DiaType"/>.</item>
        ///     <item>At identical properties, <see cref="IStructureComparable"/> values are equivalent.</item>
        ///     </list>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsStructurallyEquivalent(IStructureComparable other);
    }
}
