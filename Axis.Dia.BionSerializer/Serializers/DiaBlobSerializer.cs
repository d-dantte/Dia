using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Serializers
{
    internal class DiaBlobSerializer :
        ITypeSerializer<Blob>,
        IMetadataProvider<Blob>,
        IDefaultInstance<DiaBlobSerializer>
    {
        public static DiaBlobSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Blob value)
        {
            return TypeMetadata
                .Of(Core.DiaType.Blob)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(!value.Value?.IsEmpty ?? false); // true only if the blob contains items
        }

        public void SerializeType(Blob value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write attributes
            context.TypeSerializer.SerializeAttributeSet(value.Attributes, context);

            // Write data
            if (!value.IsEmpty)
            {
                // Write blob length/size
                value.Value!.Value.Length
                    .ToVarBytes()
                    .ApplyTo(context.Buffer.Write);

                // Write the blob data
                var bytes = value.Value!.Value.AsSpan();
                context.Buffer.Write(bytes);
            }
        }
    }
}
