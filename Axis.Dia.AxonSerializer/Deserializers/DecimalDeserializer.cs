using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using Axis.Luna.Numerics;
using Axis.Luna.Result;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class DecimalDeserializer : IValueDeserializer<Core.Types.Decimal>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_RegularDecimall}|{Symbol_ScientificDecimal}";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Decimal = "dia-decimal";
        private const string Symbol_Null = "null-decimal";
        private const string Symbol_RegularDecimall = "regular-decimal";
        private const string Symbol_ScientificDecimal = "scientific-decimal";
        #endregion

        public static Core.Types.Decimal Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Decimal] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Decimal}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid decimal format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Core.Types.Decimal>(Deserialize);
        }

        internal static Core.Types.Decimal Deserialize(ISymbolNode decimalNode)
        {
            decimalNode = decimalNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(decimalNode)))
                .ThrowIf(
                    node => !Symbol_Decimal.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = decimalNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (decimalNode.TryFindNodes(Symbol_Null, out var nodes))
                return Core.Types.Decimal.Null(attributes!);

            return decimalNode
                .FindNodes(Query)
                .First()
                .ApplyTo(n => BigDecimal.Parse(n.Tokens))
                .Map(bigDecimal => Core.Types.Decimal.Of(bigDecimal, attributes?.ToArray() ?? []))
                .Resolve();
        }
    }
}
