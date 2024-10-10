using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Serializers.Contracts;
using Axis.Dia.BionSerializer.Types;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class DiaSequenceSerializer :
        ITypeSerializer<Core.Types.Sequence>,
        IMetadataProvider<Core.Types.Sequence>,
        IDefaultInstance<DiaSequenceSerializer>
    {
        public static DiaSequenceSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Sequence value)
        {
            return TypeMetadata
                .Of(DiaType.Sequence)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(!value.IsEmpty);
        }

        public void SerializeType(Core.Types.Sequence value, ISerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.ValueTracker.TryAdd(value, _ => Reference.Of(context.Buffer.Count), out var @ref))
            {
                // Write the metadata
                this.ExtractMetadata(value)
                    .ApplyTo(meta => meta.Metadata.ToByteArray())
                    .Consume(array => context.Buffer.Write(array));

                // Write attributes
                context.AttributeSetSerializer.SerializeAttributeSet(value.Attributes, context);

                // Write data
                if (value.IsNull || value.IsEmpty)
                    return;

                // write item count
                value.Count
                    .ApplyTo(count => (BigInteger)count)
                    .ApplyTo(count => count.ToByteArray(true))
                    .ApplyTo(VarBytes.Of)
                    .Consume(data => context.Buffer.Write(data));

                // write the items
                value.ForEvery(item => context.TypeSerializer.SerializeType(item.Payload, context));
            }
            else
            {
                context.TypeSerializer.SerializeType(@ref, context);
            }
        }
    }
}
