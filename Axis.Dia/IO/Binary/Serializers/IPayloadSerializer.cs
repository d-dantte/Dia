using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Luna.Common.Results;

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
        // May never need this method
        internal static IResult<TDiaValue> Deserialize<TDiaValue>(
            byte[] bytes,
            BinarySerializerContext context,
            Func<Stream, BinarySerializerContext, IResult<TDiaValue>> deserializer)
            where TDiaValue : IDiaValue
        {
            ArgumentNullException.ThrowIfNull(bytes);
            ArgumentNullException.ThrowIfNull(deserializer);

            var memoryStream = new MemoryStream(bytes);
            var result = deserializer.Invoke(memoryStream, context);

            if (memoryStream.Position < memoryStream.Length)
                throw new PartialConsumptionException(
                    memoryStream.Position,
                    memoryStream.Length);

            else return result;
        }

        internal static bool TrySerialize<TDiaValue>(
            this TDiaValue value,
            BinarySerializerContext context,
            out IResult<byte[]> result)
            where TDiaValue : IDiaValue
        {
            throw new NotImplementedException();
        }

        internal static bool TryDeserialize<TDiaValue>(
            this Stream stream,
            BinarySerializerContext context,
            out IResult<TDiaValue> result)
            where TDiaValue : IDiaValue
        {
            throw new NotImplementedException();
        }

        internal static IResult<TypeMetadata> DeserializeTypeMetadataResult(
            this Stream stream)
            => stream.ReadVarBytesResult().Map(TypeMetadata.Of);

        internal static bool TryDeserializeTypeMetadataResult(
            this Stream stream,
            out IResult<TypeMetadata> result)
            => (result = DeserializeTypeMetadataResult(stream)) is IResult<TypeMetadata>.DataResult;


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

                    //DiaType.Symbol => IntPayloadSerializer
                    //    .Deserialize(stream, tmeta, context)
                    //    .Map(value => (IDiaValue)value),

                    //DiaType.Clob => IntPayloadSerializer
                    //    .Deserialize(stream, tmeta, context)
                    //    .Map(value => (IDiaValue)value),

                    //DiaType.Blob => IntPayloadSerializer
                    //    .Deserialize(stream, tmeta, context)
                    //    .Map(value => (IDiaValue)value),

                    //DiaType.List => IntPayloadSerializer
                    //    .Deserialize(stream, tmeta, context)
                    //    .Map(value => (IDiaValue)value),

                    //DiaType.Record => IntPayloadSerializer
                    //    .Deserialize(stream, tmeta, context)
                    //    .Map(value => (IDiaValue)value),

                    _ => throw new InvalidOperationException($"Invalid TypeMetadata found: {tmeta}")
                });
        }

        internal static bool TryDeserializeDiaValueResult(
            this Stream stream,
            BinarySerializerContext context,
            out IResult<IDiaValue> result)
            => (result = DeserializeDiaValueResult(stream, context)) is IResult<IDiaValue>.DataResult;
    }

}
