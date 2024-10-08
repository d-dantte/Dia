using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Luna.Extensions;
using System.Text;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class DiaAttributeSerializer :
        ITypeSerializer<Core.Types.Attribute>,
        IMetadataProvider<Core.Types.Attribute>,
        IDefaultInstance<DiaAttributeSerializer>
    {
        public static DiaAttributeSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Attribute value)
        {
            return TypeMetadata
                .Of(Core.DiaType.Attribute)
                .WithCustomBit(value.HasValue);
        }

        public void SerializeType(Core.Types.Attribute value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write the key
            value.Key
                .ApplyTo(Encoding.Unicode.GetBytes)
                .ApplyTo(ByteChunks.Of)
                .ApplyTo(chunks => chunks.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write the value
            value.Value?
                .ApplyTo(Encoding.Unicode.GetBytes)
                .ApplyTo(ByteChunks.Of)
                .ApplyTo(chunks => chunks.ToByteArray())
                .Consume(array => context.Buffer.Write(array));
        }
    }
}
