using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Text.Parsers
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
            ArgumentNullException.ThrowIfNull(context);

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (AnnotationNode, ValueNode) = symbolNode.DeconstructValue();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                return ValueNode.SymbolName switch
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
            }
            catch (Exception e)
            {
                return Result.Of<RecordValue>(e);
            }
        }


        public static IResult<string> Serialize(RecordValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var indentedContext = context.IndentContext();
            var indentationText = indentedContext.Indentation();
            var originalIndentationText = context.Indentation();

            (var ldelimiter, var valueSeparator, var rdelimiter) = context.Options.Records.UseMultipleLines switch
            {
                false => ("{", ", ", "}"),
                true => (
                    $"{{{Environment.NewLine}{indentationText}",
                    $",{Environment.NewLine}{indentationText}",
                    $"{Environment.NewLine}{originalIndentationText}}}")
            };

            var valueText = value.IsNull switch
            {
                true => Result.Of("null.record"),
                false => value.Value!
                    .Select(prop => SerializeProperty(prop, indentedContext))
                    .Fold()
                    .Map(items => items
                        .JoinUsing(valueSeparator)
                        .WrapIn(ldelimiter, rdelimiter))
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{ann}{value}");
        }

        internal static IResult<string> SerializeProperty(
            KeyValuePair<SymbolValue, IDiaValue> property,
            SerializerContext indentedContext)
        {
            var quoteName = indentedContext.Options.Records.UseQuotedIdentifierPropertyNames;

            if (property.Key.IsNull)
                throw new InvalidOperationException("Property name cannot be a null symbol");

            var name = TextSerializer
                .SerializeValue(property.Key)
                .Map(name =>
                {
                    if (quoteName)
                        return property.Key.HasAnnotations()
                            ? name.WrapIn("\"@", "\"")
                            : name.UnwrapFrom("'").WrapIn("\"");
                    
                    return name;
                });

            var value = TextSerializer.InternalSerializeValue(property.Value, indentedContext);

            return name.Combine(value, (pname, pvalue) => $"{pname}:{pvalue}");
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
                .Bind(valueNode => TextSerializer.ParseValue(valueNode.FirstNode(), context));

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

                SymbolNameDiaSymbol => TextSerializer
                    .ParseValue(node, context)
                    .Map(diaValue => (SymbolValue)diaValue),

                _ => Result.Of<SymbolValue>(new FormatException(
                    $"Invalid symbol: '{node.SymbolName}'. "
                    + $"Expected '{SymbolNameSingleLineString}', or '{SymbolNameDiaSymbol}'"))
            };
        }
    }

    
}
