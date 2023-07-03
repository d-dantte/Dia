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
    internal class RecordPayloadSerializer :
        IPayloadSerializer<RecordValue>,
        IPayloadProvider<RecordValue>
    {
        public static ValuePayload<RecordValue> CreatePayload(RecordValue value)
        {
            throw new NotImplementedException();
        }

        public static IResult<RecordValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }

        public static IResult<byte[]> Serialize(RecordValue value, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
