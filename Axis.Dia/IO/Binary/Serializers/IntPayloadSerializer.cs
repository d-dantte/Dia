using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using System.IO;
using System.Numerics;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class IntPayloadSerializer :
        IPayloadSerializer<IntValue>,
        IPayloadProvider<IntValue>
    {
        // prohibits instantiation
        private IntPayloadSerializer() { }

        public static ValuePayload<IntValue> CreatePayload(IntValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | (!value.IsNull && !BigInteger.Zero.Equals(value.Value!) ? TypeMetadata.MetadataFlags.Overflow : TypeMetadata.MetadataFlags.None);

            BigInteger byteCount =
                value.Value is null ? 0 :
                value.Value!.Value == 0 ? 0 :
                value.Value!.Value.GetByteCount();

            var byteCountVarBytes = byteCount
                .ToBitSequence()
                .ApplyTo(VarBytes.Of);

            var customMetadataArray = byteCountVarBytes
                .Select(@byte => (CustomMetadata)@byte)
                .ToArray();

            var typeMetadata = TypeMetadata.Of(
                DiaType.Int,
                metadataFlags,
                customMetadataArray);

            return new ValuePayload<IntValue>(value, typeMetadata);
        }

        public static IResult<IntValue> Deserialize(
            Stream stream,
            TypeMetadata typeMetadata,
            BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Int.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Int}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many bytes to read for the int
                .Map(tmeta => (
                    IntByteCount: !tmeta.IsNull
                        ? (int)tmeta.CustomMetadataAsInt()
                        : -1,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the int
                .Map(tuple => (
                    tuple.Annotations,
                    BigInt: tuple.IntByteCount < 0
                        ? (BigInteger?)null
                        : stream
                            .ReadExactBytesResult(tuple.IntByteCount)
                            .Map(bytes => new BigInteger(bytes))
                            .Resolve()))

                // construct the IntValue
                .Map(tuple => IntValue.Of(tuple.BigInt, tuple.Annotations));
        }

        public static IResult<byte[]> Serialize(IntValue value, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var intDataResult = Result.Of(value.Value?.ToByteArray() ?? Array.Empty<byte>());

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the int bytes
                    .Combine(intDataResult, (bytes, intBytes) => bytes.Concat(intBytes))
                    .Map(bytes => bytes.ToArray());
            }
            catch(Exception ex)
            {
                return Result.Of<byte[]>(ex);
            }
        }

        #region Custom Data Interpretation
        #endregion
    }
}
