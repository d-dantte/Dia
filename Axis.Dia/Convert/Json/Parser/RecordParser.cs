using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class RecordParser :
        IRootSymbolProvider,
        IJsonConverter<RecordValue>
    {
        internal const string SymbolNameObject = "object";
        internal const string SymbolNameMetaProperty = "meta-property";
        internal const string SymbolNameContainerMetadata = "container-metadata";
        internal const string SymbolNameObjectField = "object-field";
        internal const string SymbolNameFieldName = "field-name";
        internal const string SymbolNameNameMetadata = "name-metadata";
        internal const string SymbolNameAnnotationList = "annotation-list";
        internal const string SymbolNameValueContent = "value-content";
        internal const string SymbolNameJsonValue = "json-value";

        public static string RootSymbol => SymbolNameObject;

        public static IResult<RecordValue> Parse(CSTNode recordNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(recordNode);
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            if (!SymbolNameObject.Equals(recordNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol '{recordNode.SymbolName}', expected '{SymbolNameObject}'");

            var containerMetadata = recordNode
                .FindNodes($"{SymbolNameMetaProperty}/{SymbolNameContainerMetadata}")
                .Select(node => ContainerMetadataParser.Parse(node, context))
                .FirstOrDefault()
                ?? Result.Of(new ContainerMetadataParser.ContainerMetadata(null, Array.Empty<Annotation>()));

            return recordNode
                .FindNodes(SymbolNameObjectField)
                .Select(node => ParseProperty(node, context))
                .FoldInto(items => items)
                .Combine(containerMetadata, (items, metadata) =>
                {
                    var record = RecordValue.Of(items, metadata.Annotations);

                    if (metadata.AddressIndex is not null)
                        return context
                            .Track(metadata.AddressIndex.Value)
                            .ApplyTo(record.RelocateValue);

                    return record;
                });
        }

        public static IResult<string> Serialize(RecordValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            if (value.IsNull)
                return EncodedValueParser.Serialize(value, context);

            if (!value.HasAnnotations() && value.Count == 0)
                return Result.Of("{}");

            var lineJoiner = context.Options.Records.UseMultipleLines
                ? $"{Environment.NewLine}"
                : "";

            var itemIndentation = context.Options.Records.UseMultipleLines
                ? context.IndentText("", 1)
                : " ";

            var closingBracketIndentation = context.Options.Records.UseMultipleLines
                ? context.IndentText("")
                : " ";

            var result = value.Value!
                .Select(property => SerializeProperty(property, context))
                .Select(propertyText => propertyText.Map(v => $"{lineJoiner}{itemIndentation}{v}"));

            if (context.TryGetAddressIndex(value, out var addressIndex)
                || value.HasAnnotations())
            {
                var metadata = new ContainerMetadataParser.ContainerMetadata(addressIndex, value.Annotations);
                result = result.InsertAt(0, ContainerMetadataParser
                    .Serialize(metadata, context)
                    .Map(metadata => $"\"0$\": \"{metadata}\"")
                    .Map(metaProperty => $"{lineJoiner}{itemIndentation}{metaProperty}"));
            }

            return result
                .FoldInto(items => items.JoinUsing($","))
                .Map(recordText => $"{{{recordText}{lineJoiner}{closingBracketIndentation}}}");
        }

        private static IResult<string> SerializeProperty(
            KeyValuePair<SymbolValue, IDiaValue> property,
            SerializerContext context)
        {
            property.ThrowIfDefault(new ArgumentException($"Invalid {nameof(property)} instance"));
            property.Key.ThrowIf(s => s.IsNull, new ArgumentException($"Invalid property name instance"));
            property.Value.ThrowIfNull(new ArgumentException($"Invalid property value instance"));

            var nameAnnotations = !property.Key.HasAnnotations()
                ? Result.Of("")
                : AnnotationParser
                    .Serialize(property.Key.Annotations, context)
                    .Map(text => $"[{text}]");

            var propertyName = nameAnnotations.Map(annotations => $"{annotations}{property.Key.Value!}");
            var propertyValue = JsonSerializer.InternalSerializeValue(property.Value, context.Indent());

            return propertyName.Combine(propertyValue, (name, value) => $"\"{name}\": {value}");
        }

        private static IResult<KeyValuePair<SymbolValue, IDiaValue>> ParseProperty(
            CSTNode objectFieldNode,
            ParserContext context)
        {
            var nameAnnotations = objectFieldNode
                .FindNodes($"{SymbolNameFieldName}/{SymbolNameNameMetadata}/{SymbolNameAnnotationList}")
                .FirstOrOptional()
                .Map(annotationNode => AnnotationParser.Parse(annotationNode, context))
                .ValueOr(Result.Of(Array.Empty<Annotation>()));

            var name = objectFieldNode
                .FindNodes($"{SymbolNameFieldName}/{SymbolNameValueContent}")
                .FirstOrOptional()
                .Map(content => Result.Of(content.TokenValue()))
                .ValueOr(Result.Of<string>(new FormatException(
                    $"Invalid object field: {objectFieldNode.TokenValue()}",
                    new InvalidOperationException(
                        $"Missing symbol: {SymbolNameValueContent}"))));

            var value = objectFieldNode
                .FindAllNodes(SymbolNameJsonValue)
                .FirstOrOptional()
                .Map(jsonNode => JsonSerializer.ParseValue(jsonNode, context))
                .ValueOr(Result.Of<IDiaValue>(new FormatException(
                    $"Invalid object field: {objectFieldNode.TokenValue()}",
                    new InvalidOperationException(
                        $"Missing symbol: {SymbolNameJsonValue}"))));

            return name
                .Combine(nameAnnotations, SymbolValue.Of)
                .Combine(value, KeyValuePair.Create<SymbolValue, IDiaValue>);
        }
    }
}
