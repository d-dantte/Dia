using Axis.Dia.PathQuery.Grammar;
using Axis.Dia.PathQuery.Matchers;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.PathQuery
{
    public class Segment
    {
        internal const string Symbol_Segment = "segment";
        internal const string Symbol_Negation = "negation";

        public IMatcher Matcher { get; }

        public Segment(IMatcher filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            Matcher = filter;
        }

        public static implicit operator Segment(string text) => Parse(text);

        public static Segment Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Segment];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, Segment>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static Segment Parse(ISymbolNode tokenPredicateNode)
        {
            return tokenPredicateNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(tokenPredicateNode)))
                .ThrowIf(
                    n => !Symbol_Segment.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .As<INodeContainer>().Nodes
                .Aggregate((IsNegated: false, Filter: default(IMatcher)), (info, node) => node.Symbol switch
                {
                    Symbol_Negation => info with { IsNegated = true },
                    IMatcher.Symbol_Matcher => info with { Filter = IMatcher.Parse(node, info.IsNegated) },
                    _ => info
                })
                .ApplyTo(info => new Segment(info.Filter!));
        }
    }
}
