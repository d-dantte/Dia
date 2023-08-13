using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Convert.Binary.Serializers
{
    using Property = KeyValuePair<SymbolValue, IDiaValue>;

    internal class RecordPayloadSerializer :
        IPayloadSerializer<RecordValue>,
        IPayloadProvider<RecordValue>
    {
        public static ValuePayload<RecordValue> CreatePayload(RecordValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | (value.Value?.Length > 0 ? TypeMetadata.MetadataFlags.Overflow : TypeMetadata.MetadataFlags.None);

            BigInteger? itemCount = value.Value?.Length switch
            {
                null or 0 => null,
                int v => new BigInteger(v)
            };

            var typeMetadata = TypeMetadata.Of(
                DiaType.Record,
                metadataFlags,
                itemCount);

            return new ValuePayload<RecordValue>(value, typeMetadata);
        }

        public static IResult<RecordValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Record.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Record}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many items to read for the record
                .Map(tmeta => (
                    tmeta.IsNull,
                    ItemCount: tmeta.CustomMetadataCount > 0
                        ? (int)tmeta.CustomMetadata.ToBigInteger()
                        : 0,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the record
                .Map(tuple => (
                    tuple.Annotations,
                    Items: tuple.IsNull ? null :
                        tuple.ItemCount == 0 ? Array.Empty<Property>() :
                        Enumerable
                            .Range(0, tuple.ItemCount)
                            .Select(i => DeserializeProperty(stream, context))
                            .Fold()
                            .Map(values => values.ToArray())
                            .Resolve()))

                // construct the REcord
                .Map(tuple => RecordValue.Of(tuple.Items, tuple.Annotations))

                // if the value could not be deserialized, creates an instance of ValueDeserializationException
                .MapError(PayloadSerializer.TranslateValueError<RecordValue>);
        }

        public static IResult<byte[]> Serialize(RecordValue value, BinarySerializerContext context)
        {
            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var recordDataResult = (value.Value?.Length ?? 0) == 0
                    ? Result.Of(Array.Empty<byte>)
                    : Result.Of(() => value.Value!
                        .Select(item => SerializeProperty(item, context))
                        .Fold()
                        .Map(bytes => bytes.SelectMany().ToArray())
                        .Resolve());

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the blob bytes
                    .Combine(recordDataResult, (bytes, blobBytes) => bytes.Concat(blobBytes))
                    .Map(bytes => bytes.ToArray());
            }
            catch (Exception ex)
            {
                return Result.Of<byte[]>(ex);
            }
        }

        private static IResult<byte[]> SerializeProperty(
            Property property,
            BinarySerializerContext context)
        {
            if (property.Key.IsNull || string.Empty.Equals(property.Key.Value))
                throw new ArgumentException($"Invalid property symbol: {property.Key}");

            return SymbolPayloadSerializer
                .Serialize(property.Key, context)
                .Combine(
                    PayloadSerializer.SerializeDiaValueResult(property.Value, context),
                    (nameBytes, valueBytes) => nameBytes.Concat(valueBytes))
                .Map(bytes => bytes.ToArray());
        }

        private static IResult<Property> DeserializeProperty(Stream stream, BinarySerializerContext context)
        {
            return PayloadSerializer
                .DeserializeTypeMetadataResult(stream)
                .Bind(symbolTypeMetadata => SymbolPayloadSerializer
                    .Deserialize(stream, symbolTypeMetadata, context))
                .Combine(
                    PayloadSerializer.DeserializeDiaValueResult(stream, context),
                    (name, value) => KeyValuePair.Create(name, value));
        }
    }
}
