using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.PathQuery.Grammar;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using System.Collections.Immutable;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.PathQuery.Matchers
{
    public class TypeMatcher : IMatcher<IDiaValue, IDiaValue?>
    {
        internal const string Symbol_TypeMatcher = "type-matcher";
        internal const string Symbol_BoolType = "bool-type";
        internal const string Symbol_BLobType = "blob-type";
        internal const string Symbol_DecimalType = "decimal-type";
        internal const string Symbol_DurationType = "duration-type";
        internal const string Symbol_IntegerType = "integer-type";
        internal const string Symbol_RecordType = "record-type";
        internal const string Symbol_SequenceType = "sequence-type";
        internal const string Symbol_StringType = "string-type";
        internal const string Symbol_SymbolType = "symbol-type";
        internal const string Symbol_TimestampType = "timestamp-type";

        public ImmutableHashSet<DiaType> Types { get; }

        public bool IsNegated { get; }

        public TypeMatcher(bool isNegated, params DiaType[] types)
        {
            ArgumentNullException.ThrowIfNull(types);

            IsNegated = isNegated;
            Types = types
                .ThrowIf(
                    tarr => tarr.IsEmpty(),
                    _ => new ArgumentException($"Invalid type array: empty"))
                .ThrowIfAny(
                    type => !Enum.IsDefined(type),
                    type => new InvalidOperationException($"Invalid type: '{type}'"))
                .ToImmutableHashSet();
        }

        public TypeMatcher(
            params DiaType[] types)
            : this(false, types)
        { }

        public bool IsMatch(IDiaValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var isMatch = Types.Contains(value.Type);

            return IsNegated ? !isMatch : isMatch;
        }

        public bool IsMatch(IDiaValue value, out IDiaValue? outValue)
        {
            outValue = IsMatch(value) ? value : null;

            return outValue is not null;
        }

        public static TypeMatcher Parse(string text, bool isNegated = false)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_TypeMatcher];

            if (recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
                return result.Map<ISymbolNode, TypeMatcher>(v => Parse(v, isNegated));

            else throw new FormatException($"Invalid format: {text}");
        }

        internal static TypeMatcher Parse(ISymbolNode node, bool isNegated = false)
        {
            return node
                .ThrowIfNull(() => new ArgumentNullException(nameof(node)))
                .ThrowIf(
                    n => !Symbol_TypeMatcher.Equals(n.Symbol),
                    n => new InvalidOperationException($"Invalid symbol-node: {n.Symbol}"))
                .As<INodeContainer>().Nodes
                .Select(node => node.Symbol switch
                {
                    Symbol_BoolType => DiaType.Bool,
                    Symbol_BLobType => DiaType.Blob,
                    Symbol_DecimalType => DiaType.Decimal,
                    Symbol_DurationType => DiaType.Duration,
                    Symbol_IntegerType => DiaType.Int,
                    Symbol_RecordType => DiaType.Record,
                    Symbol_SequenceType => DiaType.Sequence,
                    Symbol_StringType => DiaType.String,
                    Symbol_SymbolType => DiaType.Symbol,

                    // Symbol_TimestampType
                    _ => DiaType.Timestamp
                })
                .ApplyTo(types => new TypeMatcher(isNegated, types.ToArray()));
        }
    }
}
