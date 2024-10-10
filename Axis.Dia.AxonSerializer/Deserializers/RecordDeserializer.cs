using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class RecordDeserializer : IValueDeserializer<Record>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_Value}/{Symbol_Field}";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Rec = "dia-record";
        private const string Symbol_Hash = "dia-hash";
        private const string Symbol_Null = "null-record";
        private const string Symbol_Value = "record-value";
        private const string Symbol_Field = "record-field";
        private const string Symbol_FieldName = "field-name";
        private const string Symbol_DiaValue = "dia-value";
        private const string Symbol_Identifier = "attribute-identifier";
        private const string Symbol_SLString = "singleline-string";
        #endregion

        public static Record Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Rec] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Rec}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid record format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Record>(node => Deserialize(node, context));
        }

        internal static Record Deserialize(ISymbolNode recNode, DeserializerContext? context)
        {
            ArgumentNullException.ThrowIfNull(context);

            recNode = recNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(recNode)))
                .ThrowIf(
                    node => !Symbol_Rec.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var hash = recNode
                .FindNodes(Symbol_Hash)
                .Select(n => n.As<INodeContainer>())
                .Select(c => c.Nodes[1].Tokens.ToString()!)
                .FirstOrOptional();

            var attributes = recNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (recNode.TryFindNodes(Symbol_Null, out var nodes))
                return Record.Null(attributes!);

            // Create an empty record
            var record = Record.Of(attributes);

            // add the record to the Reference map
            hash.Consume(axonHash => int
                .Parse(axonHash, System.Globalization.NumberStyles.HexNumber)
                .ApplyTo(axonHash => context.ReferenceMap.TryAddRef(record, axonHash))
                .ThrowIf(false, _ => new InvalidOperationException(
                    $"Invalid axon-hash: the given hash '{axonHash}' is already mapped.")));

            recNode
                .FindNodes(Query)
                .Select(fieldNode =>
                {
                    return Record.Property.Of(
                        DeserializePropertyName(fieldNode.FindNodes(Symbol_FieldName).First()),
                        ValueDeserializer
                            .Deserialize(fieldNode.FindNodes(Symbol_DiaValue).First(), context)
                            .ApplyTo(DiaValue.Of));
                })
                .ForEvery(prop =>
                {
                    record[prop.Name] = prop.Value;

                    if (RefDeserializer.IsTypeRef(prop.Value.AsDiaValue(), out var axonHash))
                        context.TryAddValueResolver(axonHash, () =>
                        {
                            if (!context.ReferenceMap.TryGetRef(axonHash, out var resolvedValue))
                                throw new InvalidOperationException();

                            record[prop.Name] = DiaValue.Of(resolvedValue!);
                        });
                });

            return record;
        }

        internal static Record.PropertyName DeserializePropertyName(ISymbolNode fieldNameNode)
        {
            fieldNameNode = fieldNameNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(fieldNameNode)))
                .ThrowIf(
                    node => !Symbol_FieldName.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = fieldNameNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            return fieldNameNode
                .FindNodes($"{Symbol_Identifier}|{Symbol_SLString}")
                .Select(n => n.Symbol switch
                {
                    Symbol_Identifier => Record.PropertyName.Of(n.Tokens),

                    // singleline string
                    _ => Record.PropertyName.Of(n.Tokens[1..^1]),
                })
                .First();
        }
    }
}
