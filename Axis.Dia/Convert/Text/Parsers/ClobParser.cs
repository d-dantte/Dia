using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Text.Parsers
{
    public class ClobParser : IValueSerializer<ClobValue>
    {
        #region Symbols
        public const string SymbolNameDiaClob = "dia-clob";
        public const string SymbolNameNullClob = "null-clob";
        public const string SymbolNameClobTextValue = "clob-text-value";
        #endregion


        private ClobParser() { }

        public static string GrammarSymbol => SymbolNameDiaClob;

        public static IResult<ClobValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
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
                    SymbolNameNullClob => annotationResult.Map(ClobValue.Null),
                    SymbolNameClobTextValue => ParseClob(ValueNode, annotationResult),

                    _ => Result.Of<ClobValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullClob}', or '{SymbolNameClobTextValue}', etc"))
                };
            }
            catch (Exception e)
            {
                return Result.Of<ClobValue>(e);
            }
        }

        private static IResult<ClobValue> ParseClob(CSTNode valueNode, IResult<Annotation[]> annotationResult)
        {
            var clobText = valueNode
                .TokenValue()
                .ApplyTo(EscapeSequenceGroup.ClobEscapeGroup.Unescape)!
                [2..^2];

            return annotationResult.Map(annotations => ClobValue.Of(clobText, annotations));
        }


        public static IResult<string> Serialize(ClobValue value, TextSerializerContext? context = null)
        {
            context ??= new TextSerializerContext();
            var intOptions = context.Options.Ints;

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.IsNull switch
            {
                true => Result.Of("null.clob"),
                false => EscapeSequenceGroup.ClobEscapeGroup
                    .Escape(value.Value)!
                    .WrapIn(
                        $"<<\\{Environment.NewLine}",
                        $"\\{Environment.NewLine}{context.Indentation()}>>")
                    .ApplyTo(Result.Of)
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{ann}{value}");
        }
    }
}
