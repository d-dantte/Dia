using Axis.Dia.IO.Text.Parsers;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.IO.Text
{
    internal static class TextExtensions
    {
        internal static (CSTNode? AnnotationNode, CSTNode ValueNode) DeconstructValue(this CSTNode valueRoot)
        {
            ArgumentNullException.ThrowIfNull(valueRoot);

            var annotationNode = valueRoot
                .FindNodes(AnnotationParser.GrammarSymbol)
                .FirstOrDefault();
            var valueNode = annotationNode is null
                ? valueRoot.FirstNode()
                : valueRoot.NodeAt(1);

            return (annotationNode, valueNode);
        }

        internal static string ReverseString(this string s)
        {
            var array = new char[s.Length];
            int forward = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                array[forward++] = s[i];
            }
            return new string(array);
        }
    }
}
