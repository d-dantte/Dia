using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class BoolParser : IValueSerializer<BoolValue>
    {
        #region Symbols
        public const string SymbolNameDiaBool = "dia-bool";
        #endregion

        public static string GrammarSymbol => SymbolNameDiaBool;

        private BoolParser() { }

        public static IResult<BoolValue> Parse(CSTNode symbolNode, ParserContext context)
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

                return annotationResult.Map(annotations =>
                {
                    return ValueNode.TokenValue().ToLower() switch
                    {
                        "null.bool" => BoolValue.Null(annotations),
                        "true" => BoolValue.Of(true, annotations),
                        "false" => BoolValue.Of(false, annotations),
                        _ => throw new InvalidOperationException(
                            $"Invalid bool tokens: {ValueNode.TokenValue().ToLower()}")
                    };
                });
            }
            catch(Exception e)
            {
                return Result.Of<BoolValue>(e);
            }
        }


        public static IResult<string> Serialize(BoolValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return AnnotationParser
                .Serialize(value.Annotations, context)
                .Map(annotationText => $"{annotationText}{GetString(value.Value, context.Options.Bools)}");
        }

        private static string GetString(bool? value, SerializerOptions.BoolOptions boolOptions)
        {
            return value switch
            {
                null => "null.bool",
                bool @bool => boolOptions.ValueCase switch
                {
                    SerializerOptions.Case.Uppercase => @bool.ToString().ToUpper(),
                    SerializerOptions.Case.Lowercase => @bool.ToString().ToLower(),
                    SerializerOptions.Case.Titlecase => @bool.ToString(),
                    _ => throw new InvalidOperationException($"Unknown text case: {boolOptions.ValueCase}")
                }
            };
        }
    }
}

