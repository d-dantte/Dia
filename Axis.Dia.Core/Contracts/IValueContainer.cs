namespace Axis.Dia.Core.Contracts
{
    /// <summary>
    /// A container for multiple <see cref="IDiaValue"/> instances
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IValueContainer<TSelf, TValue> :
        IDiaValue,
        IRefValue<IEnumerable<TValue>>,
        IEnumerable<TValue>,
        IValueEquatable<TSelf>
        where TSelf : IValueContainer<TSelf, TValue>
    {
        /// <summary>
        /// Creates a new, empty version of this container
        /// </summary>
        abstract static TSelf Empty(params Types.Attribute[] attributes);

        /// <summary>
        /// Creates a new, empty version of this container, with no attributes
        /// </summary>
        /// <returns></returns>
        abstract static TSelf Empty();

        /// <summary>
        /// Indicates if the container contains values or not
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// A count of the values contained.
        /// </summary>
        int Count { get; }
    }
}
