namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Represents a <see cref="IDiaValue"/> that encapsulates a clr value-type
    /// </summary>
    /// <typeparam name="TValue">the encapsulated value</typeparam>
    public interface IStructValue<TValue> : IDiaValue
    where TValue : struct
    {
        /// <summary>
        /// The encapsulated value
        /// </summary>
        TValue? Value { get; }
    }
}
