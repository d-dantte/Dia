using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Convert.Text.Parsers
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
            TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            return annotations
                .Select(Serialize)
                .Fold()
                .Map(texts => texts.JoinUsing(""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IResult<Annotation[]> Parse(
            string text,
            TextSerializerContext? context = null)
        {
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
            TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            context ??= new TextSerializerContext();

            return symbolNode
                .FindNodes(SymbolNameAnnotation)
                .Select(ParseAnnotation)
                .Fold()
                .Map(annotations => annotations.ToArray());
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
            return annotation.Text
                .ApplyTo(EscapeSequenceGroup.SymbolEscapeGroup.Escape)
                .ApplyTo(text => !annotation.IsIdentifier ? text.WrapIn("'") : text)
                .ApplyTo(text => $"{text}::")
                .ApplyTo(Result.Of);
        }
        #endregion
    }
}

