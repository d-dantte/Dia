using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class SymbolPayloadSerializer :
        IPayloadSerializer<SymbolValue>,
        IPayloadProvider<SymbolValue>
    {
        // prohibits instantiation
        private SymbolPayloadSerializer() { }

        public static ValuePayload<SymbolValue> CreatePayload(SymbolValue value)
        {
            throw new NotImplementedException();
        }

        public static IResult<SymbolValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }

        public static IResult<byte[]> Serialize(SymbolValue value, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
