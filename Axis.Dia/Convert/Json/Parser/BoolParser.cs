using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class BoolParser :
        IRootSymbolProvider,
        IJsonConverter<BoolValue>
    {
        internal const string SymbolNameBoolValue = "bool-value";

        public static string RootSymbol => SymbolNameBoolValue;

        public IResult<BoolValue> Parse(CSTNode boolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(boolNode);
            ArgumentNullException.ThrowIfNull(context);

            return boolNode.SymbolName switch
            {
                SymbolNameBoolValue => Result
                    .Of(boolNode.TokenValue)
                    .Map(bool.Parse)
                    .Map(v => BoolValue.Of(v)),

                //SymbolNameEncodedBoolValue => EncodedValueExtractor
                //    .ExtractEncodedInfo(boolNode, DiaType.Bool, context)
                //    .Map(info =>
                //    {
                //        var value = info.ValueText.TokenValue().ToLower();
                //        return "null".Equals(value)
                //            ? BoolValue.Null(info.Annotations!)
                //            : BoolValue.Of(bool.Parse(value), info.Annotations!);
                //    }),

                _ => Result.Of<BoolValue>(new InvalidOperationException(
                    $"Invalid symbol name: '{boolNode.SymbolName}', expected '{SymbolNameBoolValue}'"))
            };
        }

        public IResult<string> Serialize(BoolValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            //if (value.IsNull || value.HasAnnotations())
            //{
            //    var boolText = Result.Of(!value.IsNull
            //        ? value.Value!.Value.ToString().ToLower()
            //        : "null");

            //    var annotations = AnnotationParser.Serialize(value.Annotations, context);

            //    var addressIndex = context.TryGetAddressIndex(value, out var index) ? $"#{index};" : "";

            //    return boolText
            //        .Combine(annotations, (text, annotationText) => (BoolText: text, AnnotationText: annotationText))
            //        .Map(info => $"\"[{addressIndex}${DiaType.Bool};{info.AnnotationText}]{info.BoolText}\"");
            //}

            //else
            return Result.Of(() => value.Value!.Value.ToString().ToLower());
        }
    }
}
