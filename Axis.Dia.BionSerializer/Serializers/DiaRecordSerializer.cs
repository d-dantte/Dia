using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Serializers.Contracts;
using Axis.Dia.BionSerializer.Types;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class DiaRecordSerializer :
        ITypeSerializer<Core.Types.Record>,
        IMetadataProvider<Core.Types.Record>,
        IDefaultInstance<DiaRecordSerializer>
    {
        public static DiaRecordSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Record value)
        {
            return TypeMetadata
                .Of(DiaType.Record)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(!value.IsEmpty);
        }

        public void SerializeType(Core.Types.Record value, ISerializerContext context)
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

                // write property count
                value.Count
                    .ApplyTo(count => (BigInteger)count)
                    .ApplyTo(count => count.ToByteArray(true))
                    .ApplyTo(VarBytes.Of)
                    .Consume(data => context.Buffer.Write(data));

                // write the properties
                value.ForEvery(item => SerializeProperty(item, context));
            }
            else
            {
                context.TypeSerializer.SerializeType(@ref, context);
            }
        }

        public void SerializeProperty(Record.Property property, ISerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the property name as a DiaString
            var nameString = Core.Types.String.Of(property.Name.Name, [.. property.Name.Attributes]);
            context.TypeSerializer.SerializeType(nameString, context);

            // Write the value
            context.TypeSerializer.SerializeType(property.Value.Payload, context);
        }
    }
}
