using Axis.Dia.BionSerializer.Deserializers.Contracts;
using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public class ValueTracker: IValueTracker
    {
        private readonly Dictionary<BigInteger, IDiaValue> indexCache = new();

        public TDiaValue Track<TDiaValue>(BigInteger streamIndex, TDiaValue value)
        where TDiaValue : IDiaValue
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!indexCache.TryAdd(streamIndex, value))
                throw new InvalidOperationException(
                    $"Invalid streamIndex: duplicate detected");

            return value;
        }

        public bool TryGet<TDiaValue>(BigInteger streamIndex, out TDiaValue? value)
        where TDiaValue : IDiaValue
        {
            if (indexCache.TryGetValue(streamIndex, out var _value))
            {
                value = _value.Is(out TDiaValue __value)
                    ? __value
                    : throw new InvalidCastException(
                        $"Invalid dia-type [expected: '{typeof(TDiaValue)}', actual: '{_value!.GetType()}']");
                return true;
            }

            value = default;
            return false;
        }

        public TDiaValue GetValue<TDiaValue>(BigInteger streamIndex)
        where TDiaValue : IDiaValue
        {
            return TryGet<TDiaValue>(streamIndex, out var value)
                ? value!
                : throw new InvalidOperationException(
                    $"Invalid streamIndex: no value is being tracked for the given index");
        }

        public IDiaValue GetValue(BigInteger streamIndex)
        {
            return indexCache.TryGetValue(streamIndex, out var diaValue)
                ? diaValue
                : throw new InvalidOperationException(
                    $"Invalid streamIndex: no value is being tracked for the given index");
        }
    }
}
