using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    public class ListParser :
        IRootSymbolProvider,
        IJsonConverter<ListValue>
    {
        internal const string SymbolNameArray = "array";
        internal const string SymbolNameMetaItem = "meta-item";
        internal const string SymbolNameContainerMetadata = "container-metadata";
        internal const string SymbolNameJsonValue = "json-value";

        public static string RootSymbol => SymbolNameArray;

        public static IResult<ListValue> Parse(CSTNode arrayNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(arrayNode);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (!SymbolNameArray.Equals(arrayNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol '{arrayNode.SymbolName}', expected '{SymbolNameArray}'");

            var containerMetadata = arrayNode
                .FindNodes($"{SymbolNameMetaItem}/{SymbolNameContainerMetadata}")
                .Select(node => ContainerMetadataParser.Parse(node, context))
                .FirstOrDefault()
                ?? Result.Of(new ContainerMetadataParser.ContainerMetadata(null, Array.Empty<Annotation>()));

            return arrayNode
                .FindNodes(SymbolNameJsonValue)
                .Select(node => JsonSerializer.ParseValue(node, context))
                .FoldInto(items => items)
                .Combine(containerMetadata, (items, metadata) =>
                {
                    var list = ListValue.Of(items, metadata.Annotations);

                    if (metadata.AddressIndex is not null)
                        return context
                            .Track(metadata.AddressIndex.Value)
                            .ApplyTo(list.RelocateValue);

                    return list;
                });
        }

        public static IResult<string> Serialize(ListValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (value.IsNull)
                return EncodedValueParser.Serialize(value, context);

            if (!value.HasAnnotations() && value.Count == 0)
                return Result.Of("[]");

            var lineJoiner = context.Options.Lists.UseMultipleLines
                ? $"{Environment.NewLine}"
                : "";

            var itemIndentation = context.Options.Lists.UseMultipleLines
                ? context.IndentText("", 1)
                : " ";

            var closingBracketIndentation = context.Options.Lists.UseMultipleLines
                ? context.IndentText("")
                : " ";

            var result = value.Value!
                .Select(item => JsonSerializer.InternalSerializeValue(item, context.Indent()))
                .Select(itemText => itemText.Map(v => $"{lineJoiner}{itemIndentation}{v}"));

            if (context.TryGetAddressIndex(value, out var addressIndex)
                || value.HasAnnotations())
            {
                var metadata = new ContainerMetadataParser.ContainerMetadata(addressIndex, value.Annotations);
                result = result.InsertAt(0, ContainerMetadataParser
                    .Serialize(metadata, context)
                    .Map(text => $"0${text}".WrapIn("\""))
                    .Map(v => $"{lineJoiner}{itemIndentation}{v}"));
            }

            return result
                .FoldInto(items => items.JoinUsing($","))
                .Map(listText => $"[{listText}{lineJoiner}{closingBracketIndentation}]");
        }
    }
}
