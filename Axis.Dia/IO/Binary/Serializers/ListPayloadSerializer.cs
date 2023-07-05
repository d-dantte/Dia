using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class ListPayloadSerializer :
        IPayloadSerializer<ListValue>,
        IPayloadProvider<ListValue>
    {
        public static ValuePayload<ListValue> CreatePayload(ListValue value)
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
                DiaType.List,
                metadataFlags,
                itemCount);

            return new ValuePayload<ListValue>(value, typeMetadata);
        }

        public static IResult<ListValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.List.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.List}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many items to read for the list
                .Map(tmeta => (
                    tmeta.IsNull,
                    ItemCount: tmeta.CustomMetadataCount > 0
                        ? (int)tmeta.CustomMetadata.ToBigInteger()
                        : 0,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the list
                .Map(tuple => (
                    tuple.Annotations,
                    Items: tuple.IsNull ? null :
                        tuple.ItemCount == 0 ? Array.Empty<IDiaValue>() :
                        Enumerable
                            .Range(0, tuple.ItemCount)
                            .Select(i => PayloadSerializer.DeserializeDiaValueResult(stream, context))
                            .Fold()
                            .Map(values => values.ToArray())
                            .Resolve()))

                // construct the List
                .Map(tuple => ListValue.Of(tuple.Items, tuple.Annotations));
        }

        public static IResult<byte[]> Serialize(ListValue value, BinarySerializerContext context)
        {
            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var listDataResult = (value.Value?.Length ?? 0) == 0
                    ? Result.Of(Array.Empty<byte>)
                    : Result.Of(() => value.Value!
                        .Select(item => PayloadSerializer.SerializeDiaValueResult(item, context))
                        .Fold()
                        .Map(bytes => bytes.SelectMany().ToArray())
                        .Resolve());

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the blob bytes
                    .Combine(listDataResult, (bytes, blobBytes) => bytes.Concat(blobBytes))
                    .Map(bytes => bytes.ToArray());
            }
            catch (Exception ex)
            {
                return Result.Of<byte[]>(ex);
            }
        }
    }
}
