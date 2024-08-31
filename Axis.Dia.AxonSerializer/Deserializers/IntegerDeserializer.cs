using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Types;
using Axis.Luna.BitSequence;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Luna.Result;
using Axis.Pulsar.Core.CST;
using System.Numerics;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class IntegerDeserializer : IValueDeserializer<Integer>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_IntNumber}/{Symbol_IntNotation}/{Symbol_BinaryInt}|{Symbol_DecimalInt}|{Symbol_HexInt}";
        private readonly static string SignQuery = $"{Symbol_IntNumber}/<->";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Int = "dia-int";
        private const string Symbol_Null = "null-int";
        private const string Symbol_IntNumber = "int-number";
        private const string Symbol_IntNotation = "int-notation";
        private const string Symbol_BinaryInt = "binary-int";
        private const string Symbol_DecimalInt = "regular-int";
        private const string Symbol_HexInt = "hex-int";
        #endregion

        public static Integer Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Int] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Int}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid integer format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Integer>(Deserialize);
        }

        internal static Integer Deserialize(ISymbolNode intNode)
        {
            intNode = intNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(intNode)))
                .ThrowIf(
                    node => !Symbol_Int.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = intNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (intNode.TryFindNodes(Symbol_Null, out var nodes))
                return Integer.Null(attributes!);

            var isNegative = intNode.TryFindNodes(SignQuery, out _);

            return intNode
                .FindNodes(Query)
                .Select(n => n.Symbol switch
                {
                    Symbol_BinaryInt => n.Tokens[2..]
                        .ToString()!
                        .Replace("_", "")
                        .ApplyTo(BitSequence.Parse)
                        .Map(bs => bs.ToByteArray())
                        .Map(barr => new BigInteger(barr))
                        .Resolve(),

                    Symbol_DecimalInt => n.Tokens
                        .ToString()!
                        .Replace("_", "")
                        .ApplyTo(BigInteger.Parse),

                    //Symbol_HexInt
                    // see here: https://stackoverflow.com/a/30119254
                    // and here: https://en.wikipedia.org/wiki/Signed_number_representations
                    // and here: https://en.wikipedia.org/wiki/Two's_complement
                    _ => n.Tokens[2..]
                        .ToString()!
                        .Replace("_", "")
                        .ApplyTo(text => BigInteger.Parse($"0{text}", System.Globalization.NumberStyles.HexNumber)),
                })
                .First()
                .ApplyTo(bi => isNegative ? BigInteger.Negate(bi) : bi)
                .ApplyTo(bi => Integer.Of(bi, attributes));
        }
    }
}
