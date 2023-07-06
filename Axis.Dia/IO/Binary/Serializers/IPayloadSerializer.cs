using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal interface IPayloadSerializer<TDiaValue>
    where TDiaValue: IDiaValue
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        abstract static IResult<byte[]> Serialize(TDiaValue value, BinarySerializerContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        abstract static IResult<TDiaValue> Deserialize(
            Stream stream,
            TypeMetadata typeMetadata,
            BinarySerializerContext context);
    }

    internal static class PayloadSerializer
    {
        #region Metadata
        internal static IResult<TypeMetadata> DeserializeTypeMetadataResult(
            this Stream stream)
            => stream.ReadVarBytesResult().Map(TypeMetadata.Of).MapError(TranslateTypeMetadataError);

        internal static bool TryDeserializeTypeMetadataResult(
            this Stream stream,
            out IResult<TypeMetadata> result)
            => (result = DeserializeTypeMetadataResult(stream)) is IResult<TypeMetadata>.DataResult;
        #endregion

        #region DiaValue
        internal static IResult<IDiaValue> DeserializeDiaValueResult(
            this Stream stream,
            BinarySerializerContext context)
        {
            return stream
                .DeserializeTypeMetadataResult()
                .Bind(tmeta => tmeta.Type switch
                {
                    DiaType.Bool => BoolPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.Instant => InstantPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.Int => IntPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.Decimal => DecimalPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.Blob => BlobPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.Clob => ClobPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.Symbol => SymbolPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.String => StringPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.List => ListPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    DiaType.Record => RecordPayloadSerializer
                        .Deserialize(stream, tmeta, context)
                        .Map(value => (IDiaValue)value),

                    _ => throw new InvalidOperationException($"Invalid TypeMetadata found: {tmeta}")
                });
        }

        internal static bool TryDeserializeDiaValueResult(
            this Stream stream,
            BinarySerializerContext context,
            out IResult<IDiaValue> result)
            => (result = DeserializeDiaValueResult(stream, context)) is IResult<IDiaValue>.DataResult;

        internal static IResult<byte[]> SerializeDiaValueResult(
            this IDiaValue value,
            BinarySerializerContext context)
        {
            return value.Type switch
            {
                DiaType.Bool => BoolPayloadSerializer.Serialize((BoolValue)value, context),

                DiaType.Instant => InstantPayloadSerializer.Serialize((InstantValue)value, context),

                DiaType.Int => IntPayloadSerializer.Serialize((IntValue)value, context),

                DiaType.Decimal => DecimalPayloadSerializer.Serialize((DecimalValue)value, context),

                DiaType.Blob => BlobPayloadSerializer.Serialize((BlobValue)value, context),

                DiaType.Clob => ClobPayloadSerializer.Serialize((ClobValue)value, context),

                DiaType.Symbol => SymbolPayloadSerializer.Serialize((SymbolValue)value, context),

                DiaType.String => StringPayloadSerializer.Serialize((StringValue)value, context),

                DiaType.List => ListPayloadSerializer.Serialize((ListValue)value, context),

                DiaType.Record => RecordPayloadSerializer.Serialize((RecordValue)value, context),

                _ => throw new InvalidOperationException($"Invalid DiaType found: {value.Type}")
            };
        }

        internal static bool TrySerializeDiaValueResult(
            this IDiaValue value,
            BinarySerializerContext context,
            out IResult<byte[]> result)
            => (result = SerializeDiaValueResult(value, context)) is IResult<byte[]>.DataResult;
        #endregion

        internal static TypeMetadata TranslateTypeMetadataError(ResultException exception)
        {
            return exception.InnerException switch
            {
                EndOfStreamException eos => eos.Throw<TypeMetadata>(),
                null => throw new Exception("Translation error: ResultException had no cause."),
                _ => throw new ValueDeserializationException(exception.InnerException!)
            };
        }

        internal static TDiaValue TranslateValueError<TDiaValue>(ResultException exception)
        {
            var cause = exception?.InnerException ?? throw new Exception("Translation error: ResultException had no cause.");
            throw new ValueDeserializationException(cause);
        }
    }

}
