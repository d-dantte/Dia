using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Grammar;
using Axis.Dia.PathQuery.Predicates;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using System.Collections.Immutable;

namespace Axis.Dia.PathQuery.Matchers
{
    internal class PropertyNameMatcher : IMatcher<Record, ImmutableArray<Record.PropertyName>>
    {
        internal const string Symbol_PropertyNameMatcher = "property-name-matcher";

        internal static readonly NodePath PropertyPath = $"{ITokenPredicate.Symbol_TokenPredicate}|{AttributeMatcher.Symbol_AttributeMatcher}";

        public ITokenPredicate NameMatcher { get; }

        public AttributeMatcher? AttributeMatcher { get; }

        public bool IsNegated { get; }

        public PropertyNameMatcher(
            bool isNegated,
            ITokenPredicate nameMatcher,
            AttributeMatcher? attributeMatcher = null)
        {
            ArgumentNullException.ThrowIfNull(nameMatcher);

            NameMatcher = nameMatcher;
            AttributeMatcher = attributeMatcher;
            IsNegated = isNegated;
        }

        public PropertyNameMatcher(
            ITokenPredicate nameMatcher,
            AttributeMatcher? attributeMatcher = null)
            : this(false, nameMatcher, attributeMatcher)
        { }

        public bool IsMatch(Record.PropertyName propertyName)
        {
            var isMatch = NameMatcher.IsMatch(propertyName.Name, out _)
                && (AttributeMatcher?.IsMatch(propertyName, out _) ?? true);

            return IsNegated ? !isMatch : isMatch;
        }

        public bool IsMatch(Record record, out ImmutableArray<Record.PropertyName> matches)
        {
            matches = record.Value!
                .AsOptional()
                .Map(props => props
                    .Select(prop => prop.Name)
                    .Where(IsMatch)
                    .ToImmutableArray())
                .ValueOr(ImmutableArray.Create<Record.PropertyName>());

            return !matches.IsEmpty;
        }

        public static PropertyNameMatcher Parse(string text, bool isNegated = false)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_PropertyNameMatcher];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, PropertyNameMatcher>(v => Parse(v, isNegated));

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static PropertyNameMatcher Parse(ISymbolNode node, bool isNegated = false)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !Symbol_PropertyNameMatcher.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(PropertyPath)
                .Aggregate((Name: default(ITokenPredicate), Att: default(AttributeMatcher)), (info, node) => node.Symbol switch
                {
                    ITokenPredicate.Symbol_TokenPredicate => info with { Name = ITokenPredicate.Parse(node) },

                    //AttributeMatcher.Symbol_AttributeFilter
                    _ => info with { Att = AttributeMatcher.Parse(node) }
                })
                .ApplyTo(info => new PropertyNameMatcher(isNegated, info.Name!, info.Att));
        }
    }
}
