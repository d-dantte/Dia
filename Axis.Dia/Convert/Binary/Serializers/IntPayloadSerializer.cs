using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Convert.Binary.Serializers
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

            var sigSign = value.Value?.ApplyTo(BigInteger.IsPositive);
            BigInteger byteCount =
                value.Value is null ? 0 :
                value.Value!.Value == 0 ? 0 :
                value.Value!.Value.GetByteCount(sigSign!.Value);

            // padding byte: for positive numbers that have the last bit set, an extra byte (0x0) is padded
            // at the end of the array so it isn't seen as a negative number. e.g,
            // 192 => [1100-0000] => [0000-0000, 1100-0000]
            // -64 => [1100-0000] => [1100-0000] // no padding is done here
            //
            // with the above in mind,
            // 1. if value is positive, set the cmeta[0][D1] bit to 1, else leave it at zero
            // 2. concat `BitSequence.OfSignificantBits(byteCount.ToByteArray(true))` to cmeta from [D2..]
            var cmetaBits =
                value.Value is null ? BitSequence.Empty :
                value.Value!.Value == 0 ? BitSequence.Empty :
                BitSequence
                    .Of(sigSign!.Value) // setting [D1]
                    .Concat(BitSequence.Of(byteCount.ToByteArray(true)).SignificantBits); // concatenating the byteCount

            var cmeta = VarBytes
                .Of(cmetaBits)
                .ToCustomMetadata();

            var typeMetadata = TypeMetadata.Of(
                DiaType.Int,
                metadataFlags,
                cmeta);

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
                    IsNullValue: tmeta.IsNull,
                    IsPositiveValue: tmeta.CustomMetadataCount > 0
                        ? tmeta.CustomMetadata[0].IsSet(CustomMetadata.MetadataFlags.D1)
                        : true,
                    IntByteCount: tmeta.CustomMetadataCount > 0
                        ? (int)tmeta.CustomMetadata.ToBigInteger(1..)
                        : 0,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the int
                .Map(tuple => (
                    tuple.Annotations,
                    BigInt: tuple.IsNullValue
                        ? (BigInteger?)null
                        : stream
                            .ReadExactBytesResult(tuple.IntByteCount)
                            .Map(bytes => new BigInteger(bytes, tuple.IsPositiveValue))
                            .Resolve()))

                // construct the IntValue
                .Map(tuple => IntValue.Of(tuple.BigInt, tuple.Annotations))

                // if the value could not be deserialized, creates an instance of ValueDeserializationException
                .MapError(PayloadSerializer.TranslateValueError<IntValue>);
        }

        public static IResult<byte[]> Serialize(IntValue value, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var intDataResult = Result.Of(
                    value.Value is null ? Array.Empty<byte>() :
                    value.Value!.Value == 0 ? Array.Empty<byte>() :
                    value.Value!.Value.ToByteArray(BigInteger.IsPositive(value.Value!.Value)));

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
