using System.Collections.Immutable;

namespace Axis.Dia.Core
{
    public interface IValueContainer<TSelf, TValue>: IEnumerable<TValue>
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
        ImmutableArray<Types.Attribute> Attributes { get; }
    }
}
