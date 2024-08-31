using Axis.Dia.PathQuery.Grammar;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using Axis.Dia.Core;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.PathQuery.Matchers
{
    /// <summary>
    /// Marker interface for matchers
    /// </summary>
    public interface IMatcher
    {
        internal const string Symbol_Matcher = "matcher";

        bool IsNegated { get; }


        public static IMatcher Parse(string text, bool isNegated = false)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Matcher];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, IMatcher>(n => Parse(n, isNegated));

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static IMatcher Parse(ISymbolNode tokenPredicateNode, bool isNegated = false)
        {
            return tokenPredicateNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(tokenPredicateNode)))
                .ThrowIf(
                    n => !Symbol_Matcher.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .As<INodeContainer>().Nodes
                .Select(node => node.Symbol switch
                {
                    PropertyNameMatcher.Symbol_PropertyNameMatcher => PropertyNameMatcher.Parse(node, isNegated),
                    AttributeMatcher.Symbol_AttributeMatcher => AttributeMatcher.Parse(node, isNegated),
                    IndexRangeMatcher.Symbol_IndexRangeMatcher => IndexRangeMatcher.Parse(node, isNegated),

                    //PropertyNameMatcher.Symbol_PropertyFilter 
                    _ => (IMatcher)TypeMatcher.Parse(node, isNegated),
                })
                .First();
        }
    }

    /// <summary>
    /// Match-action contract intarface.
    /// </summary>
    /// <typeparam name="TOperand">The data, extracted from the <see cref="IDiaValue"/>, upon which the match action is carried out</typeparam>
    /// <typeparam name="TMatchInfo">The data that was matched</typeparam>
    public interface IMatcher<TOperand, TMatchInfo>: IMatcher
    {
        bool IsMatch(TOperand operand, out TMatchInfo matchInfo);
    }
}
