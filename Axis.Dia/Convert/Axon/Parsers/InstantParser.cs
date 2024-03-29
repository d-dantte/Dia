﻿using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using System.Globalization;
using static Axis.Dia.Convert.Axon.SerializerOptions;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class InstantParser : IValueSerializer<InstantValue>
    {
        #region Symbols
        public const string SymbolNameDiaInstant = "dia-instant";
        public const string SymbolNameNullInstant = "null-instant";
        public const string SymbolNameMilliSecond = "millisecond-precision";
        public const string SymbolNameSecond = "second-precision";
        public const string SymbolNameMinute = "minute-precision";
        public const string SymbolNameDay = "day-precision";
        public const string SymbolNameMonth = "month-precision";
        public const string SymbolNameYear = "year-precision";
        #endregion

        #region Formats
        private static readonly string YearFormat = "yyyyT";
        private static readonly string MonthFormat = "yyyy-MMT";
        private static readonly string DayFormat = "yyyy-MM-dd";
        private static readonly string MinuteFormat = "yyyy-MM-ddTHH:mmzzz";
        private static readonly string SecondFormat = "yyyy-MM-ddTHH:mm:sszzz";
        private static readonly string MillisecondFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";
        #endregion


        private InstantParser() { }

        public static string GrammarSymbol => SymbolNameDiaInstant;

        public static IResult<InstantValue> Parse(CSTNode symbolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            context.ThrowIfDefault($"Invalid {nameof(context)} instance");

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (AddressIndexNode, AnnotationNode, ValueNode) = symbolNode.DeconstructValueNode();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                var culture = CultureInfo.InvariantCulture;
                var result = ValueNode.SymbolName switch
                {
                    SymbolNameNullInstant => annotationResult.Map(InstantValue.Null),

                    SymbolNameYear => Result
                        .Of(() => DateTimeOffset.ParseExact(
                            ValueNode.TokenValue(),
                            YearFormat,
                            culture))
                        .Combine(annotationResult, (dto, ann) => InstantValue.Of(dto, ann)),

                    SymbolNameMonth => Result
                        .Of(() => DateTimeOffset.ParseExact(
                            ValueNode.TokenValue(),
                            MonthFormat,
                            culture))
                        .Combine(annotationResult, (dto, ann) => InstantValue.Of(dto, ann)),

                    SymbolNameDay => Result
                        .Of(() => DateTimeOffset.ParseExact(
                            ValueNode.TokenValue(),
                            DayFormat,
                            culture))
                        .Combine(annotationResult, (dto, ann) => InstantValue.Of(dto, ann)),

                    SymbolNameMinute => Result
                        .Of(() => DateTimeOffset.ParseExact(
                            ValueNode.TokenValue(),
                            MinuteFormat,
                            culture))
                        .Combine(annotationResult, (dto, ann) => InstantValue.Of(dto, ann)),

                    SymbolNameSecond => Result
                        .Of(() => DateTimeOffset.ParseExact(
                            ValueNode.TokenValue(),
                            SecondFormat,
                            culture))
                        .Combine(annotationResult, (dto, ann) => InstantValue.Of(dto, ann)),

                    SymbolNameMilliSecond => Result
                        .Of(() => DateTimeOffset.ParseExact(
                            ValueNode.TokenValue(),
                            MillisecondFormat,
                            culture))
                        .Combine(annotationResult, (dto, ann) => InstantValue.Of(dto, ann)),

                    _ => Result.Of<InstantValue>(new ArgumentException(
                        $"Invalid symbol encountered: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullInstant}', '{SymbolNameYear}', '{SymbolNameMonth}', etc"))
                };

                return AddressIndexNode is not null
                    ? result.Combine(
                        AddressIndexParser.Parse(AddressIndexNode),
                        (value, addressIndex) => value.RelocateValue(context.Track(addressIndex)))
                    : result;
            }
            catch (Exception e)
            {
                return Result.Of<InstantValue>(e);
            }
        }


        public static IResult<string> Serialize(InstantValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var intOptions = context.Options.Ints;

            var addressIndexText = context.TryGetAddressIndex(value, out var index)
                ? $"#0x{index:x}"
                : "";

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.IsNull switch
            {
                true => Result.Of("null.instant"),
                false => context.Options.Timestamps.TimestampPrecision switch
                {
                    TimestampPrecision.Year => Result.Of(value.Value!.Value.ToString(YearFormat)),
                    TimestampPrecision.Month => Result.Of(value.Value!.Value.ToString(MonthFormat)),
                    TimestampPrecision.Day => Result.Of(value.Value!.Value.ToString(DayFormat)),
                    TimestampPrecision.Minute => Result.Of(value.Value!.Value.ToString(MinuteFormat)),
                    TimestampPrecision.Second => Result.Of(value.Value!.Value.ToString(SecondFormat)),
                    TimestampPrecision.MilliSecond => Result.Of(value.Value!.Value.ToString(MillisecondFormat)),
                    _ => Result.Of<string>(new InvalidOperationException(
                        $"Invalid timestamp precision: {context.Options.Timestamps.TimestampPrecision}"))
                }
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{ann}{value}");
        }
    }
}
