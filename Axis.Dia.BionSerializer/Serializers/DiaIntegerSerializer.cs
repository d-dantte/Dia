using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Bion.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Bion.Serializers
{
    public class DiaIntegerSerializer :
        ITypeSerializer<Core.Types.Integer>,
        IMetadataProvider<Core.Types.Integer>,
        IDefaultInstance<DiaIntegerSerializer>
    {
        public static DiaIntegerSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Integer value)
        {
            return TypeMetadata
                .Of(DiaType.Int)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(!BigInteger.Zero.Equals(value.Value ?? 0));
        }

        public void SerializeType(Core.Types.Integer value, ISerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write attributes
            context.AttributeSetSerializer.SerializeAttributeSet(value.Attributes, context);

            // Write data
            if (value.IsNull || BigInteger.Zero.Equals(value.Value!))
                return;

            value.Value!.Value
                .ToByteArray()
                .ApplyTo(ByteChunks.Of)
                .Consume(chunks =>  context.Buffer.Write(chunks));
        }
    }
}
