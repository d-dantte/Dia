using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class ListPayloadSerializer :
        IPayloadSerializer<ListValue>,
        IPayloadProvider<ListValue>
    {
        public static ValuePayload<ListValue> CreatePayload(ListValue value)
        {
            throw new NotImplementedException();
        }

        public static IResult<ListValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }

        public static IResult<byte[]> Serialize(ListValue value, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
