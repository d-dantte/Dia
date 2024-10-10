
using Axis.Dia.BionSerializer.Deserializers.Contracts;
using Axis.Dia.BionSerializer.Types;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public class DeserializerContext: IDeserializerContext
    {
        public IValueTracker ValueTracker { get; }

        public IValueDeserializer ValueDeserializer { get; }

        public IAttributeSetDeserializer AttributeSetDeserializer { get; }

        public DeserializerContext()
        {
            var deserializer = new CompoundDeserializer();

            ValueTracker = new ValueTracker();
            ValueDeserializer = deserializer;
            AttributeSetDeserializer = deserializer;
        }

        public DeserializerContext(
            IValueTracker valueTracker,
            IValueDeserializer valueDeserializer,
            IAttributeSetDeserializer attributeSetDeserializer)
        {
            ArgumentNullException.ThrowIfNull(valueTracker);
            ArgumentNullException.ThrowIfNull(valueDeserializer);
            ArgumentNullException.ThrowIfNull(attributeSetDeserializer);

            ValueTracker = valueTracker;
            ValueDeserializer = valueDeserializer;
            AttributeSetDeserializer = attributeSetDeserializer;
        }

        #region Nested types
        public class CompoundDeserializer :
            IValueDeserializer,
            IAttributeSetDeserializer,
            ITypeMetadataDeserializer,
            ITypeDeserializer<IDiaType>,
            IDefaultInstance<CompoundDeserializer>
        {
            public static CompoundDeserializer DefaultInstance { get; } = new();

            public Metadata.TypeMetadata DeserializeTypeMetadata(Stream stream, IDeserializerContext context)
            {
                ArgumentNullException.ThrowIfNull(stream);

                return stream
                    .ReadVarBytes()
                    .ApplyTo(Metadata.TypeMetadata.Of);
            }

            public IDiaValue DeserializeValue(Stream stream, IDeserializerContext context)
            {
                var position = stream.Position;
                var tmeta = DeserializeTypeMetadata(stream, context);

                if (!IsValueMetadata(tmeta))
                    throw new InvalidOperationException($"Invalid value-metadata: {tmeta}");

                return tmeta.Type switch
                {
                    DiaType.Sequence => DiaSequenceDeserializer.DefaultInstance
                        .ApplyTo(Converter => (Converter, Instance: Converter.DeserializeInstance(stream, tmeta, context)))
                        .ApplyTo(tuple => (tuple.Converter, Instance: context.ValueTracker.Track(position, tuple.Instance)))
                        .ApplyTo(tuple => tuple.Converter.DeserializeItems(stream, tuple.Instance.As<Sequence>(), context)),

                    DiaType.Record => DiaRecordDeserializer.DefaultInstance
                        .ApplyTo(Converter => (Converter, Instance: Converter.DeserializeInstance(stream, tmeta, context)))
                        .ApplyTo(tuple => (tuple.Converter, Instance: context.ValueTracker.Track(position, tuple.Instance)))
                        .ApplyTo(tuple => tuple.Converter.DeserializeItems(stream, tuple.Instance.As<Record>(), context)),

                    _ => DeserializeType(stream, tmeta, context).As<IDiaValue>()
                };
            }

            public IDiaType DeserializeType(
                Stream stream,
                Metadata.TypeMetadata typeMetadata,
                IDeserializerContext context)
            {
                ArgumentNullException.ThrowIfNull(stream);
                ArgumentNullException.ThrowIfNull(context);

                var position = stream.Position;
                return typeMetadata.Type switch
                {
                    DiaType.Attribute => DiaAttributeDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Blob => DiaBlobDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Bool => DiaBooleanDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Decimal => DiaDecimalDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Duration => DiaDurationDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Int => DiaIntegerDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.String => DiaStringDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Symbol => DiaSymbolDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Timestamp => DiaTimestampDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),

                    DiaType.Sequence 
                    or DiaType.Record => throw new InvalidOperationException(
                        $"Invalid metadata-type: ({typeMetadata.Type}). '{DiaType.Record}' or '{DiaType.Sequence}' are not valid for this method."),

                    Reference.ReferenceType => DiaReferenceDeserializer.DefaultInstance
                        .DeserializeType(stream, typeMetadata, context)
                        .ApplyTo(@ref => context.ValueTracker.GetValue(@ref.Ref)),

                    _ => throw new InvalidOperationException($"Invalid type: {typeMetadata.Type}")
                };
            }

            public Core.Types.Attribute[] DeserializeAttributeSet(Stream stream, IDeserializerContext context)
            {
                ArgumentNullException.ThrowIfNull(stream);
                ArgumentNullException.ThrowIfNull(context);

                var count = stream
                    .ReadVarBytes()
                    .ApplyTo(vbytes => vbytes.ToBigInteger());

                var attDeserializer = DiaAttributeDeserializer.DefaultInstance;

                return count
                    .Repeat()
                    .Select(_ => DeserializeTypeMetadata(stream, context))
                    .Select(tmeta => attDeserializer.DeserializeType(stream, tmeta, context))
                    .ToArray();
            }


            private static bool IsValueMetadata(Metadata.TypeMetadata tmeta)
            {
                return tmeta.Type switch
                {
                    DiaType.Unknown
                    or DiaType.Attribute => false,
                    _ => true
                };
            }
        }
        #endregion
    }
}
