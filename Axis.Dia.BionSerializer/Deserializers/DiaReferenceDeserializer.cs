using Axis.Dia.Bion.Deserializers.Contracts;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Types;
using Axis.Dia.Bion.Utils;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Bion.Deserializers
{
    public class DiaReferenceDeserializer :
        ITypeDeserializer<Reference>,
        IDefaultInstance<DiaReferenceDeserializer>
    {
        public static DiaReferenceDeserializer DefaultInstance { get; } = new();

        public Reference DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);

            if (!Reference.ReferenceType.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{Reference.ReferenceType}, actual: {typeMetadata.Type}]");

            return stream
                .ReadChunks()
                .ApplyTo(chunks => chunks.ToRawBytes())
                .ApplyTo(bytes => new BigInteger(bytes, true))
                .ApplyTo(Reference.Of);
        }
    }
}
