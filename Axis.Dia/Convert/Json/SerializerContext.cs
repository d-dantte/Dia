using Axis.Dia.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.Convert.Json
{
    public class SerializerContext
    {
        private readonly Dictionary<Guid, int> addressIndexMap = new();

        public SerializerOptions Options { get; }


        public bool TryGetAddressIndex(IDiaAddressProvider addressProvider, out int index)
        {
            return addressIndexMap.TryGetValue(addressProvider.Address, out index);
        }
    }
}
