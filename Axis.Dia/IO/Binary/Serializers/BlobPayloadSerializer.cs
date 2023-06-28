using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class BlobPayloadSerializer :
        IPayloadSerializer<BlobValue>,
        IPayloadProvider<BlobValue>
    {
        public static ValuePayload<BlobValue> CreatePayload(BlobValue value)
        {
            throw new NotImplementedException();
        }

        public static IResult<BlobValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }

        public static IResult<byte[]> Serialize(BlobValue value, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
