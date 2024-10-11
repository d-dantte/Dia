using Axis.Dia.Axon;
using Axis.Dia.Axon.Lang;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Deserializers
{
    public class BooleanDeserializer : IValueDeserializer<Core.Types.Boolean>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_True}|{Symbol_False}|{Symbol_Null}";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Bool = "dia-bool";
        private const string Symbol_True = "true-bool";
        private const string Symbol_False = "false-bool";
        private const string Symbol_Null = "null-bool";
        #endregion

        public static Core.Types.Boolean Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Bool] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Bool}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid boolean format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Core.Types.Boolean>(Deserialize);
        }

        internal static Core.Types.Boolean Deserialize(ISymbolNode boolNode)
        {
            boolNode = boolNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(boolNode)))
                .ThrowIf(
                    node => !Symbol_Bool.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = boolNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (boolNode.TryFindNodes(Symbol_Null, out var nodes))
                return Core.Types.Boolean.Null(attributes!);

            return boolNode
                .FindNodes(Query)
                .Select(n => n.Symbol switch
                {
                    Symbol_True => true,
                    Symbol_False => false,
                    _ => (bool?)null
                })
                .First()
                .ApplyTo(@bool => Core.Types.Boolean.Of(@bool, attributes?.ToArray() ?? []));
        }
    }
}
