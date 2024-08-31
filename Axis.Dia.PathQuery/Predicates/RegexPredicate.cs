using Axis.Dia.PathQuery.Grammar;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using System.Text.RegularExpressions;

namespace Axis.Dia.PathQuery.Predicates
{
    public readonly struct RegexPredicate : ITokenPredicate
    {
        internal const string Symbol_RegularExpression = "regular-expression";

        public readonly Regex Regex { get; }

        public RegexPredicate(Regex regex)
        {
            ArgumentNullException.ThrowIfNull(regex);
            Regex = regex;
        }

        public RegexPredicate(
            string pattern)
            : this(new Regex(pattern, RegexOptions.Compiled))
        { }

        public static RegexPredicate Of(Regex regex) => new(regex);

        public static RegexPredicate Of(string pattern) => new(pattern);

        public bool IsMatch(CharSequence chars, out int consumedTokenCount)
        {
            var match = Regex.Match(chars.ToString())!;

            if (match.Success)
            {
                consumedTokenCount = match.Value.Length;
                return true;
            }

            consumedTokenCount = 0;
            return false;
        }

        public override string ToString()
        {
            return $"`{Regex}`";
        }


        public static RegexPredicate Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_RegularExpression];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, RegexPredicate>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static RegexPredicate Parse(ISymbolNode node)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !Symbol_RegularExpression.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .Tokens[1..^1]
                .ApplyTo(tokens => new Regex(tokens.ToString()!, RegexOptions.Compiled))
                .ApplyTo(Of);
        }
    }
}
