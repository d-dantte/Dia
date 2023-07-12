using Axis.Dia.Types;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.IO.Text.Parsers
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

        public static IResult<DecimalValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                context ??= new TextSerializerContext();

                var (AnnotationNode, ValueNode) = symbolNode.DeconstructValue();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                return ValueNode.SymbolName switch
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
            }
            catch (Exception e)
            {
                return Result.Of<DecimalValue>(e);
            }
        }


        public static IResult<string> Serialize(DecimalValue value, TextSerializerContext? context = null)
        {
            context ??= new TextSerializerContext();
            var intOptions = context.Options.Ints;

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

            return annotationText!.Combine(valueText, (ann, value) => $"{ann}{value}");
        }
    }
}
