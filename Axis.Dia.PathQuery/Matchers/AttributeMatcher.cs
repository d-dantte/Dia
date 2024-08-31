using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Grammar;
using Axis.Dia.PathQuery.Predicates;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using System.Collections.Immutable;

namespace Axis.Dia.PathQuery.Matchers
{
    public class AttributeMatcher : IMatcher<IAttributeContainer, ImmutableHashSet<Core.Types.Attribute>>
    {
        internal const string Symbol_AttributeMatcher = "attribute-matcher";
        internal const string Symbol_AttributeExpression = "attribute-expression";
        internal const string Symbol_AttributeKey = "attribute-key";
        internal const string Symbol_AttributeValue = "attribute-value";

        private static readonly NodePath AttributeExpressionPath = Symbol_AttributeExpression;
        private static readonly NodePath AttributeKeyPath = $"{Symbol_AttributeKey}/{ITokenPredicate.Symbol_TokenPredicate}";
        private static readonly NodePath AttributeValuePath = $"{Symbol_AttributeValue}/{RegexPredicate.Symbol_RegularExpression}";

        private readonly ImmutableArray<(ITokenPredicate keyPredicate, ITokenPredicate? valuePredicate)> predicates;

        public ImmutableArray<(ITokenPredicate KeyPredicate, ITokenPredicate? ValuePredicate)> Predicates => predicates
            .Select(p => (p.keyPredicate, p.valuePredicate))
            .ToImmutableArray();

        public int PredicateCount => predicates.Length;

        public bool IsNegated { get; }

        public AttributeMatcher(
            bool isNegated,
            params (ITokenPredicate keyPredicate, ITokenPredicate? valuePredicate)[] attributePredicates)
        {
            ArgumentNullException.ThrowIfNull(attributePredicates);

            IsNegated = isNegated;
            predicates = attributePredicates
                .ThrowIf(
                    p => p.IsEmpty(),
                    _ => new ArgumentException($"Invalid predicates: empty"))
                .ThrowIfAny(
                    predicate => predicate.keyPredicate is null,
                    _ => new InvalidOperationException($"Invalid predicate: keyPredicate is null"))
                .ToImmutableArray();
        }

        public AttributeMatcher(
            params (ITokenPredicate keyPredicate, ITokenPredicate? valuePredicate)[] attributePredicates)
            : this(false, attributePredicates)
        { }

        public bool IsMatch(IAttributeContainer container, out ImmutableHashSet<Core.Types.Attribute> matchInfo)
        {
            ArgumentNullException.ThrowIfNull(container);

            var orderedAttributes = container.Attributes
                .OrderBy(a => a.Key)
                .ThenBy(a => a.Value)
                .ToArray();

            matchInfo = orderedAttributes
                .Where(att =>
                {
                    var isMatch = predicates.Any(predicate =>
                        predicate.keyPredicate.IsMatch(att.Key, out _)
                        && (predicate.valuePredicate?.IsMatch(att.Value ?? "", out _) ?? true));
                    return IsNegated ? !isMatch : isMatch;
                })
                .ToImmutableHashSet();

            return !matchInfo.IsEmpty;
        }


        public static AttributeMatcher Parse(string text, bool isNegated = false)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_AttributeMatcher];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, AttributeMatcher>(v => Parse(v, isNegated));

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static AttributeMatcher Parse(ISymbolNode node, bool isNegated = false)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !Symbol_AttributeMatcher.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(AttributeExpressionPath)
                .Select(node =>
                {
                    var key = node
                        .FindNodes(AttributeKeyPath)
                        .Select(ITokenPredicate.Parse)
                        .First();

                    var value = node
                        .FindNodes(AttributeValuePath)
                        .Select(RegexPredicate.Parse)
                        .Select(predicate => predicate.As<ITokenPredicate>())
                        .FirstOrDefault();

                    return (KeyPredicte: key, ValuePredicte: value);
                })
                .ApplyTo(predicates => new AttributeMatcher(isNegated, predicates.ToArray()));
        }
    }
}
