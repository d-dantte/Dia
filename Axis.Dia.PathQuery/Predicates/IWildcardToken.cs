using Axis.Dia.PathQuery.Grammar;
using Axis.Dia.PathQuery.Predicates.Wildcard;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.PathQuery.Predicates
{
    internal interface IWildcardToken : ITokenPredicate
    {
        internal const string Symbol_WildcardToken = "wildcard-token";

        private static readonly NodePath TokensPath = TokenSequence.Symbol_Tokens;

        TokenCardinality Cardinality { get; }

        new public static IWildcardToken Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_WildcardExpression];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, IWildcardToken>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        new internal static IWildcardToken Parse(ISymbolNode wildcardExpressionNode)
        {
            return wildcardExpressionNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(wildcardExpressionNode)))
                .ThrowIf(
                    n => !Symbol_WildcardExpression.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(TokensPath)
                .Select(TokenSequence.Parse)
                .First();
        }
    }
}
