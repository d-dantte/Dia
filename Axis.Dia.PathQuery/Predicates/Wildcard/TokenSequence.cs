using Axis.Dia.PathQuery.Grammar;
using Axis.Dia.PathQuery.Predicates;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using System.Collections.Immutable;
using System.Text;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.PathQuery.Predicates.Wildcard
{
    internal readonly struct TokenSequence : IWildcardToken
    {
        internal const string Symbol_Tokens = "wildcard-tokens";
        internal const string Symbol_Token = "wildcard-token";
        internal const string Symbol_GroupToken = "group-token";

        private static readonly NodePath WildcardTokenPath = Symbol_Token;
        private static readonly NodePath GroupToTokensPath = $"{Symbol_GroupToken}/{Symbol_Tokens}";

        public ImmutableArray<IWildcardToken> Tokens { get; }

        public TokenCardinality Cardinality { get; }

        public TokenSequence(TokenCardinality cardinality, params IWildcardToken[] tokens)
        {
            Cardinality = cardinality;
            Tokens = tokens
                .ThrowIfNull(() => new ArgumentNullException(nameof(tokens)))
                .ThrowIf(t => t.IsEmpty(), _ => new ArgumentException($"Invalid token array: empty"))
                .ThrowIfAny(t => t is null, _ => new InvalidOperationException($"Invalid token: null"))
                .ToImmutableArray();
        }

        public static TokenSequence Of(
            TokenCardinality cardinality,
            params IWildcardToken[] tokens)
            => new(cardinality, tokens);

        public static TokenSequence Of(
            params IWildcardToken[] tokens)
            => new(TokenCardinality.Of(1, 1), tokens);

        public static implicit operator TokenSequence(string text) => Parse(text);

        public override string ToString()
        {
            var cardinality = Cardinality;
            return Tokens
                .Aggregate(new StringBuilder(), (sb, next) => sb.Append(next))
                .ApplyTo(sb => (cardinality.MinOccurence, cardinality.MaxOccurence) switch
                {
                    (1, 1) => sb.ToString(),
                    _ => $"({sb}){cardinality}"
                });
        }

        public bool IsMatch(CharSequence chars, out int consumedTokenCount)
        {
            var @this = this;
            return Cardinality.IsMatch(chars, @this.IsMatch_, out consumedTokenCount);
        }

        private bool IsMatch_(CharSequence chars, out int consumedTokenCount)
        {
            var index = consumedTokenCount = 0;

            if (chars.IsDefault)
                return false;

            foreach (var token in Tokens)
            {
                if (token.IsMatch(chars[index..], out var tokenCount))
                {
                    consumedTokenCount = index += tokenCount;
                    continue;
                }

                else if (token.Cardinality.IsOptionalMinOccurence)
                    continue;

                else return false;
            }

            return true;
        }

        public static TokenSequence Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Tokens];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, TokenSequence>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static TokenSequence Parse(
            ISymbolNode node)
            => ParseTokens(node, TokenCardinality.Once);

        private static TokenSequence ParseTokens(ISymbolNode node, TokenCardinality cardinality)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !Symbol_Tokens.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(WildcardTokenPath)
                .Select(wildcardTokenNode =>
                {
                    var firstNode = wildcardTokenNode.As<INodeContainer>().Nodes[0];
                    return firstNode.Symbol switch
                    {
                        Token.Symbol_WildcardChar => (IWildcardToken)Token.Parse(wildcardTokenNode),
                        // Symbol_GroupToken
                        _ => ParseGroupToken(wildcardTokenNode)
                    };
                })
                .ApplyTo(tokens => Of(cardinality, tokens.ToArray()));
        }

        private static TokenSequence ParseGroupToken(ISymbolNode tokenNode)
        {
            var cardinality = tokenNode
                .FindNodes(TokenCardinality.Symbol_Cardinality)
                .FirstOrOptional()
                .Map(TokenCardinality.Parse)
                .ValueOr(TokenCardinality.Once);

            return tokenNode
                .FindNodes(GroupToTokensPath)
                .Select(node => ParseTokens(node, cardinality))
                .First();
        }
    }
}
