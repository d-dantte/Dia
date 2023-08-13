namespace Axis.Dia.Convert.Type
{

    public enum TypeCategory
    {
        /// <summary>
        /// POCOs that have properties that can be read and/or modified
        /// </summary>
        Record,

        /// <summary>
        /// POCOs that, in addition to hosting regular properties, also implement the <see cref="IDictionary{TKey, TValue}"/> type
        /// </summary>
        ComplexMap,

        /// <summary>
        /// POCOs that, in addition to hosting regular properties, also implement the <see cref="IEnumerable{T}"/> type.
        /// </summary>
        ComplexCollection,

        /// <summary>
        /// POCOs that implement the <see cref="IDictionary{TKey, TValue}"/> type
        /// </summary>
        Map,

        /// <summary>
        /// Clr types that implement <see cref="IEnumerable{T}"/>, but are not arrays, or records, or maps
        /// </summary>
        Collection,

        /// <summary>
        /// Clr types that directly extend the <see cref="Array"/> type.
        /// </summary>
        SingleDimensionArray,

        /// <summary>
        /// Clr types that extend the <see cref="Enum"/> type
        /// </summary>
        Enum,

        /// <summary>
        /// Clr primitive types
        /// </summary>
        Primitive,

        /// <summary>
        /// Types that do not fall into any of the other categories
        /// </summary>
        InvalidType
    }
}
