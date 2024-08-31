using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Grammar;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using System.Collections.Immutable;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.PathQuery.Matchers
{
    public class IndexRangeMatcher : IMatcher<Sequence, ImmutableArray<Range>>
    {
        internal const string Symbol_IndexRangeMatcher = "index-range-matcher";
        internal const string Symbol_DigitNotation = "digits";
        internal const string Symbol_IndexNotation = "index-notation";
        internal const string Symbol_Index = "index";

        private static readonly NodePath NotationPath = $"{Symbol_DigitNotation}|{Symbol_IndexNotation}";

        public Range Range { get; }

        public bool IsNegated { get; }

        public IndexRangeMatcher(bool isNegated, Range range)
        {
            Range = range;
            IsNegated = isNegated;
        }

        public IndexRangeMatcher(
            Range range)
            : this(false, range)
        { }

        /// <summary>
        /// Determines if the encapsulated range is valid across the given sequence. Negated matches return the difference set of the valid range.
        /// <para/>
        /// If the encapsulated range is invalid, false and an empty range array are returned - same goes for the negated state.
        /// </summary>
        /// <param name="sequence">The sequence to test</param>
        /// <param name="normalizedRanges">The ranges meeting the match criteria</param>
        /// <returns>True if there is a valid match, false otherwise</returns>
        public bool IsMatch(Sequence sequence, out ImmutableArray<Range> normalizedRanges)
        {
            var isValidRange = IsValidRange(Range, sequence.Count, out var normalizedRange);
            var minIndex = Min(normalizedRange);
            var maxIndex = Max(normalizedRange);

            var left = new Range(0, minIndex < 0 ? 0 : minIndex);
            var right = new Range(maxIndex > sequence.Count ? sequence.Count : maxIndex, sequence.Count);

            if (isValidRange)
            {
                if (IsNegated)
                    normalizedRanges = ImmutableArray.Create(left, right);

                else
                    normalizedRanges = ImmutableArray.Create(normalizedRange);

                return true;
            }
            else
            {
                normalizedRanges = ImmutableArray.Create<Range>();
                return false;
            }
        }

        internal static int GetIndex(Index index, int sequenceLength)
        {
            return index.IsFromEnd switch
            {
                true => sequenceLength - index.Value,
                false => index.Value
            };
        }

        internal static int Min(Range range)
        {
            return int.Min(range.Start.Value, range.End.Value);
        }

        internal static int Max(Range range)
        {
            return int.Max(range.Start.Value, range.End.Value);
        }

        internal static bool IsValidRange(Range range, int sequenceLength, out Range normalizedRange)
        {
            normalizedRange = new Range(
                GetIndex(range.Start, sequenceLength),
                GetIndex(range.End, sequenceLength));

            return normalizedRange.Start.Value >= 0
                && normalizedRange.Start.Value < sequenceLength
                && normalizedRange.End.Value <= sequenceLength
                && normalizedRange.End.Value >= -1;
        }


        #region Parse

        public static IndexRangeMatcher Parse(string text, bool isNegated = false)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_IndexRangeMatcher];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, IndexRangeMatcher>(v => Parse(v, isNegated));

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static IndexRangeMatcher Parse(ISymbolNode node, bool isNegated = false)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !Symbol_IndexRangeMatcher.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(NotationPath)
                .Select(node => node.Symbol switch
                {
                    Symbol_DigitNotation => int
                        .Parse(node.Tokens)
                        .ApplyTo(digits => new IndexRangeMatcher(isNegated, new Range(digits, digits))),

                    // Symbol_IndexNotation
                    _ => ParseIndexNotation(isNegated, node)
                })
                .First();
        }

        private static IndexRangeMatcher ParseIndexNotation(bool isNegated, ISymbolNode indexNotationNode)
        {
            var indices = indexNotationNode
                .As<INodeContainer>().Nodes
                .ToArray();

            var start = Symbol_Index.Equals(indices[0].Symbol)
                ? ParseIndex(indices[0])
                : 0;

            var end = Symbol_Index.Equals(indices[^1].Symbol)
                ? ParseIndex(indices[^1])
                : ^0;

            return new IndexRangeMatcher(isNegated, new Range(start, end));
        }

        private static Index ParseIndex(ISymbolNode indexNode)
        {
            var nodes = indexNode
                .As<INodeContainer>().Nodes
                .ToArray();

            if (nodes.Length == 1)
                return new Index(int.Parse(nodes[0].Tokens));

            else return new Index(int.Parse(nodes[^1].Tokens), true);
        }
        #endregion
    }
}
