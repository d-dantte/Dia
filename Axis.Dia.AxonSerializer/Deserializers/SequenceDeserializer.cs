using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class SequenceDeserializer : IValueDeserializer<Sequence>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_Value}/{Symbol_DiaValue}";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Seq = "dia-sequence";
        private const string Symbol_Hash = "dia-hash";
        private const string Symbol_Null = "null-sequence";
        private const string Symbol_Value = "sequence-value";
        private const string Symbol_DiaValue = "dia-value";
        #endregion

        public static Sequence Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Seq] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Seq}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid sequence format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Sequence>(node => Deserialize(node, context));
        }

        internal static Sequence Deserialize(ISymbolNode seqNode, DeserializerContext? context)
        {
            ArgumentNullException.ThrowIfNull(context);

            seqNode = seqNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(seqNode)))
                .ThrowIf(
                    node => !Symbol_Seq.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var hash = seqNode
                .FindNodes(Symbol_Hash)
                .Select(n => n.As<INodeContainer>())
                .Select(c => c.Nodes[1].Tokens.ToString()!)
                .FirstOrOptional();

            var attributes = seqNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (seqNode.TryFindNodes(Symbol_Null, out var nodes))
                return Sequence.Null(attributes!);

            // Create an empty sequence
            var seq = Sequence.Of(attributes);

            // add the sequence to the Reference map
            hash.Consume(axonHash => int
                .Parse(axonHash, System.Globalization.NumberStyles.HexNumber)
                .ApplyTo(axonHash => context.ReferenceMap.TryAddRef(seq, axonHash))
                .ThrowIf(false, _ => new InvalidOperationException(
                    $"Invalid axon-hash: the given hash '{axonHash}' is already mapped.")));

            // add items to the sequence
            seqNode
                .FindNodes(Query)
                .Select(node => ValueDeserializer.Deserialize(node, context))
                .ForEvery((index, value) =>
                {
                    seq.AddItem(value);

                    if (RefDeserializer.IsTypeRef(value, out var axonHash))
                        context.TryAddValueResolver(axonHash, () =>
                        {
                            if (!context.ReferenceMap.TryGetRef(axonHash, out var resolvedValue))
                                throw new InvalidOperationException();

                            seq.Set((int)index, resolvedValue!);
                        });
                });

            return seq;
        }
    }
}
