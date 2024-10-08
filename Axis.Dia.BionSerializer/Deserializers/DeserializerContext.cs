
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public interface IDeserializerContext
    {
        IValueTracker ValueTracker { get; }

        IValueDeserializer ValueDeserializer { get; }

        IAttributeSetDeserializer AttributeSetDeserializer { get; }
    }

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
                var tmeta = DeserializeTypeMetadata(stream, context);

                if (!IsValueMetadata(tmeta))
                    throw new InvalidOperationException($"Invalid value-metadata: {tmeta}");

                return DeserializeType(stream, tmeta, context).As<IDiaValue>();
            }

            public IDiaType DeserializeType(
                Stream stream,
                Metadata.TypeMetadata typeMetadata,
                IDeserializerContext context)
            {
                ArgumentNullException.ThrowIfNull(stream);
                ArgumentNullException.ThrowIfNull(context);

                return typeMetadata.Type switch
                {
                    DiaType.Attribute => DiaAttributeDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Blob => DiaBlobDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Bool => DiaBooleanDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Decimal => DiaDecimalDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Duration => DiaDurationDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
                    DiaType.Int => DiaIntegerDeserializer.DefaultInstance.DeserializeType(stream, typeMetadata, context),
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
