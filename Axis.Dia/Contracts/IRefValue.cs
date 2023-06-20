namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Represents a <see cref="IDiaValue"/> that encapsulates a clr ref-type
    /// </summary>
    /// <typeparam name="TValue">the encapsulated value</typeparam>
    public interface IRefValue<TValue> : IDiaValue
    where TValue : class
    {
        /// <summary>
        /// The encapsulated value
        /// </summary>
        TValue? Value { get; }
    }
}
