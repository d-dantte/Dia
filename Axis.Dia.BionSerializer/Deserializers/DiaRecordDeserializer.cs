using Axis.Dia.BionSerializer.Deserializers.Contracts;
using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Deserializers
{
    using DiaRecord = Core.Types.Record;

    public class DiaRecordDeserializer :
        IValueContainerDeserializer<DiaRecord, DiaRecord.Property>,
        IDefaultInstance<DiaRecordDeserializer>
    {
        public static DiaRecordDeserializer DefaultInstance { get; } = new();

        public DiaRecord DeserializeInstance(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Record.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Record}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return DiaRecord.Null(attributes!);

            else if (!typeMetadata.IsCustomFlagSet)
                return DiaRecord.Of([], attributes);

            else return DiaRecord.Of(attributes);
        }

        public DiaRecord DeserializeItems(Stream stream, DiaRecord instance, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (instance.IsDefault)
                throw new ArgumentException($"Invalid instance: default");

            // read item count
            var count = stream
                .ReadVarBytes()
                .ApplyTo(vbytes => (int)vbytes.ToBigInteger());

            // read all items
            Enumerable
                .Range(0, count)
                .Select(_ => DeserializeProperty(stream, context))
                .Consume(items => instance.AddAll(items.ToArray()));

            return instance;
        }

        public DiaRecord.Property DeserializeProperty(Stream stream, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            // prop name (it is serialized as a String)
            var name = context.ValueDeserializer
                .DeserializeValue(stream, context)
                .ThrowIfNot(
                    v => v.Is(out Core.Types.String _),
                    v => new InvalidOperationException(
                        $"Invalid property-name type: {v.Type}"))
                .As<Core.Types.String>();

            // prop value
            var value = context.ValueDeserializer.DeserializeValue(stream, context);

            return DiaRecord.Property.Of(name, DiaValue.Of(value));
        }
    }
}
