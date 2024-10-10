using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal static class AnnotationParser
    {
        internal const string SymbolNameAnnotationList = "annotation-list";
        internal const string SymbolNameAnnotation = "annotation";

        public static string RootSymbol => SymbolNameAnnotationList;

        #region Annotation list
        public static IResult<string> Serialize(Annotation[] annotations, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(annotations);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            return annotations
                .Select(annotation => SerializeAnnotation(annotation, context))
                .FoldInto(texts => texts.JoinUsing(""));
        }

        public static IResult<Annotation[]> Parse(CSTNode annotationListNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(annotationListNode);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (!RootSymbol.Equals(annotationListNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol '{annotationListNode.SymbolName}', expected '{SymbolNameAnnotationList}'");

            return annotationListNode
                .FindNodes(SymbolNameAnnotation)
                .Select(annotation => ParseAnnotation(annotation, context))
                .FoldInto(annotations => annotations.ToArray());
        }
        #endregion

        #region Annotation
        public static IResult<string> SerializeAnnotation(Annotation annotation, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (Annotation.Default.ValueEquals(annotation))
                return Result.Of<string>(new ArgumentException($"Invalid anntotation: {annotation}"));

            return annotation.Text
                .ApplyTo(EscapeSequenceGroup.SymbolEscapeGroup.Escape)
                .ApplyTo(text => !annotation.IsIdentifier ? text.WrapIn("'") : text)
                .ApplyTo(text => $"{text};")
                .ApplyTo(Result.Of);
        }

        public static IResult<Annotation> ParseAnnotation(CSTNode annotationNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(annotationNode);
            ArgumentNullException.ThrowIfNull(context);

            if (!SymbolNameAnnotation.Equals(annotationNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol '{annotationNode.SymbolName}', expected '{SymbolNameAnnotation}'");

            return Result.Of(() => annotationNode
                .TokenValue()
                .UnwrapFrom("'")
                .ApplyTo(EscapeSequenceGroup.SymbolEscapeGroup.Unescape)
                .ApplyTo(text => Annotation.Of(text!)));
        }
        #endregion
    }
}
