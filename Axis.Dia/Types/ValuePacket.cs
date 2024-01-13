using Axis.Dia.Contracts;
using Axis.Luna.Extensions;

namespace Axis.Dia.Types
{
    public readonly struct ValuePacket
    {
        private readonly List<IDiaValue>? values;

        public IDiaValue[] Values => values?.ToArray() ?? Array.Empty<IDiaValue>();

        public ValuePacket(IEnumerable<IDiaValue> values)
        : this(values.ToArray())
        {
        }

        public ValuePacket(params IDiaValue[] values)
        {
            this.values = values?
                .ThrowIfAny(
                    value => value is null,
                    _ => new ArgumentException($"Invalid {nameof(values)}: contains null"))
                .ToList();
        }

        public static ValuePacket Of(params IDiaValue[] values) => new ValuePacket(values);

        public static ValuePacket Of(IEnumerable<IDiaValue> values) => new ValuePacket(values);
    }
}
