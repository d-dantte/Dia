using Axis.Dia.Contracts;

namespace Axis.Dia.Types
{
    public readonly struct ValuePacket
    {
        private readonly List<IDiaValue> values;

        public IDiaValue[]? Values => values?.ToArray();

        public ValuePacket(IEnumerable<IDiaValue> values)
        {
            this.values = values?
                .ToList()
                ?? throw new ArgumentNullException(nameof(values));
        }

        public ValuePacket(params IDiaValue[] values)
        : this((IEnumerable<IDiaValue>)values)
        {
        }
    }
}
