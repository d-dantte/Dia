using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class ValueDeserializer : IValueDeserializer<IDiaValue>
    {
        public static readonly string ExceptionData = "RecognizerError";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Value = "dia-value";
        private const string Symbol_Bool = "dia-bool";
        private const string Symbol_Decimal = "dia-decimal";
        private const string Symbol_Int = "dia-int";
        private const string Symbol_Duration = "dia-duration";
        private const string Symbol_Timestamp = "dia-timestamp";
        private const string Symbol_String = "dia-string";
        private const string Symbol_Symbol = "dia-symbol";
        private const string Symbol_Blob = "dia-blob";
        private const string Symbol_Ref = "dia-ref";
        private const string Symbol_Sequence = "dia-sequence";
        private const string Symbol_Record = "dia-record";
        #endregion

        public static IDiaValue Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Value] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Value}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid dia-value format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, IDiaValue>(node => Deserialize(node, context));
        }

        internal static IDiaValue Deserialize(ISymbolNode valueNode, DeserializerContext? context)
        {
            valueNode = valueNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(valueNode)))
                .ThrowIf(
                    node => !Symbol_Value.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            return valueNode
                .As<INodeContainer>().Nodes
                .First()
                .ApplyTo(n => n.Symbol switch
                {
                    Symbol_Bool => BooleanDeserializer.Deserialize(n),
                    Symbol_Decimal => DecimalDeserializer.Deserialize(n),
                    Symbol_Int => IntegerDeserializer.Deserialize(n),
                    Symbol_Duration => DurationDeserializer.Deserialize(n),
                    Symbol_Timestamp => TimestampDeserializer.Deserialize(n),
                    Symbol_String => StringDeserializer.Deserialize(n),
                    Symbol_Symbol => SymbolDeserializer.Deserialize(n),
                    Symbol_Blob => BlobDeserializer.Deserialize(n),
                    Symbol_Sequence => SequenceDeserializer.Deserialize(n, context),
                    Symbol_Record => RecordDeserializer.Deserialize(n, context),

                    //Symbol_Ref
                    _ => RefDeserializer.Deserialize(n),
                });
        }
    }
}
