using Axis.Dia.Contracts;

namespace Axis.Dia.IO.Binary.Serializers
{
    /// <summary>
    /// Contract for creating value payloads out of <see cref="IDiaValue"/> instances.
    /// </summary>
    /// <typeparam name="TDiaValue">The value type</typeparam>
    public interface IPayloadProvider<TDiaValue>
    where TDiaValue : IDiaValue
    {
        /// <summary>
        /// Creates a <see cref="ValuePayload{TDiaValue}"/> from a value
        /// </summary>
        /// <param name="value">The value from which the payload is created</param>
        /// <returns>The value payload</returns>
        abstract static ValuePayload<TDiaValue> CreatePayload(TDiaValue value);
    }
}
