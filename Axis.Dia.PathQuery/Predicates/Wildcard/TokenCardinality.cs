using Axis.Dia.PathQuery.Grammar;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.PathQuery.Predicates.Wildcard
{
    internal readonly struct TokenCardinality
    {
        internal const string Symbol_Cardinality = "cardinality";
        internal const string Symbol_SimpleCardinality = "simple-cardinality";
        internal const string Symbol_ComplexCardinality = "complex-cardinality";
        internal const string Symbol_MaxOccurence = "max-occurence";

        private static readonly NodePath CardinalityTypePath = $"{Symbol_SimpleCardinality}|{Symbol_ComplexCardinality}";
        private static readonly NodePath MaxOccurencePath = $"{Symbol_MaxOccurence}";

        public static readonly TokenCardinality Once = new TokenCardinality(1, 1);
        public static readonly TokenCardinality OnceOrMore = new TokenCardinality(1, null);
        public static readonly TokenCardinality Optional = new TokenCardinality(0, 1);
        public static readonly TokenCardinality ZeroOrMore = default;


        public ushort MinOccurence { get; }

        public ushort? MaxOccurence { get; }

        public bool IsOptionalMinOccurence => MinOccurence == 0;

        public bool IsInfiniteMaxOccurence => MaxOccurence is null;

        public TokenCardinality(ushort minOccurence, ushort? maxOccurence)
        {
            MinOccurence = minOccurence;
            MaxOccurence = maxOccurence;

            Validate();
        }

        public static TokenCardinality Of(
            ushort minOccurence,
            ushort? maxOccurence)
            => new(minOccurence, maxOccurence);

        public static implicit operator TokenCardinality(string text) => Parse(text);

        public override string ToString()
        {
            return (MinOccurence, MaxOccurence) switch
            {
                (0, null) => "{*}",
                (1, null) => "{+}",
                (0, 1) => "{?}",
                (_, null) => $"{{{MinOccurence},+}}",
                _ => $"{{{MinOccurence},{MaxOccurence}}}"
            };
        }

        public bool IsMatch(
            CharSequence sequence,
            CardinalPredicate predicate,
            out int consumedTokenCount)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            var repetitions = 0;
            var position = 0;
            while (CanRepeat(repetitions))
            {
                if (!predicate.Invoke(sequence[position..], out var tokenCount))
                {
                    position += tokenCount;
                    break;
                }
                else
                {
                    position += tokenCount;
                    repetitions++;
                }
            }

            consumedTokenCount = position;
            return IsValidRepetition(repetitions);
        }

        public bool IsValidRepetition(int occurenceCount)
        {
            return occurenceCount >= MinOccurence
                && (MaxOccurence == null || occurenceCount <= MaxOccurence);
        }

        public bool CanRepeat(int completedRepetitions)
        {
            return MaxOccurence == null || completedRepetitions < MaxOccurence;
        }

        private void Validate()
        {
            if (MinOccurence > MaxOccurence || MinOccurence == 0 && MaxOccurence == 0)
                throw new InvalidOperationException(
                    $"Invalid cardinality! [min: {MinOccurence}, max: {MaxOccurence}");
        }

        public static TokenCardinality Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Cardinality];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, TokenCardinality>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static TokenCardinality Parse(ISymbolNode node)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !Symbol_Cardinality.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(CardinalityTypePath)
                .Select(node => node.Symbol switch
                {
                    Symbol_SimpleCardinality => ParseSimpleNotation(node),

                    //Symbol_ComplexCardinality
                    _ => ParseComplexNotation(node)
                })
                .First();
        }

        private static TokenCardinality ParseComplexNotation(ISymbolNode node)
        {
            var minOccurence = node
                .As<INodeContainer>()
                .Nodes[0].Tokens
                .ApplyTo(t => ushort.Parse(t));

            var maxOccurence = node
                .FindNodes(Symbol_MaxOccurence)
                .First()
                .Tokens.ToString()! switch
            {
                "+" => default(ushort?),
                string s => ushort.Parse(s)
            };

            return Of(minOccurence, maxOccurence);
        }

        private static TokenCardinality ParseSimpleNotation(ISymbolNode node)
        {
            return node.Tokens.ToString()! switch
            {
                "*" => Of(0, null),
                "?" => Of(0, 1),
                "+" => Of(1, null),
                string s => ushort
                    .Parse(s)
                    .ApplyTo(digits => Of(digits, digits))
            };
        }
    }

    internal delegate bool CardinalPredicate(CharSequence chars, out int consumedTokenCount);
}
