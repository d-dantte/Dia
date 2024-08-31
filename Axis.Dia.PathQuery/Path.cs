using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Accessors;
using Axis.Dia.PathQuery.Grammar;
using Axis.Dia.PathQuery.Matchers;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using System.Collections.Immutable;

namespace Axis.Dia.PathQuery
{
    public class Path :
        IFilter,
        IModify,
        IDelete
    {
        internal const string Symbol_Path = "path";

        private static readonly NodePath SegmentPath = Segment.Symbol_Segment;

        public ImmutableArray<Segment> Segments { get; }

        public Path(params Segment[] segments)
        {
            Segments = segments
                .ThrowIfNull(() => new ArgumentNullException(nameof(segments)))
                .ThrowIfAny(
                    segment => segment is null,
                    _ => new InvalidOperationException($"Invalid segment: null"))
                .ToImmutableArray();
        }

        #region IFilter
        public IEnumerable<IDiaValue> FilterValues(IDiaValue root)
        {
            ArgumentNullException.ThrowIfNull(root);

            return Segments.Aggregate(root.Enumerate(), (results, nextSegment) =>
            {
                var empty = Array.Empty<IDiaValue>();
                return results.SelectMany(value => (nextSegment.Matcher, value) switch
                {
                    (TypeMatcher tm, _) => tm.IsMatch(value, out _)
                    ? new[] { value }
                    : empty,

                    (AttributeMatcher am, IAttributeContainer ac) => am.IsMatch(ac, out _)
                        ? new[] { value }
                        : empty,

                    (IndexRangeMatcher ir, Sequence seq) => ir.IsMatch(seq, out var ranges)
                        ? GetIndexes(ranges).Select(index => seq[index].Payload)
                        : empty,

                    (PropertyNameMatcher pn, Record rec) => pn.IsMatch(rec, out var props)
                        ? props.Select(propName => rec[propName].Payload)
                        : empty,

                    _ => empty
                });
            });
        }
        #endregion

        #region IDelete
        public IEnumerable<IDiaValue> DeleteValues(IDiaValue root)
        {
            ArgumentNullException.ThrowIfNull(root);

            var tail = Seek(root).ToArray();

            // process index accessors
            var indexValues = tail
                .Where(info => info.Accessor is IndexAccessor)
                .Select(info => (info.Value, Accessor: info.Accessor.As<IndexAccessor>()))
                .OrderByDescending(info => info.Accessor.Key) // ordered because removing from a list messes with indices
                .Where(info => info.Accessor.Source.TryRemoveAt(info.Accessor.Key, out _));

            // process property accessors
            var propertyValues = tail
                .Where(info => info.Accessor is PropertyAccessor)
                .Select(info => (info.Value, Accessor: info.Accessor.As<PropertyAccessor>()))
                .Where(info => info.Accessor.Source.TryRemove(info.Accessor.Key, out _));

            // return the removed values
            return indexValues
                .Select(info => info.Value)
                .Concat(propertyValues.Select(info => info.Value));
        }
        #endregion

        #region IModify
        public IEnumerable<IDiaValue> ReplaceValues(
            IDiaValue root,
            IModify.ConditionalValueProvider tryProvideValue)
        {
            ArgumentNullException.ThrowIfNull(root);
            ArgumentNullException.ThrowIfNull(tryProvideValue);

            var tail = Seek(root).ToArray();

            // process index accessors
            var indexValues = tail
                .Where(info => info.Accessor is IndexAccessor)
                .Select(info => (info.Value, Accessor: info.Accessor.As<IndexAccessor>()))
                .Where(info =>
                {
                    var provided = tryProvideValue((info.Accessor.Key, info.Value), out var newValue);
                    if (provided)
                        info.Accessor.Source.Set(info.Accessor.Key, newValue!);

                    return provided;
                });

            // process property accessors
            var propertyValues = tail.Where(info => info.Accessor is PropertyAccessor)
                .Select(info => (info.Value, Accessor: info.Accessor.As<PropertyAccessor>()))
                .Where(info =>
                {
                    var provided = tryProvideValue((info.Accessor.Key, info.Value), out var newValue);
                    if (provided)
                        info.Accessor.Source.SetProperty(info.Accessor.Key, ContainerValue.Of(newValue!));

                    return provided;
                });

            // return the removed values
            return indexValues
                .Select(info => info.Value)
                .Concat(propertyValues.Select(info => info.Value));
        }
        #endregion

        #region Parse
        public static Path Parse(string text)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Path];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, Path>(Parse);

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static Path Parse(ISymbolNode tokenPredicateNode)
        {
            return tokenPredicateNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(tokenPredicateNode)))
                .ThrowIf(
                    n => !Symbol_Path.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .FindNodes(SegmentPath)
                .Select(Segment.Parse)
                .ApplyTo(segments => new Path(segments.ToArray()));
        }
        #endregion

        private static IEnumerable<int> GetIndexes(ImmutableArray<Range> ranges)
        {
            return ranges.SelectMany(range => range.Enumerate());
        }

        internal IEnumerable<(IDiaValue Value, IAccessor? Accessor)> Seek(IDiaValue root)
        {
            ArgumentNullException.ThrowIfNull(root);

            var rootInfo = (Value: root, Accessor: default(IAccessor));

            return Segments.Aggregate(rootInfo.Enumerate(), (results, nextSegment) =>
            {
                var empty = Enumerable.Empty<(IDiaValue, IAccessor?)>();
                return results.SelectMany(info => (nextSegment.Matcher, info.Value) switch
                {
                    (TypeMatcher tm, _) => tm.IsMatch(info.Value, out _)
                    ? new[] { info }
                    : empty,

                    (AttributeMatcher am, IAttributeContainer ac) => am.IsMatch(ac, out _)
                        ? new[] { info }
                        : empty,

                    (IndexRangeMatcher ir, Sequence seq) => ir.IsMatch(seq, out var ranges)
                        ? GetIndexes(ranges).Select(index => (
                            Value: seq[index].Payload,
                            Accessor: (IAccessor?)new IndexAccessor(seq, index)))
                        : empty,

                    (PropertyNameMatcher pn, Record rec) => pn.IsMatch(rec, out var props)
                        ? props.Select(propName => (
                            Value: rec[propName].Payload,
                            Accessor: (IAccessor?)new PropertyAccessor(rec, propName)))
                        : empty,

                    _ => empty
                });
            });
        }
    }
}
