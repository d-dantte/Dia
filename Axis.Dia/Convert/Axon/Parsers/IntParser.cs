using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using System.Globalization;
using System.Numerics;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class IntParser : IValueSerializer<IntValue>
    {
        #region Symbols
        public const string SymbolNameDiaInt = "dia-int";
        public const string SymbolNameNullInt = "null-int";
        public const string SymbolNameIntNumber = "int-number";

        public const string SymbolNameNegativeSign = "negative-sign";
        public const string SymbolNameIntNotationn = "int-notation";
        public const string SymbolNameBinaryNotationn = "binary-int";
        public const string SymbolNameHexNotationn = "hex-int";
        public const string SymbolNameRegularNotationn = "regular-int";
        #endregion


        private IntParser() { }

        public static string GrammarSymbol => SymbolNameDiaInt;

        public static IResult<IntValue> Parse(CSTNode symbolNode, ParserContext context)
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

                var result = ValueNode.SymbolName switch
                {
                    SymbolNameNullInt => annotationResult.Map(IntValue.Null),
                    SymbolNameIntNumber => ParseIntNumber(ValueNode, annotationResult),

                    _ => Result.Of<IntValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullInt}', or '{SymbolNameIntNumber}'"))
                };

                return AddressIndexNode is not null
                    ? result.Combine(
                        AddressIndexParser.Parse(AddressIndexNode),
                        (value, addressIndex) => value.RelocateValue(context.Track(addressIndex)))
                    : result;
            }
            catch (Exception e)
            {
                return Result.Of<IntValue>(e);
            }
        }

        private static IResult<IntValue> ParseIntNumber(
            CSTNode intSymbol,
            IResult<Annotation[]> annotationResult)
        {
            var isNegative = intSymbol
                .FindNodes(SymbolNameNegativeSign)
                .FirstOrOptional()
                .Map(value => value.TokenValue())
                .Map(value => value.Equals("-"))
                .ValueOr(false);

            var notations = $"{SymbolNameBinaryNotationn}|{SymbolNameHexNotationn}|{SymbolNameRegularNotationn}";
            var searchPath = $"{SymbolNameIntNotationn}/{notations}";
            var notation = intSymbol
                .FindNodes(searchPath)
                .FirstOrDefault();
                
            return notation?.SymbolName switch
            {
                SymbolNameBinaryNotationn => notation
                    .TokenValue()[2..] // remove the '0b' prefix, and any negative sign
                    .ReverseString()
                    .ApplyTo(BitSequence.Parse)
                    .Map(bs => new BigInteger(bs.ToByteArray(), true))
                    .Map(@int => @int * (isNegative ? -1 : 1))
                    .Combine(annotationResult, (@int, annotations) => IntValue.Of(@int, annotations)),

                SymbolNameHexNotationn => notation
                    .TokenValue()[2..] // remove the '0x' prefix, and any negative sign
                    .Replace("_", "")
                    .ApplyTo(hex => BigInteger.Parse(hex, NumberStyles.HexNumber))
                    .ApplyTo(@int => @int * (isNegative ? -1 : 1))
                    .ApplyTo(@int => annotationResult.Map(annotations => new IntValue(@int, annotations))),

                SymbolNameRegularNotationn => notation
                    .TokenValue()
                    .Replace("_", "")
                    .ApplyTo(dec => BigInteger.Parse(dec, NumberStyles.Integer))
                    .ApplyTo(@int => @int * (isNegative ? -1 : 1))
                    .ApplyTo(@int => annotationResult.Map(annotations => new IntValue(@int, annotations))),

                _ => Result.Of<IntValue>(new ArgumentException(
                    $"Invalid symbol encountered. "
                    + $"Expected '{SymbolNameBinaryNotationn}', '{SymbolNameHexNotationn}', etc"))
            };
        }


        public static IResult<string> Serialize(IntValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var intOptions = context.Options.Ints;

            var addressIndexText = context.TryGetAddressIndex(value, out var index)
                ? $"#0x{index:x}"
                : "";

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.IsNull switch
            {
                true => Result.Of("null.int"),
                false => ConvertToText(value, intOptions)
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{addressIndexText}{ann}{value}");
        }

        internal static IResult<string> ConvertToText(IntValue value, SerializerOptions.IntOptions options)
        {
            var bigInt = value.Value ?? throw new ArgumentException("Invalid int value: 'null'");
            return Result.Of(() => options.NumberFormat switch
            {
                SerializerOptions.IntFormat.Decimal => bigInt
                    .ApplyTo(@int => (sign: @int >= 0, unsigned: @int.ToString().TrimStart("-")))
                    .ApplyTo(tuple => options.UseDigitSeparator
                        ? (tuple.sign, unsigned: ApplySeparator(tuple.unsigned, 3))
                        : tuple)
                    .ApplyTo(tuple => $"{(tuple.sign ? "" : "-")}{tuple.unsigned}"),

                SerializerOptions.IntFormat.BigHex => BigInteger
                    .Abs(bigInt)
                    .ToString("X")
                    .ApplyTo(hex => options.UseDigitSeparator
                        ? ApplySeparator(hex, 4)
                        : hex)
                    .ApplyTo(hex => $"{(bigInt < 0 ? "-" : "")}0X{hex}"),

                SerializerOptions.IntFormat.SmallHex => BigInteger
                    .Abs(bigInt)
                    .ToString("x")
                    .ApplyTo(hex => options.UseDigitSeparator
                        ? ApplySeparator(hex, 4)
                        : hex)
                    .ApplyTo(hex => $"{(bigInt < 0 ? "-" : "")}0x{hex}"),

                SerializerOptions.IntFormat.BigBinary => BigInteger
                    .Abs(bigInt)
                    .ApplyTo(BitSequence.Of)
                    .ApplyTo(ToBinaryString)
                    .ApplyTo(bin => options.UseDigitSeparator
                        ? ApplySeparator(bin, 4)
                        : bin)
                    .ApplyTo(bin => $"{(bigInt < 0 ? "-" : "")}0B{bin}"),

                SerializerOptions.IntFormat.SmallBinary => BigInteger
                    .Abs(bigInt)
                    .ApplyTo(BitSequence.Of)
                    .ApplyTo(ToBinaryString)
                    .ApplyTo(bin => options.UseDigitSeparator
                        ? ApplySeparator(bin, 4)
                        : bin)
                    .ApplyTo(bin => $"{(bigInt < 0 ? "-" : "")}0b{bin}"),

                _ => throw new ArgumentException($"Invalid number format specified: {options.NumberFormat}")
            });
        }

        internal static string ApplySeparator(string value, int interval)
        {
            return value
                .Reverse()
                .Select((@char, index) => (@char, index))
                .GroupBy(tuple => tuple.index / interval, tuple => tuple.@char)
                .Select(group => new string(group.ToArray()))
                .JoinUsing("_")
                .Reverse();
        }

        internal static string ToBinaryString(BitSequence bitSequence)
        {
            return bitSequence.SignificantBits
                .Select(bit => bit ? "1" : "0")
                .JoinUsing("")
                .ReverseString();
        }
    }
}
