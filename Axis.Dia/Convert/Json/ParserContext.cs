using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Json
{
    public readonly struct ParserContext
    {
        private readonly Dictionary<int, Guid> addressIndexMap = new();

        public ParserContext()
        {
        }

        internal Guid Track(int addressIndex)
        {
            return addressIndexMap.GetOrAdd(addressIndex, index => Guid.NewGuid());
        }
    }
}
