using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class BlobPayloadSerializer :
        IPayloadSerializer<BlobValue>,
        IPayloadProvider<BlobValue>
    {
        // prohibits instantiation
        private BlobPayloadSerializer() { }

        public static ValuePayload<BlobValue> CreatePayload(BlobValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | (value.Value?.Length > 0 ? TypeMetadata.MetadataFlags.Overflow : TypeMetadata.MetadataFlags.None);

            BigInteger? byteCount = value.Value?.Length switch
            {
                null or 0 => null,
                int v => new BigInteger(v)
            };

            var typeMetadata = TypeMetadata.Of(
                DiaType.Blob,
                metadataFlags,
                byteCount);

            return new ValuePayload<BlobValue>(value, typeMetadata);
        }

        public static IResult<BlobValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Blob.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Blob}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many bytes/chars to read for the Blob
                .Map(tmeta => (
                    tmeta.IsNull,
                    ByteCount: tmeta.CustomMetadataCount > 0
                        ? (int)tmeta.CustomMetadata.ToBigInteger()
                        : 0,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the Blob
                .Map(tuple => (
                    tuple.Annotations,
                    Bytes: tuple.IsNull ? null:
                        tuple.ByteCount == 0 ? Array.Empty<byte>():
                        stream.ReadExactBytesResult(tuple.ByteCount).Resolve()))

                // construct the SymbolValue
                .Map(tuple => BlobValue.Of(tuple.Bytes, tuple.Annotations));
        }

        public static IResult<byte[]> Serialize(BlobValue value, BinarySerializerContext context)
        {
            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var blobDataResult = (value.Value?.Length ?? 0) == 0
                    ? Result.Of(Array.Empty<byte>)
                    : Result.Of(() => value.Value!);

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the blob bytes
                    .Combine(blobDataResult, (bytes, blobBytes) => bytes.Concat(blobBytes))
                    .Map(bytes => bytes.ToArray());
            }
            catch (Exception ex)
            {
                return Result.Of<byte[]>(ex);
            }
        }
    }
}
