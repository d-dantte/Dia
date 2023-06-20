using Axis.Dia.Types;

namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Defines the contract for Values that contain other values: i.e, whose value is a collection (array) of values
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IValueContainer<TSelf, TValue> :
        IRefValue<TValue[]>
        where TSelf : IValueContainer<TSelf, TValue>
    {
        /// <summary>
        /// Creates a new, empty version of this container
        /// </summary>
        abstract static TSelf Empty(params Annotation[] attributes);
    }
}
