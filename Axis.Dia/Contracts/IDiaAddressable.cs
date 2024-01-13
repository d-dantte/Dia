namespace Axis.Dia.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDiaAddressProvider: IDiaValue
    {
        /// <summary>
        /// The id/address of this instance
        /// </summary>
        Guid Address { get; }
    }

    /// <summary>
    /// Represents instances that have "addresses", and may be referred to by these addresses elsewhere
    /// within the Dia object graph. These "addresses" are implemented using <see cref="Guid"/> values to ensure uniqueness.
    /// <para>
    /// The companion type to addressable instances is the <see cref="Types.ReferenceValue"/>. It holds an address value that
    /// "points" to another <see cref="IDiaAddressable{TType}"/> instance within the object graph.
    /// </para>
    /// </summary>
    public interface IDiaAddressable<TType>: IDiaAddressProvider
    where TType : IDiaAddressable<TType>//, IDiaValue
    {
        /// <summary>
        /// Copies the content of this instance to a new one that has the given address.
        /// </summary>
        /// <param name="newAddress">The new address</param>
        /// <returns>The new instance of <typeparamref name="TType"/> having the new address</returns>
        TType RelocateValue(Guid newAddress);
    }
}
