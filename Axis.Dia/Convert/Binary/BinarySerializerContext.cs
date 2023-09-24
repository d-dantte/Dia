using Axis.Dia.Contracts;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Convert.Binary
{

    public class SerializerContext
    {
        private readonly Dictionary<Guid, int> addressIndexMap = new();

        internal int Track(IDiaAddressProvider addressProvider)
        {
            ArgumentNullException.ThrowIfNull(addressProvider);

            return addressIndexMap.GetOrAdd(addressProvider.Address, _ => addressIndexMap.Count + 1);
        }

        internal int IndexOf(IDiaReference reference)
        {
            ArgumentNullException.ThrowIfNull(reference);

            if (!reference.IsLinked)
                throw new InvalidOperationException($"Reference is not linked: {reference}");

            return addressIndexMap.GetOrAdd(reference.ValueAddress, _ => addressIndexMap.Count + 1);
        }

        internal bool IsTracked(Guid address) => addressIndexMap.ContainsKey(address);
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
