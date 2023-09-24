using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using System.Globalization;
using System.Numerics;
using System.Text;
using static Axis.Dia.Convert.Json.SerializerOptions;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class EncodedValueParser :
        IRootSymbolProvider,
        IJsonConverter<IDiaValue>
    {
        internal const string SymbolNameEncodedValue = "encoded-value";
        internal const string SymbolNameValueMetadata = "value-metadata";
        internal const string SymbolNameValueContent = "value-content";
        internal const string SymbolNameTimestamp= "time-stamp";

        public const string SymbolNameNullInstant = "null-instant";
        public const string SymbolNameMilliSecond = "millisecond-precision";
        public const string SymbolNameSecond = "second-precision";
        public const string SymbolNameMinute = "minute-precision";
        public const string SymbolNameDay = "day-precision";
        public const string SymbolNameMonth = "month-precision";
        public const string SymbolNameYear = "year-precision";

        #region Instant Formats
        private static readonly string YearFormat = "yyyyT";
        private static readonly string MonthFormat = "yyyy-MMT";
        private static readonly string DayFormat = "yyyy-MM-dd";
        private static readonly string MinuteFormat = "yyyy-MM-ddTHH:mmzzz";
        private static readonly string SecondFormat = "yyyy-MM-ddTHH:mm:sszzz";
        private static readonly string MillisecondFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";
        #endregion

        public static string RootSymbol => SymbolNameEncodedValue;


        public static IResult<string> Serialize(IDiaValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            var type = Result.Of($"${value.Type};");

            var annotations = AnnotationParser.Serialize(value.Annotations, context);

            var refIndex = value is IDiaAddressProvider idp && context.TryGetAddressIndex(idp, out var index)
                ? Result.Of($"#0x{index:x};")
                : Result.Of("");

            return refIndex
                .Combine(type, (i, t) => (Index: i, Type: t))
                .Combine(annotations, (info, _annotations) => (info.Index, info.Type, Annotations: _annotations))
                .Map(info =>
                {
                    var content = value.IsNull
                        ? "null"
                        : value switch
                        {
                            BoolValue v => v.Value!.Value.ToString().ToLower(),
                            IntValue v => v.Value!.Value.ToString(),

                            DecimalValue v => context.Options.Decimals.UseExponentNotation
                                ? v.Value!.Value.ToScientificString(context.Options.Decimals.MaxPrecision)
                                : v.Value!.Value.ToNonScientificString(context.Options.Decimals.MaxPrecision),

                            InstantValue v => context.Options.Timestamps.TimestampPrecision switch
                            {
                                TimestampPrecision.Year => v.Value!.Value.ToString(YearFormat),
                                TimestampPrecision.Month => v.Value!.Value.ToString(MonthFormat),
                                TimestampPrecision.Day => v.Value!.Value.ToString(DayFormat),
                                TimestampPrecision.Minute => v.Value!.Value.ToString(MinuteFormat),
                                TimestampPrecision.Second => v.Value!.Value.ToString(SecondFormat),
                                TimestampPrecision.MilliSecond => v.Value!.Value.ToString(MillisecondFormat),
                                _ => throw new InvalidOperationException(
                                    $"Invalid timestamp precision: {context.Options.Timestamps.TimestampPrecision}")
                            },

                            BlobValue v => System.Convert.ToBase64String(v.Value!),
                            ClobValue v => System.Convert.ToBase64String(Encoding.Unicode.GetBytes(v.Value!)),

                            StringValue v => v.Value!,
                            SymbolValue v => v.Value!,

                            ReferenceValue v => context.TryGetAddressIndex(v.Value!, out index)
                                ? $"0x{index:x}"
                                : throw new InvalidOperationException($"The reference is not tracked: '{v}'"),

                            _ => throw new InvalidOperationException($"Invalid type: '{value.Type}'")
                        };

                    return $"\"[{info.Index}{info.Type}{info.Annotations}]{content}\"";
                });
        }

        public static IResult<IDiaValue> Parse(CSTNode encodedValueNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(encodedValueNode);
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            if (!SymbolNameEncodedValue.Equals(encodedValueNode.SymbolName))
                return Result.Of<IDiaValue>(new FormatException(
                    $"Invalid encoded metadata: '{encodedValueNode.TokenValue()}'",
                    new ArgumentException(
                        $"Invalid symbol '{encodedValueNode.SymbolName}', expected '{SymbolNameEncodedValue}'")));

            var valueMetadataNode = encodedValueNode
                .FindNodes(SymbolNameValueMetadata)
                .FirstOrDefault();

            if (valueMetadataNode is null)
                return Result.Of<IDiaValue>(new FormatException(
                    $"Invalid encoded metadata: '{encodedValueNode.TokenValue()}'",
                    new InvalidOperationException(
                        $"Missing symbol: '{SymbolNameValueMetadata}'")));

            var contentNode = encodedValueNode
                .FindNodes($"{SymbolNameValueContent}|{SymbolNameTimestamp}")
                .FirstOrDefault();

            if (contentNode is null)
                return Result.Of<IDiaValue>(new FormatException(
                    $"Invalid encoded metadata: '{encodedValueNode.TokenValue()}'",
                    new InvalidOperationException(
                        $"Missing symbol: '{SymbolNameValueMetadata}'")));

            return ValueMetadataParser
                .Parse(valueMetadataNode, context)
                .Bind(valueMetadata =>
                {
                    var valueTokens = contentNode.TokenValue();

                    if ("null".Equals(valueTokens))
                        return Result
                            .Of(IDiaValue.NullOf(valueMetadata.Type, valueMetadata.Annotations));

                    if (DiaType.Int.Equals(valueMetadata.Type))
                        return BigInteger
                            .Parse(valueTokens)
                            .ApplyTo(i => IntValue.Of(i, valueMetadata.Annotations))
                            .ApplyTo(i => valueMetadata.AddressIndex is not null
                                ? i.RelocateValue(context.Track(valueMetadata.AddressIndex.Value))
                                : i)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.Decimal.Equals(valueMetadata.Type))
                        return BigDecimal
                            .Parse(valueTokens)
                            .Map(d => DecimalValue.Of(d, valueMetadata.Annotations))
                            .Map(d => valueMetadata.AddressIndex is not null
                                ? d.RelocateValue(context.Track(valueMetadata.AddressIndex.Value))
                                : d)
                            .MapAs<IDiaValue>();

                    if (DiaType.Bool.Equals(valueMetadata.Type))
                        return bool
                            .Parse(valueTokens)
                            .ApplyTo(b => BoolValue.Of(b, valueMetadata.Annotations))
                            .ApplyTo(b => valueMetadata.AddressIndex is not null
                                ? b.RelocateValue(context.Track(valueMetadata.AddressIndex.Value))
                                : b)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.Clob.Equals(valueMetadata.Type))
                        return System.Convert
                            .FromBase64String(valueTokens)
                            .ApplyTo(Encoding.Unicode.GetString)
                            .ApplyTo(s => ClobValue.Of(s, valueMetadata.Annotations))
                            .ApplyTo(s => valueMetadata.AddressIndex is not null
                                ? s.RelocateValue(context.Track(valueMetadata.AddressIndex.Value))
                                : s)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.Blob.Equals(valueMetadata.Type))
                        return System.Convert
                            .FromBase64String(valueTokens)
                            .ApplyTo(s => BlobValue.Of(s, valueMetadata.Annotations))
                            .ApplyTo(s => valueMetadata.AddressIndex is not null
                                ? s.RelocateValue(context.Track(valueMetadata.AddressIndex.Value))
                                : s)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.Instant.Equals(valueMetadata.Type))
                        return EncodedValueParser
                            .ParseTimestamp(valueMetadata, contentNode!, context)
                            .MapAs<IDiaValue>();

                    if (DiaType.Symbol.Equals(valueMetadata.Type))
                        return SymbolValue
                            .Of(valueTokens, valueMetadata.Annotations)
                            .ApplyTo(s => valueMetadata.AddressIndex is not null
                                ? s.RelocateValue(context.Track(valueMetadata.AddressIndex.Value))
                                : s)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.String.Equals(valueMetadata.Type))
                        return StringValue
                            .Of(valueTokens, valueMetadata.Annotations)
                            .ApplyTo(s => valueMetadata.AddressIndex is not null
                                ? s.RelocateValue(context.Track(valueMetadata.AddressIndex.Value))
                                : s)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.Ref.Equals(valueMetadata.Type))
                        return int
                            .Parse(valueTokens[2..], NumberStyles.HexNumber)
                            .ApplyTo(i => context.Track(i))
                            .ApplyTo(address => ReferenceValue.Of(address, valueMetadata.Annotations))
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.List.Equals(valueMetadata.Type))
                        return ListValue
                            .Null(valueMetadata.Annotations)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    if (DiaType.Record.Equals(valueMetadata.Type))
                        return RecordValue
                            .Null(valueMetadata.Annotations)
                            .ApplyTo(Result.Of)
                            .MapAs<IDiaValue>();

                    throw new InvalidOperationException($"Invalid dia type: '{valueMetadata.Type}'");
                });
        }


        private static IResult<InstantValue> ParseTimestamp(
            ValueMetadataParser.ValueMetadata metadata,
            CSTNode valueContentNode,
            ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(valueContentNode);

            var contentNode = valueContentNode.FirstNode();
            var annotations = metadata.Annotations;

            var culture = CultureInfo.InvariantCulture;
            var result = contentNode.SymbolName switch
            {
                SymbolNameYear => Result
                    .Of(() => DateTimeOffset.ParseExact(
                        contentNode.TokenValue(),
                        YearFormat,
                        culture))
                    .Map(dto => InstantValue.Of(dto, annotations)),

                SymbolNameMonth => Result
                    .Of(() => DateTimeOffset.ParseExact(
                        contentNode.TokenValue(),
                        MonthFormat,
                        culture))
                    .Map(dto => InstantValue.Of(dto, annotations)),

                SymbolNameDay => Result
                    .Of(() => DateTimeOffset.ParseExact(
                        contentNode.TokenValue(),
                        DayFormat,
                        culture))
                    .Map(dto => InstantValue.Of(dto, annotations)),

                SymbolNameMinute => Result
                    .Of(() => DateTimeOffset.ParseExact(
                        contentNode.TokenValue(),
                        MinuteFormat,
                        culture))
                    .Map(dto => InstantValue.Of(dto, annotations)),

                SymbolNameSecond => Result
                    .Of(() => DateTimeOffset.ParseExact(
                        contentNode.TokenValue(),
                        SecondFormat,
                        culture))
                    .Map(dto => InstantValue.Of(dto, annotations)),

                SymbolNameMilliSecond => Result
                    .Of(() => DateTimeOffset.ParseExact(
                        contentNode.TokenValue(),
                        MillisecondFormat,
                        culture))
                    .Map(dto => InstantValue.Of(dto, annotations)),

                _ => Result.Of<InstantValue>(new FormatException(
                    $"Invalid value content: {valueContentNode.TokenValue()}",
                    new InvalidOperationException(
                        $"Invalid symbol encountered: '{contentNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullInstant}', '{SymbolNameYear}', '{SymbolNameMonth}', etc")))
            };

            return result.Map(i => metadata.AddressIndex is not null
                ? i.RelocateValue(context.Track(metadata.AddressIndex.Value))
                : i);
        }
    }
}
