using Axis.Dia.PathQuery.Grammar;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.PathQuery.Predicates
{
    public interface ITokenPredicate
    {
        bool IsMatch(CharSequence chars, out int consumedTokenCount);


        internal const string Symbol_TokenPredicate = "token-predicate";
        internal const string Symbol_RegularExpression = "regular-expression";
        internal const string Symbol_WildcardExpression = "wildcard-expression";

        private static readonly NodePath ExpressionPath = $"{Symbol_RegularExpression}|{Symbol_WildcardExpression}";

        public static ITokenPredicate Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_TokenPredicate];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, ITokenPredicate>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static ITokenPredicate Parse(ISymbolNode tokenPredicateNode)
        {
            return tokenPredicateNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(tokenPredicateNode)))
                .ThrowIf(
                    n => !Symbol_TokenPredicate.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(ExpressionPath)
                .Select(node => node.Symbol switch
                {
                    Symbol_RegularExpression => (ITokenPredicate)RegexPredicate.Parse(node),
                    //Symbol_WildcardExpression
                    _ => IWildcardToken.Parse(node)
                })
                .First();
        }
    }
}
