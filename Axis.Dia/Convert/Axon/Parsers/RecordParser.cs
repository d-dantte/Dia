using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class RecordParser : IValueSerializer<RecordValue>
    {
        #region Symbols
        public const string SymbolNameDiaRecord = "dia-record";
        public const string SymbolNameNullRecord = "null-record";
        public const string SymbolNameRecordValue = "record-value";
        public const string SymbolNameRecordField = "record-field";
        public const string SymbolNameFieldName = "field-name";
        public const string SymbolNameDiaValue = "dia-value";
        public const string SymbolNameSingleLineString = "singleline-string";
        public const string SymbolNameDiaSymbol = "dia-symbol";
        #endregion


        private RecordParser() { }

        public static string GrammarSymbol => SymbolNameDiaRecord;

        public static IResult<RecordValue> Parse(CSTNode symbolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            context.ThrowIfDefault($"Invalid {nameof(context)} instance");

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (AddressIndexNode, AnnotationNode, ValueNode) = symbolNode.DeconstructValueNode();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                var result = ValueNode.SymbolName switch
                {
                    SymbolNameNullRecord => annotationResult.Map(RecordValue.Null),
                    SymbolNameRecordValue => ValueNode
                        .FindNodes(SymbolNameRecordField)
                        .Select(node => ParseField(node, context))
                        .Fold()
                        .Combine(
                            annotationResult,
                            (fields, annotations) => RecordValue.Of(annotations, fields.ToArray())),

                    _ => Result.Of<RecordValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullRecord}', or '{SymbolNameRecordValue}'"))
                };

                return AddressIndexNode is not null
                    ? result.Combine(
                        AddressIndexParser.Parse(AddressIndexNode),
                        (value, addressIndex) => value.RelocateValue(context.Track(addressIndex)))
                    : result;
            }
            catch (Exception e)
            {
                return Result.Of<RecordValue>(e);
            }
        }


        public static IResult<string> Serialize(RecordValue value, SerializerContext context)
        {
            context.ThrowIfDefault($"Invalid {nameof(context)} instance");

            var addressIndexText = context.TryGetAddressIndex(value, out var index)
                ? $"#0x{index:x}"
                : "";

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);

            var lineJoiner = context.Options.Records.UseMultipleLines
                ? $"{Environment.NewLine}"
                : "";

            var itemIndentation = context.Options.Records.UseMultipleLines
                ? context.IndentText("", 1)
                : " ";

            var closingBracketIndentation = context.Options.Records.UseMultipleLines
                ? context.IndentText("")
                : " ";

            var valueText = value.IsNull switch
            {
                true => Result.Of("null.record"),
                false => value.Value!
                    .Select(prop => SerializeProperty(prop, context.Indent()))
                    .Select(propertyText => propertyText.Map(v => $"{lineJoiner}{itemIndentation}{v}"))
                    .FoldInto(items => items.JoinUsing($","))
                    .Map(recordText => $"{{{recordText}{lineJoiner}{closingBracketIndentation}}}")
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{addressIndexText}{ann}{value}");
        }

        internal static IResult<string> SerializeProperty(
            KeyValuePair<SymbolValue, IDiaValue> property,
            SerializerContext indentedContext)
        {
            var quoteName = indentedContext.Options.Records.UseQuotedIdentifierPropertyNames;

            if (property.Key.IsNull)
                throw new InvalidOperationException("Property name cannot be a null symbol");

            var name = AxonSerializer
                .SerializeValue(property.Key)
                .Map(name =>
                {
                    if (quoteName)
                        return property.Key.HasAnnotations()
                            ? name.WrapIn("\"@", "\"")
                            : name.UnwrapFrom("'").WrapIn("\"");
                    
                    return name;
                });

            var value = AxonSerializer.InternalSerializeValue(property.Value, indentedContext);

            return name.Combine(value, (pname, pvalue) => $"{pname}: {pvalue}");
        }

        internal static IResult<KeyValuePair<SymbolValue, IDiaValue>> ParseField(
            CSTNode recordFieldNode,
            ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(recordFieldNode);

            var name = recordFieldNode
                .FindNodes(SymbolNameFieldName)
                .FirstOrOptional<CSTNode>()
                .AsResult()
                .Bind(nameNode => ParseFieldName(nameNode, context));

            var value = recordFieldNode
                .FindNodes(SymbolNameDiaValue)
                .FirstOrOptional()
                .AsResult()
                .Bind(valueNode => AxonSerializer.ParseValue(valueNode.FirstNode(), context));

            return name.Combine(value, KeyValuePair.Create<SymbolValue, IDiaValue>);
        }

        private static IResult<SymbolValue> ParseFieldName(CSTNode nameNode, ParserContext context)
        {
            var node = nameNode
                .FindNodes($"{SymbolNameDiaSymbol}|{SymbolNameSingleLineString}")
                .FirstOrThrow(new FormatException(
                    $"Could not find expected symbols: "
                    + $"'{SymbolNameSingleLineString}', or '{SymbolNameDiaSymbol}'"));

            return node.SymbolName switch
            {
                SymbolNameSingleLineString => node
                    .TokenValue()
                    .UnwrapFrom("\"")
                    .ApplyTo(SymbolValue.Of)
                    .ApplyTo(Result.Of),

                SymbolNameDiaSymbol => AxonSerializer
                    .ParseValue(node, context)
                    .Map(diaValue => (SymbolValue)diaValue),

                _ => Result.Of<SymbolValue>(new FormatException(
                    $"Invalid symbol: '{node.SymbolName}'. "
                    + $"Expected '{SymbolNameSingleLineString}', or '{SymbolNameDiaSymbol}'"))
            };
        }
    }

    
}
