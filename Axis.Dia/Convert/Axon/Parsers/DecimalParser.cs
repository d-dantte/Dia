using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class DecimalParser : IValueSerializer<DecimalValue>
    {
        #region Symbols
        public const string SymbolNameDiaDecimal = "dia-decimal";
        public const string SymbolNameNullDecimal = "null-decimal";
        public const string SymbolNameScientificDecimal = "scientific-decimal";
        public const string SymbolNameRegularDecimal = "regular-decimal";
        #endregion


        private DecimalParser() { }

        public static string GrammarSymbol => SymbolNameDiaDecimal;

        public static IResult<DecimalValue> Parse(CSTNode symbolNode, ParserContext context)
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
                    SymbolNameNullDecimal => annotationResult.Map(DecimalValue.Null),
                    SymbolNameScientificDecimal
                    or SymbolNameRegularDecimal => ValueNode
                        .TokenValue()
                        .Replace("_", "")
                        .ApplyTo(BigDecimal.Parse)
                        .Combine(
                            annotationResult,
                            (@decimal, annotations) => DecimalValue.Of(@decimal, annotations)),                        

                    _ => Result.Of<DecimalValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullDecimal}', or '{SymbolNameScientificDecimal}', etc..."))
                };

                return AddressIndexNode is not null
                    ? result.Combine(
                        AddressIndexParser.Parse(AddressIndexNode),
                        (value, addressIndex) => value.RelocateValue(context.Track(addressIndex)))
                    : result;
            }
            catch (Exception e)
            {
                return Result.Of<DecimalValue>(e);
            }
        }


        public static IResult<string> Serialize(DecimalValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var intOptions = context.Options.Ints;

            var addressIndexText = context.TryGetAddressIndex(value, out var index)
                ? $"#0x{index:x}"
                : "";

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.Value switch
            {
                null => Result.Of("null.decimal"),
                BigDecimal bd => Result.Of(context.Options.Decimals.UseExponentNotation switch
                {
                    true => bd.ToScientificString(context.Options.Decimals.MaxPrecision),
                    false => bd.ToNonScientificString(context.Options.Decimals.MaxPrecision)
                })
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{addressIndexText}{ann}{value}");
        }
    }
}
