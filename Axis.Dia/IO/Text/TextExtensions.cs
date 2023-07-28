using Axis.Dia.IO.Text.Parsers;
using Axis.Pulsar.Grammar;
using Axis.Pulsar.Grammar.CST;
using System.Text;

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

        internal static bool Contains(this StringBuilder builder, char @char)
        {
            ArgumentNullException.ThrowIfNull(builder);

            for(int index = 0; index < builder.Length; index++)
            {
                if (builder[index] == @char)
                    return true;
            }

            return false;
        }

        internal static void AddRange<TItem>(ICollection<TItem> items, params TItem[] addendum)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(addendum);

            foreach (var item in addendum)
                items.Add(item);
        }

        internal static char Peek(this BufferedTokenReader reader)
        {
            if (!reader.TryNextToken(out var token))
                throw new EndOfStreamException();

            reader.Back();
            return token;
        }
    }
}
