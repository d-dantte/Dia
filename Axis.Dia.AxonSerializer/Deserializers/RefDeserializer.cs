using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using static Axis.Pulsar.Core.CST.ISymbolNode;
using Axis.Dia.Core.Types;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class RefDeserializer : IValueDeserializer<IDiaValue>
    {
        public static readonly string ExceptionData = "RecognizerError";
        public static readonly string RefMarkerAttribute = "Axon.Ref";

        #region Symbol
        private const string Symbol_Ref = "dia-ref";
        private const string Symbol_TypeQualifier = "type-qualifier";
        #endregion

        public static IDiaValue Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Ref] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Ref}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid ref format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, IDiaValue>(Deserialize);
        }

        internal static IDiaValue Deserialize(ISymbolNode refNode)
        {
            refNode = refNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(refNode)))
                .ThrowIf(
                    node => !Symbol_Ref.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var hash = refNode
                .As<INodeContainer>()
                .Nodes[5].Tokens
                .ToString();

            var refMarker = Core.Types.Attribute.Of(RefMarkerAttribute, hash);

            return refNode
                .FindNodes($"{Symbol_TypeQualifier}/@a")
                .Select(n => n.Tokens.ToString()!.ToLower() switch
                {
                    "boolean" => (IDiaValue)Core.Types.Boolean.Null(refMarker),
                    "decimal" => Core.Types.Decimal.Null(refMarker),
                    "integer" => Integer.Null(refMarker),
                    "duration" => Duration.Null(refMarker),
                    "timestamp" => Timestamp.Null(refMarker),
                    "string" => Core.Types.String.Null(refMarker),
                    "symbol" => Symbol.Null(refMarker),
                    "blob" => Blob.Null(refMarker),
                    "sequence" => Sequence.Null(refMarker),
                    //"record"
                    _ => Record.Null(refMarker),
                })
                .First();
        }

        internal static bool IsTypeRef(IDiaValue value, out int axonHash)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value.As<INullableContract>().IsNull
                && value.As<IAttributeContainer>().Attributes
                    .TryGetAttribute(RefMarkerAttribute, out var att))
            {
                axonHash = int.Parse(att!.Value.Value!, System.Globalization.NumberStyles.HexNumber);
                return true;
            }
            else
            {
                axonHash = 0;
                return false;
            }
        }
    }
}
