using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using System.Numerics;

namespace Axis.Dia.Convert.Binary
{

    public class SerializerContext
    {
        private readonly Dictionary<Guid, byte[]> refDataMap = new();

        internal bool TrySerializeRef(IDiaReference @ref, out byte[]? refData)
        {
            ArgumentNullException.ThrowIfNull(@ref);

            if (refDataMap.TryGetValue(@ref.ValueAddress, out refData))
                return true;

            else
            {
                refDataMap[@ref.ValueAddress] = refData = RefPayloadSerializer
                    .Serialize((ReferenceValue)@ref, this)
                    .Resolve();

                return false;
            }
        }

        internal BigInteger CurrentIndex => refDataMap.Count + 1;
    }

    public class DeserializerContext
    {
        private readonly Dictionary<BigInteger, Guid> addressMap = new();

        internal int CurrentIndex => addressMap.Count;

        internal Guid AllocateAddress()
        {
            return addressMap[addressMap.Count + 1] = Guid.NewGuid();
        }

        internal bool TryGetRefAddress(BigInteger refIndex, out Guid address)
        {
            return addressMap.TryGetValue(refIndex, out address);
        }
    }
}
