using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;

namespace Axis.Dia.Json.Deserializers
{
    internal class ReferenceMap
    {
        private readonly Dictionary<uint, IDiaValue> valueMap = [];

        public IDiaValue GetOrAdd(
            uint @ref,
            Func<uint, IDiaValue> valueProducer)
            => valueMap.GetOrAdd(@ref, valueProducer);

        public IDiaValue Dereference(uint @ref)
        {
            if (valueMap.TryGetValue(@ref, out IDiaValue? value))
                return value!;

            else throw new InvalidOperationException($"Invalid ref '0x{@ref:x}': value missing");
        }
    }
}
