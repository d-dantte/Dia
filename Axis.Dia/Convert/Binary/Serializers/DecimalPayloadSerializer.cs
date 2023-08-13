using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Convert.Binary.Serializers
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
                | (!value.IsNull && !BigDecimal.Zero.Equals(value.Value!) ? TypeMetadata.MetadataFlags.Overflow : TypeMetadata.MetadataFlags.None);

            var (significand, scale) = (value.Value ?? 0.0);
            var scaleSign = int.IsPositive(scale);
            var sigSign = BigInteger.IsPositive(significand);
            BigInteger sigByteCount =
                value.Value is null ? 0 :
                value.Value!.Value == 0 ? 0 :
                significand.GetByteCount(sigSign);

            // padding byte: for positive numbers that have the last bit set, an extra byte (0x0) is padded
            // at the end of the array so it isn't seen as a negative number. e.g,
            // 192 => [1100-0000] => [0000-0000, 1100-0000]
            // -64 => [1100-0000] => [1100-0000] // no padding is done here
            //
            // with the above in mind,
            // 1. if significand is positive, set the cmeta[0][D1] bit to 1, else leave it at zero
            // 2. if scale is positive, set the cmeta[0][D2] bit to 1, else leave it at zero
            // 2. concat `BitSequence.Of(significand.ToByteArray(true))`.SignificantBits to cmeta from [D3..]
            var cmetaBits =
                value.Value is null ? BitSequence.Empty :
                value.Value!.Value == 0 ? BitSequence.Empty :
                BitSequence
                    .Of(sigSign, scaleSign) // setting [D1], [D2]
                    .Concat(BitSequence.Of(sigByteCount.ToByteArray(true)).SignificantBits); // concatenating the byteCount

            var cmeta = VarBytes
                .Of(cmetaBits)
                .ToCustomMetadata();

            var typeMetadata = TypeMetadata.Of(
                DiaType.Decimal,
                metadataFlags,
                cmeta);

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
                    IsNullValue: tmeta.IsNull,
                    IsPositiveValue: tmeta.CustomMetadataCount > 0
                        ? tmeta.CustomMetadata[0].IsSet(CustomMetadata.MetadataFlags.D1)
                        : true,
                    IsPositiveScale: tmeta.CustomMetadataCount > 0
                        ? tmeta.CustomMetadata[0].IsSet(CustomMetadata.MetadataFlags.D2)
                        : true,
                    SignificandByteCount: tmeta.CustomMetadataCount > 0
                        ? (int)tmeta.CustomMetadata.ToBigInteger(2..)
                        : 0,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the decimal from the scale, and the sig
                .Map(tuple => (
                    tuple.Annotations,
                    Decimal: tuple.IsNullValue
                        ? (BigDecimal?)null
                        : new BigDecimal(
                            scale: tuple.SignificandByteCount == 0
                                ? 0
                                : stream
                                    .ReadVarBytesResult()
                                    .Map(vb => BitSequence.Of(vb.ToByteArray()))
                                    .Map(bs => bs.SignificantBits())
                                    .Map(bs => (int)new BigInteger(bs.ToByteArray(), true))
                                    .Map(scale => scale * (tuple.IsPositiveScale ? 1 : -1))
                                    .Resolve(),
                            significand: tuple.SignificandByteCount == 0
                                ? 0
                                : stream
                                    .ReadExactBytesResult(tuple.SignificandByteCount)
                                    .Map(bytes => new BigInteger(bytes, true))
                                    .Map(sig => sig * (tuple.IsPositiveValue ? 1 : -1))
                                    .Resolve())))

                // construct the DecimalValue
                .Map(tuple => DecimalValue.Of(
                    tuple.Decimal,
                    tuple.Annotations))

                // if the value could not be deserialized, creates an instance of ValueDeserializationException
                .MapError(PayloadSerializer.TranslateValueError<DecimalValue>);
        }

        public static IResult<byte[]> Serialize(DecimalValue value, BinarySerializerContext context)
        {
            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var decimalDataResult = (value.Value ?? 0) == 0
                    ? Result.Of(Array.Empty<byte>) // this is the only instance where significand is zero
                    : Result.Of(() =>
                    {
                        var (sig, scale) = value.Value!.Value;

                        var sigBytes = BigInteger
                            .Abs(sig)
                            .ToByteArray(true);

                        var scaleBytes = scale == 0
                            ? new byte[1]
                            : Math
                                .Abs(scale)
                                .ApplyTo(v => new BigInteger(v))
                                .ApplyTo(BitSequence.Of).SignificantBits
                                .ApplyTo(VarBytes.Of)
                                .ToArray();

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
