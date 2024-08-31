using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.AxonSerializer.Deserializers
{
    public class TimestampDeserializer : IValueDeserializer<Timestamp>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private readonly static string Query = $"{Symbol_QuotedTimestamp}/{Symbol_TimestampPrecisions}";

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_Timestamp = "dia-timestamp";
        private const string Symbol_Null = "null-timestamp";
        private const string Symbol_QuotedTimestamp = "quoted-timestamp";
        private const string Symbol_TimestampPrecisions = "timestamp-precisions";
        private const string Symbol_Year = "year";
        private const string Symbol_Month = "month";
        private const string Symbol_Day = "day";
        private const string Symbol_Hour = "hour";
        private const string Symbol_Minute = "minute";
        private const string Symbol_Second = "second";
        private const string Symbol_Ticks = "ticks";
        private const string Symbol_Timezone = "time-zone-offset";
        #endregion

        public static Timestamp Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_Timestamp] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_Timestamp}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid timestamp format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Timestamp>(Deserialize);
        }

        internal static Timestamp Deserialize(ISymbolNode timestampNode)
        {
            timestampNode = timestampNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(timestampNode)))
                .ThrowIf(
                    node => !Symbol_Timestamp.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = timestampNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (timestampNode.TryFindNodes(Symbol_Null, out var nodes))
                return Timestamp.Null(attributes!);

            return timestampNode
                .FindNodes(Query)
                .First()
                .As<ISymbolNode.INodeContainer>().Nodes
                .Aggregate((Year:0, Month:1, Day:1, Hour:0, Minute:0, Second:0, Ticks:"0", TimeZone: "Z"), (tsInfo, node) =>
                {
                    return node.Symbol switch
                    {
                        Symbol_Year => tsInfo with { Year = int.Parse(node.Tokens) },
                        Symbol_Month => tsInfo with { Month = int.Parse(node.Tokens) },
                        Symbol_Day => tsInfo with { Day = int.Parse(node.Tokens) },

                        Symbol_Hour => tsInfo with { Hour = int.Parse(node.Tokens) },
                        Symbol_Minute => tsInfo with { Minute = int.Parse(node.Tokens) },
                        Symbol_Second => tsInfo with { Second = int.Parse(node.Tokens) },
                        Symbol_Ticks => tsInfo with
                        {
                            Ticks = $"{node.Tokens}"
                        },
                        
                        Symbol_Timezone => tsInfo with 
                        { 
                            TimeZone = node.Tokens
                        },

                        _ => tsInfo
                    };
                })
                .ApplyTo(info => $"{info.Year}-{info.Month}-{info.Day} {info.Hour}:{info.Minute}:{info.Second}.{info.Ticks} {info.TimeZone}")
                .ApplyTo(DateTimeOffset.Parse)
                .ApplyTo(datetime => Timestamp.Of(datetime, attributes?.ToArray() ?? []));
        }
    }
}
