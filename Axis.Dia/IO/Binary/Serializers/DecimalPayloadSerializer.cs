using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class DecimalPayloadSerializer :
        IPayloadSerializer<DecimalValue>,
        IPayloadProvider<DecimalValue>
    {
        // prohibits instantiation
        private DecimalPayloadSerializer() { }

        public static ValuePayload<DecimalValue> CreatePayload(DecimalValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | TypeMetadata.MetadataFlags.Overflow;

            var (significant, _) = (value.Value ?? 0.0);
            var  byteCount = (BigInteger)significant.GetByteCount();
            BitSequence byteCountSequence = byteCount.ToByteArray();
            VarBytes byteCountVarBytes = byteCountSequence[..(int)byteCount.GetBitLength()];

            var customMetadataArray = byteCountVarBytes
                .Select(@byte => (CustomMetadata)@byte)
                .ToArray();

            var typeMetadata = TypeMetadata.Of(
                DiaType.Int,
                metadataFlags,
                customMetadataArray);

            return new ValuePayload<DecimalValue>(value, typeMetadata);
        }

        public static IResult<DecimalValue> Deserialize(
            Stream stream,
            TypeMetadata typeMetadata,
            BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Decimal.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Decimal}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many bytes to read for the significand
                .Map(tmeta => (
                    SignificandByteCount: !tmeta.IsNull
                        ? (int)tmeta.CustomMetadataAsInt()
                        : -1,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the int
                .Map(tuple => (
                    tuple.Annotations,
                    BigInt: tuple.SignificandByteCount < 0
                        ? (BigInteger?)null
                        : stream
                            .ReadExactBytesResult(tuple.SignificandByteCount)
                            .Map(bytes => new BigInteger(bytes))
                            .Resolve()))

                // construct the DecimalValue
                .Map(tuple => DecimalValue.Of(tuple.BigInt, tuple.Annotations));
        }

        public static IResult<byte[]> Serialize(DecimalValue value, BinarySerializerContext context)
        {
            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var decimalDataResult = (value.Value ?? 0) == 0
                    ? Result.Of(Array.Empty<byte>)
                    : Result.Of(() =>
                    {
                        var (sig, scale) = value.Value!.Value;
                        var scaleBytes = BitConverter.GetBytes(scale);
                        var sigBytes = sig.ToByteArray();

                        return scaleBytes.JoinWith(sigBytes);
                    });

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the decimal bytes
                    .Combine(decimalDataResult, (bytes, decimalBytes) => bytes.Concat(decimalBytes))
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
