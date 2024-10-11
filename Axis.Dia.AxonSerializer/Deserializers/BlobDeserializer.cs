using Axis.Dia.Axon;
using Axis.Dia.Axon.Lang;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using System.Text;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.Axon.Deserializers
{
    public class BlobDeserializer : IValueDeserializer<Blob>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_BlobContent}/{Symbol_Segment1}|{Symbol_Segment2}";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Blob = "dia-blob";
        private const string Symbol_Null = "null-blob";
        private const string Symbol_BlobContent = "blob-content";
        private const string Symbol_Segment1 = "blob-content-segment1";
        private const string Symbol_Segment2 = "blob-content-segment2";
        #endregion

        public static Blob Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Blob] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Blob}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid blob format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Blob>(Deserialize);
        }

        internal static Blob Deserialize(ISymbolNode blobNode)
        {
            blobNode = blobNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(blobNode)))
                .ThrowIf(
                    node => !Symbol_Blob.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = blobNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (blobNode.TryFindNodes(Symbol_Null, out var nodes))
                return Blob.Null(attributes!);

            return blobNode
                .FindNodes(Query)
                .Select(n => n.Symbol switch
                {
                    Symbol_Segment1 => n.As<INodeContainer>().Nodes[2].Tokens[1..^1],
                    _ => n.Tokens[1..^1]
                })
                .Aggregate(new StringBuilder(), (sb, token) => sb.Append(token))
                .ApplyTo(sb => Convert.FromBase64String(sb.ToString()))
                .ApplyTo(bytes => Blob.Of(bytes, attributes?.ToArray() ?? []));
        }
    }
}
