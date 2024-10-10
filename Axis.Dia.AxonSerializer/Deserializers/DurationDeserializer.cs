using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using System.Text;

namespace Axis.Dia.Axon.Deserializers
{
    public class DurationDeserializer : IValueDeserializer<Duration>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_QuotedDuration}/{Symbol_Components}";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Duration = "dia-duration";
        private const string Symbol_Null = "null-duration";
        private const string Symbol_QuotedDuration = "quoted-duration";
        private const string Symbol_Components = "duration-components";
        private const string Symbol_DaysComponent = "duration-days-component";
        private const string Symbol_MainComponent = "duration-main-component";
        private const string Symbol_TicksComponent = "duration-ticks-component";
        #endregion

        public static Duration Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Duration] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Duration}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid duration format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Duration>(Deserialize);
        }

        internal static Duration Deserialize(ISymbolNode durationNode)
        {
            durationNode = durationNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(durationNode)))
                .ThrowIf(
                    node => !Symbol_Duration.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = durationNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (durationNode.TryFindNodes(Symbol_Null, out var nodes))
                return Duration.Null(attributes!);

            return durationNode
                .FindNodes(Query)
                .First()
                .ApplyTo(node =>
                {
                    var sb = new StringBuilder();

                    #region Days
                    sb = node.TryFindNodes(Symbol_DaysComponent, out var nodes)
                        ? sb.Append(nodes.First().Tokens)
                        : sb.Append("0.");
                    #endregion

                    #region HMS
                    sb = node
                        .FindNodes(Symbol_MainComponent)
                        .First()
                        .ApplyTo(n => sb.Append(n.Tokens));
                    #endregion

                    #region Ticks
                    sb = node.TryFindNodes(Symbol_TicksComponent, out nodes)
                        ? sb.Append(nodes.First().Tokens)
                        : sb.Append(".0000000");
                    #endregion

                    return TimeSpan.Parse(sb.ToString());
                })
                .ApplyTo(timespan => Duration.Of(timespan, attributes?.ToArray() ?? []));
        }
    }
}
