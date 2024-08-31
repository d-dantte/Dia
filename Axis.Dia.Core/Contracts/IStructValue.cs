namespace Axis.Dia.Core.Contracts
{
    /// <summary>
    /// A container for a clr value-type value
    /// </summary>
    /// <typeparam name="TValue">the encapsulated value</typeparam>
    public interface IStructValue<TValue> :
        IDiaValue,
        IAttributeContainer
        where TValue : struct
    {
        /// <summary>
        /// The encapsulated value
        /// </summary>
        TValue? Value { get; }
    }
}
