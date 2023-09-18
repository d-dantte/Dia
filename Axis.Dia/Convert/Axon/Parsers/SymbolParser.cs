using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class SymbolParser : IValueSerializer<SymbolValue>
    {
        #region Symbols
        public const string SymbolNameDiaSymbol = "dia-symbol";
        public const string SymbolNameNullSymbol = "null-symbol";
        public const string SymbolNameQuotedSymbol = "quoted-symbol";
        public const string SymbolNameIdentifier = "identifier";
        #endregion

        private SymbolParser() { }

        public static string GrammarSymbol => SymbolNameDiaSymbol;

        public static IResult<SymbolValue> Parse(CSTNode symbolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            ArgumentNullException.ThrowIfNull(context);

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (AnnotationNode, ValueNode) = symbolNode.DeconstructValue();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                return ValueNode.SymbolName switch
                {
                    SymbolNameNullSymbol => annotationResult.Map(SymbolValue.Null),
                    SymbolNameIdentifier => ParseIdentifier(ValueNode, annotationResult),
                    SymbolNameQuotedSymbol => ParseQuotedSymbol(ValueNode, annotationResult),

                    _ => Result.Of<SymbolValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullSymbol}', or '{SymbolNameQuotedSymbol}', etc"))
                };
            }
            catch (Exception e)
            {
                return Result.Of<SymbolValue>(e);
            }
        }

        #region Quoted Symbol parser
        public static IResult<SymbolValue> ParseQuotedSymbol(
            CSTNode singlelineSymbolNode,
            IResult<Annotation[]> annotationsResult)
        {
            var slstring = singlelineSymbolNode.TokenValue();
            var unescapedValue = EscapeSequenceGroup
                .SymbolEscapeGroup
                .Unescape(slstring[1..^1]);

            return annotationsResult.Map(annotations => SymbolValue.Of(unescapedValue, annotations));
        }

        #region Identifier parser
        public static IResult<SymbolValue> ParseIdentifier(
            CSTNode identifierNode,
            IResult<Annotation[]> annotationsResult)
        {
            return annotationsResult.Map(annotations => SymbolValue.Of(
                identifierNode.TokenValue(),
                annotations));
        }
        #endregion
        #endregion


        public static IResult<string> Serialize(
            SymbolValue value,
            SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var intOptions = context.Options.Ints;

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.IsNull switch
            {
                true => Result.Of("null.symbol"),
                false =>  EscapeSequenceGroup.SymbolEscapeGroup
                    .Escape(value.Value)
                    .ApplyTo(Result.Of)
                    .Map(symbolText => SymbolValue.IdentifierPattern.IsMatch(symbolText)
                        ? symbolText
                        : symbolText.WrapIn("'"))
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{ann}{value}");
        }
    }
}
