using Axis.Dia.PathQuery.Grammar;
using Axis.Dia.PathQuery.Predicates;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.PathQuery.Predicates.Wildcard
{
    internal readonly struct Token : IWildcardToken
    {
        internal const string Symbol_WildcardChar = "wildcard-char";

        private static readonly NodePath Path = $"{Symbol_WildcardChar}|{TokenCardinality.Symbol_Cardinality}";

        public char? Char { get; }

        public bool IsWildcard => Char is null;

        public TokenCardinality Cardinality { get; }

        public Token(TokenCardinality cardinality, char? @char)
        {
            Cardinality = cardinality;
            Char = @char;
        }

        public static Token Of(
            TokenCardinality cardinality,
            char? @char)
            => new(cardinality, @char);

        public static Token Of(char? @char) => Of(TokenCardinality.Of(1, 1), @char);

        public bool IsMatch(CharSequence chars, out int consumedTokenCount)
        {
            var @this = this;

            if (Cardinality.MinOccurence == 1 && Cardinality.MaxOccurence == 1)
                return IsMatch_(chars, out consumedTokenCount);

            else return Cardinality.IsMatch(chars, @this.IsMatch_, out consumedTokenCount);
        }

        private bool IsMatch_(CharSequence chars, out int consumedTokenCount)
        {
            consumedTokenCount = 0;
            if (chars.IsDefault)
                return false;

            if (!chars.IsEmpty && (IsWildcard || chars[0].Equals(Char)))
            {
                consumedTokenCount = 1;
                return true;
            }

            else return false;
        }

        public override string ToString()
        {
            var charText = Char switch
            {
                null => "_",
                '_' => "\\_",
                _ => Char!.Value.ToString()
            };

            var cardinalityText = (Cardinality.MinOccurence, Cardinality.MaxOccurence) switch
            {
                (1, 1) => string.Empty,
                _ => Cardinality.ToString()
            };

            return $"{charText}{cardinalityText}";
        }

        public static implicit operator Token(char? @char) => Of(TokenCardinality.Of(1, 1), @char);

        public static Token Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[IWildcardToken.Symbol_WildcardToken];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, Token>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static Token Parse(ISymbolNode node)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !IWildcardToken.Symbol_WildcardToken.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(Path)
                .Aggregate(
                    (Char: default(char?), Cardinality: TokenCardinality.Of(1, 1)),
                    (info, nextNode) => nextNode.Symbol switch
                    {
                        Symbol_WildcardChar => info with { Char = GetChar(nextNode.Tokens) },

                        // TokenCardinality.Symbol_Cardinality
                        _ => info with { Cardinality = TokenCardinality.Parse(nextNode) }
                    })
                .ApplyTo(info => Of(info.Cardinality, info.Char));
        }

        private static char? GetChar(string text)
        {
            return text switch
            {
                "_" => null,
                "\\_" => '_',
                _ => text.Length != 1
                    ? throw new FormatException($"Invalid char text: {text}")
                    : text[0]
            };
        }
    }
}
