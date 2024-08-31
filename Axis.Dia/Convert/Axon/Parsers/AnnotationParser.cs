using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using Axis.Luna.Result;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public static class AnnotationParser
    {
        #region Symbols
        public const string SymbolNameAnnotationList = "annotation-list";
        public const string SymbolNameAnnotation = "annotation";
        #endregion

        public static string GrammarSymbol => SymbolNameAnnotationList;

        #region Annotation Array
        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotations"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IResult<string> Serialize(
            Annotation[] annotations,
            SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(annotations);
            ArgumentNullException.ThrowIfNull(context);

            return annotations
                .Select(Serialize)
                .FoldInto(texts => texts.JoinUsing(""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IResult<Annotation[]> Parse(
            string text,
            ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"Invalid text: '{text}'");

            var parseResult = GrammarUtil.Grammar
                .GetRecognizer(GrammarSymbol)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => Parse(success.Symbol, context),
                null => Result.Of<Annotation[]>(new Exception("Unknown Error")),
                _ => Result.Of<Annotation[]>(new ParseException(parseResult))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IResult<Annotation[]> Parse(
            CSTNode symbolNode,
            ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            ArgumentNullException.ThrowIfNull(context);

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            return symbolNode
                .FindNodes(SymbolNameAnnotation)
                .Select(ParseAnnotation)
                .FoldInto(annotations => annotations.ToArray());
        }
        #endregion

        #region Annotation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbolNode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<Annotation> ParseAnnotation(CSTNode symbolNode)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);

            if (!SymbolNameAnnotation.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{symbolNode}', "
                    + $"but found '{symbolNode.SymbolName}'");

            return Result.Of(() => symbolNode
                .TokenValue()
                .UnwrapFrom("'")
                .ApplyTo(EscapeSequenceGroup.SymbolEscapeGroup.Unescape)
                .ApplyTo(text => Annotation.Of(text!)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public static IResult<string> Serialize(Annotation annotation)
        {
            if (Annotation.Default.Equals(annotation))
                return Result.Of<string>(new ArgumentException($"Invalid anntotation: {annotation}"));

            return annotation.Text
                .ApplyTo(EscapeSequenceGroup.SymbolEscapeGroup.Escape)
                .ApplyTo(text => !annotation.IsIdentifier ? text.WrapIn("'") : text)
                .ApplyTo(text => $"{text}::")
                .ApplyTo(Result.Of);
        }
        #endregion
    }
}

