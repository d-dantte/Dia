namespace Axis.Dia.Core
{
    /// <summary>
    /// A container for a clr value-type value
    /// </summary>
    /// <typeparam name="TValue">the encapsulated value</typeparam>
    public interface IStructValue<TValue> :
        IDiaValue
        where TValue : struct
    {
        /// <summary>
        /// The encapsulated value
        /// </summary>
        TValue? Value { get; }

        /// <summary>
        /// The attrbutes
        /// </summary>
        Types.AttributeSet Attributes { get; }
    }

    /// <summary>
    /// A container for a clr value-type value
    /// </summary>
    /// <typeparam name="TValue">the encapsulated value</typeparam>
    public interface IRefValue<TValue> :
        IDiaValue
        where TValue : class
    {
        /// <summary>
        /// The encapsulated value
        /// </summary>
        TValue? Value { get; }

        /// <summary>
        /// The attrbutes
        /// </summary>
        Types.AttributeSet Attributes { get; }
    }

    /// <summary>
    /// A container for a multiple <see cref="IDiaValue"/> instances
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IValueContainer<TSelf, TValue> :
        IDiaValue,
        IEnumerable<TValue>,
        IValueEquatable<TSelf>
        where TSelf : IValueContainer<TSelf, TValue>
    {
        /// <summary>
        /// Creates a new, empty version of this container
        /// </summary>
        abstract static TSelf Empty(params Types.Attribute[] attributes);

        /// <summary>
        /// Indicates if the container contains values or not
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// A count of the values contained.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The attrbutes
        /// </summary>
        Types.AttributeSet Attributes { get; }
    }
}
