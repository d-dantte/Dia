using Axis.Dia.BionSerializer.Metadata;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class DiaBooleanSerializer :
        ITypeSerializer<Core.Types.Boolean>,
        IMetadataProvider<Core.Types.Boolean>,
        IDefaultInstance<DiaBooleanSerializer>
    {
        public static DiaBooleanSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Boolean value)
        {
            return TypeMetadata
                .Of(Core.DiaType.Bool)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(value.Value ?? false);
        }

        public void SerializeType(Core.Types.Boolean value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write attributes
            context.TypeSerializer.SerializeAttributeSet(value.Attributes, context);
        }
    }
}
