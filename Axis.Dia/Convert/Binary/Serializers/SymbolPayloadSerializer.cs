using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;
using System.Text;

namespace Axis.Dia.Convert.Binary.Serializers
{
    internal class SymbolPayloadSerializer :
        IPayloadSerializer<SymbolValue>,
        IPayloadProvider<SymbolValue>
    {
        // prohibits instantiation
        private SymbolPayloadSerializer() { }

        public static ValuePayload<SymbolValue> CreatePayload(SymbolValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | (!string.IsNullOrEmpty(value.Value) ? TypeMetadata.MetadataFlags.Overflow : TypeMetadata.MetadataFlags.None);

            BigInteger? charCount = string.IsNullOrEmpty(value.Value) ? null : value.Value!.Length;

            var typeMetadata = TypeMetadata.Of(
                DiaType.Symbol,
                metadataFlags,
                charCount);

            return new ValuePayload<SymbolValue>(value, typeMetadata);
        }

        public static IResult<SymbolValue> Deserialize(
            Stream stream,
            TypeMetadata typeMetadata,
            BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Symbol.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Symbol}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many bytes/chars to read for the symbol
                .Map(tmeta => (
                    CharCount: !tmeta.IsNull
                        ? (int)tmeta.CustomMetadata.ToBigInteger()
                        : -1,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the symbol
                .Map(tuple => (
                    tuple.Annotations,
                    SymbolString: tuple.CharCount < 0
                        ? null
                        : stream
                            .ReadExactBytesResult(tuple.CharCount)
                            .Map(bytes => Encoding.ASCII.GetString(bytes))
                            .Resolve()))

                // construct the SymbolValue
                .Map(tuple => SymbolValue.Of(tuple.SymbolString, tuple.Annotations))

                // if the value could not be deserialized, creates an instance of ValueDeserializationException
                .MapError(PayloadSerializer.TranslateValueError<SymbolValue>);
        }

        public static IResult<byte[]> Serialize(SymbolValue value, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var symbolDataResult = Result.Of(value.Value?.ApplyTo(Encoding.ASCII.GetBytes) ?? Array.Empty<byte>());

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the symbol bytes
                    .Combine(symbolDataResult, (bytes, symbolBytes) => bytes.Concat(symbolBytes))
                    .Map(bytes => bytes.ToArray());
            }
            catch (Exception ex)
            {
                return Result.Of<byte[]>(ex);
            }
        }
    }
}
