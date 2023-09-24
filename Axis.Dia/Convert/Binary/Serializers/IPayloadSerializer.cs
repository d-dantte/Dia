using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Binary.Serializers
{
    internal interface IPayloadSerializer<TDiaValue>
    where TDiaValue: IDiaValue
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        abstract static IResult<byte[]> Serialize(TDiaValue value, SerializerContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        abstract static IResult<TDiaValue> Deserialize(
            Stream stream,
            TypeMetadata typeMetadata,
            DeserializerContext context);
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

        #region Deserialize
        internal static IResult<IDiaValue> DeserializeDiaValueResult(
            this Stream stream,
            DeserializerContext context)
        {
            return stream
                .DeserializeTypeMetadataResult()
                .Bind(tmeta =>
                {
                    if (DiaType.Ref.Equals(tmeta.Type))
                        return RefPayloadSerializer
                            .Deserialize(stream, tmeta, context)
                            .MapAs<IDiaValue>();

                    else
                    {
                        var address = context.AllocateAddress();
                        return tmeta.Type switch
                        {
                            DiaType.Bool => BoolPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.Instant => InstantPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.Int => IntPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => (IDiaValue)value),

                            DiaType.Decimal => DecimalPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.Blob => BlobPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.Clob => ClobPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.Symbol => SymbolPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.String => StringPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.List => ListPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            DiaType.Record => RecordPayloadSerializer
                                .Deserialize(stream, tmeta, context)
                                .Map(value => value.RelocateValue(address))
                                .MapAs<IDiaValue>(),

                            _ => throw new InvalidOperationException($"Invalid TypeMetadata found: {tmeta}")
                        };
                    }
                });
        }

        internal static bool TryDeserializeDiaValueResult(
            this Stream stream,
            DeserializerContext context,
            out IResult<IDiaValue> result)
            => (result = DeserializeDiaValueResult(stream, context)) is IResult<IDiaValue>.DataResult;

        #endregion

        #region Serialize

        internal static IResult<byte[]> SerializeDiaValueResult(
            this IDiaValue value,
            SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);

            try
            {
                if (value is ReferenceValue @ref)
                {
                    if (!context.IsTracked(@ref.ValueAddress))
                        return SerializeDiaValueResult(@ref.Value!, context);

                    return RefPayloadSerializer.Serialize(@ref, context);
                }

                else if (value is IDiaAddressProvider provider)
                {
                    if (context.IsTracked(provider.Address))
                        return SerializeDiaValueResult(ReferenceValue.Of(provider), context);

                    _ = context.Track(provider);

                    return provider!.Type switch
                    {
                        DiaType.Bool => BoolPayloadSerializer.Serialize((BoolValue)provider, context),

                        DiaType.Instant => InstantPayloadSerializer.Serialize((InstantValue)provider, context),

                        DiaType.Int => IntPayloadSerializer.Serialize((IntValue)provider, context),

                        DiaType.Decimal => DecimalPayloadSerializer.Serialize((DecimalValue)provider, context),

                        DiaType.Blob => BlobPayloadSerializer.Serialize((BlobValue)provider, context),

                        DiaType.Clob => ClobPayloadSerializer.Serialize((ClobValue)provider, context),

                        DiaType.Symbol => SymbolPayloadSerializer.Serialize((SymbolValue)provider, context),

                        DiaType.String => StringPayloadSerializer.Serialize((StringValue)provider, context),

                        DiaType.List => ListPayloadSerializer.Serialize((ListValue)provider, context),

                        DiaType.Record => RecordPayloadSerializer.Serialize((RecordValue)provider, context),

                        _ => Result.Of<byte[]>(new InvalidOperationException($"Invalid DiaType: {provider.Type}"))
                    };
                }
                else return Result.Of<byte[]>(new InvalidOperationException($"Invalid value instance: {value}"));
            }
            catch(Exception e)
            {
                return Result.Of<byte[]>(e);
            }
        }
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
