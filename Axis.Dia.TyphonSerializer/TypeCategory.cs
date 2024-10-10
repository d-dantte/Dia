namespace Axis.Dia.Typhon
{
    public enum TypeCategory
    {
        /// <summary>
        /// Types that do not fall into any of the known categories
        /// </summary>
        Unknown,

        /// <summary>
        /// POCOs that have properties that can be read and/or modified
        /// </summary>
        Record,

        /// <summary>
        /// POCOs that implement the <see cref="IDictionary{TKey, TValue}"/> type
        /// </summary>
        Map,

        /// <summary>
        /// Clr types that implement <see cref="IEnumerable{T}"/>, but are not arrays, or records, or maps
        /// </summary>
        Sequence,

        /// <summary>
        /// Clr types that directly extend the <see cref="Array"/> type.
        /// </summary>
        SingleDimensionArray,

        /// <summary>
        /// Clr types that extend the <see cref="System.Enum"/> type
        /// </summary>
        Enum,

        /// <summary>
        /// Clr types that is equivalent to a dia primitive/simple type.
        /// <para/>
        /// All Dia types, except for <c>Record</c> and <c>Sequence</c>, are simple types.
        /// <para/>
        /// See <see cref="Extensions.IsSimpleType(Type)"/>
        /// </summary>
        Simple,
    }
}
