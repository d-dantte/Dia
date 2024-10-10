using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Deserializers
{
    public class AttributeDeserializer : IDeserializer<Core.Types.Attribute[]>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_Attribute}/{Symbol_AttributeFlag}|{Symbol_AttributeKVP}";

        #region Symbols
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Attribute = "attribute";
        private const string Symbol_AttributeFlag = "attribute-flag";
        private const string Symbol_AttributeKVP = "attribute-kvp";
        private const string Symbol_AttributeName = "attribute-name";
        private const string Symbol_AttributeValue = "attribute-value";
        #endregion

        public static Core.Types.Attribute[] Deserialize(
            string text,
            DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_AttributeList] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_AttributeList}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid attribute format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Core.Types.Attribute[]>(Deserialize);
        }

        internal static Core.Types.Attribute[] Deserialize(ISymbolNode attributeListNode)
        {
            return attributeListNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(attributeListNode)))
                .ThrowIf(
                    node => !Symbol_AttributeList.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"))
                .FindNodes(Query)
                .Select(n => n.Symbol switch
                {
                    Symbol_AttributeFlag => Core.Types.Attribute.Of(n.Tokens[1..^1]),
                    _ => Core.Types.Attribute.Of(
                        n.FindNodes(Symbol_AttributeName).First().Tokens[1..],
                        n.FindNodes(Symbol_AttributeValue).First().Tokens[1..^1])
                })
                .ToArray();
        }
    }
}
