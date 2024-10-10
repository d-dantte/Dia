using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Bion.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Text;

namespace Axis.Dia.Bion.Serializers
{
    public class DiaSymbolSerializer :
        ITypeSerializer<Core.Types.Symbol>,
        IMetadataProvider<Core.Types.Symbol>,
        IDefaultInstance<DiaSymbolSerializer>
    {
        public static DiaSymbolSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Symbol value)
        {
            return TypeMetadata
                .Of(DiaType.Symbol)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(!string.Empty.Equals(value.Value ?? string.Empty));
        }

        public void SerializeType(Core.Types.Symbol value, ISerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write attributes
            context.AttributeSetSerializer.SerializeAttributeSet(value.Attributes, context);

            // Write data
            if (value.IsNull || string.Empty.Equals(value.Value))
                return;

            Encoding.Unicode
                .GetBytes(value.Value!)
                .ApplyTo(ByteChunks.Of)
                .Consume(chunks => context.Buffer.Write(chunks));
        }
    }
}
