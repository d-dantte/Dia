using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Bion.Types;
using Axis.Dia.Bion.Utils;
using Axis.Luna.Extensions;

namespace Axis.Dia.Bion.Serializers
{
    public class DiaReferenceSerializer :
        ITypeSerializer<Reference>,
        IMetadataProvider<Reference>,
        IDefaultInstance<DiaReferenceSerializer>
    {
        public static DiaReferenceSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Reference value)
        {
            return TypeMetadata.Of(Reference.ReferenceType);
        }

        public void SerializeType(Reference value, ISerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write data
            value.Ref
                .ToByteArray(true)
                .ApplyTo(ByteChunks.Of)
                .Consume(chunks => context.Buffer.Write(chunks));
        }
    }
}
