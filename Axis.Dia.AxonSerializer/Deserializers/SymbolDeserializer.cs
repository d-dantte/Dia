using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Common.StringEscape;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using System.Text;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class SymbolDeserializer : IValueDeserializer<Symbol>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_SymbolContent}/{Symbol_Segment1}|{Symbol_Segment2}";

        private static readonly CommonStringEscaper Escaper = new();

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Symbol = "dia-symbol";
        private const string Symbol_Null = "null-symbol";
        private const string Symbol_SymbolContent = "symbol-content";
        private const string Symbol_Segment1 = "symbol-content-segment1";
        private const string Symbol_Segment2 = "symbol-content-segment2";
        #endregion

        public static Symbol Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Symbol] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Symbol}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid symbol format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Symbol>(Deserialize);
        }

        internal static Symbol Deserialize(ISymbolNode symbolNode)
        {
            symbolNode = symbolNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(symbolNode)))
                .ThrowIf(
                    node => !Symbol_Symbol.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = symbolNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (symbolNode.TryFindNodes(Symbol_Null, out var nodes))
                return Symbol.Null(attributes!);

            return symbolNode
                .FindNodes(Query)
                .Select(n => n.Symbol switch
                {
                    Symbol_Segment1 => n.As<INodeContainer>().Nodes[2].Tokens[1..^1],
                    _ => n.Tokens[1..^1]
                })
                .Aggregate(new StringBuilder(), (sb, token) => sb.Append(token))
                .ApplyTo(sbuilder => Escaper.UnescapeString(sbuilder.ToString()))
                .ApplyTo(str => Symbol.Of(str, attributes?.ToArray() ?? []));
        }
    }
}
