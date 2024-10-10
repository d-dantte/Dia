using Axis.Dia.Bion.Deserializers.Contracts;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Utils;
using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Bion.Deserializers
{
    using DiaSequence = Core.Types.Sequence;

    public class DiaSequenceDeserializer :
        IValueContainerDeserializer<DiaSequence, DiaValue>,
        IDefaultInstance<DiaSequenceDeserializer>
    {
        public static DiaSequenceDeserializer DefaultInstance { get; } = new();

        public DiaSequence DeserializeInstance(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Sequence.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Sequence}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return DiaSequence.Null(attributes!);

            else if (!typeMetadata.IsCustomFlagSet)
                return DiaSequence.Of([], attributes);

            else return DiaSequence.Of(attributes!);
        }

        public DiaSequence DeserializeItems(Stream stream, DiaSequence instance, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (instance.IsDefault)
                throw new ArgumentException($"Invalid instance: default");

            // read item count
            var count = stream
                .ReadVarBytes()
                .ApplyTo(vbytes => (int)vbytes.ToBigInteger(true));

            // read all items
            Enumerable
                .Range(0, count)
                .Select(_ => context.ValueDeserializer.DeserializeValue(stream, context))
                .Select(DiaValue.Of)
                .Consume(items => instance.AddAll(items.ToArray()));

            return instance;
        }
    }
}
