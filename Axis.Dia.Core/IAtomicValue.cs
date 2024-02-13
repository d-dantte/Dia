using System.Collections.Immutable;

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
        ImmutableArray<Types.Attribute> Attributes { get; }
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
        ImmutableArray<Types.Attribute> Attributes { get; }
    }
}
