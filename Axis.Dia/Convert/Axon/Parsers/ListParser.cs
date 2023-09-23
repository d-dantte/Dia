using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class ListParser : IValueSerializer<ListValue>
    {
        #region Symbols
        public const string SymbolNameDiaList = "dia-list";
        public const string SymbolNameNullList = "null-list";
        public const string SymbolNameListValue = "list-value";
        public const string SymbolNameDiaValue = "dia-value";
        #endregion


        private ListParser() { }

        public static string GrammarSymbol => SymbolNameDiaList;

        public static IResult<ListValue> Parse(CSTNode symbolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            context.ThrowIfDefault($"Invalid {nameof(context)} instance");

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (AddressIndexNode, AnnotationNode, ValueNode) = symbolNode.DeconstructValueNode();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                var result = ValueNode.SymbolName switch
                {
                    SymbolNameNullList => annotationResult.Map(ListValue.Null),
                    SymbolNameListValue => ValueNode
                        .FindNodes(SymbolNameDiaValue)
                        .Select(node => AxonSerializer.ParseValue(node, context))
                        .Fold()
                        .Combine(
                            annotationResult,
                            (items, annotations) => ListValue.Of(annotations, items.ToArray())),

                    _ => Result.Of<ListValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullList}', or '{SymbolNameListValue}'"))
                };

                return AddressIndexNode is not null
                    ? result.Combine(
                        AddressIndexParser.Parse(AddressIndexNode),
                        (value, addressIndex) => value.RelocateValue(context.Track(addressIndex)))
                    : result;
            }
            catch (Exception e)
            {
                return Result.Of<ListValue>(e);
            }
        }


        public static IResult<string> Serialize(ListValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var addressIndexText = context.TryGetAddressIndex(value, out var index)
                ? $"#0x{index:x}"
                : "";

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);

            var lineJoiner = context.Options.Lists.UseMultipleLines
                ? $"{Environment.NewLine}"
                : "";

            var itemIndentation = context.Options.Lists.UseMultipleLines
                ? context.IndentText("", 1)
                : " ";

            var closingBracketIndentation = context.Options.Lists.UseMultipleLines
                ? context.IndentText("")
                : " ";

            var valueText = value.IsNull switch
            {
                true => Result.Of("null.list"),
                false => value.Value!
                    .Select(item => AxonSerializer.InternalSerializeValue(item, context.Indent()))
                    .Select(itemText => itemText.Map(v => $"{lineJoiner}{itemIndentation}{v}"))
                    .FoldInto(items => items.JoinUsing($","))
                    .Map(listText => $"[{listText}{lineJoiner}{closingBracketIndentation}]")
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{addressIndexText}{ann}{value}");
        }
    }
}
