using Axis.Dia.Core.Contracts;
using Axis.Dia.Json.Deserializers;
using Axis.Dia.Json.Path;
using Axis.Dia.Json.Serializers;
using Axis.Luna.Extensions;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.Globalization;

namespace Axis.Dia.Json
{
    using FullMetadata = (
        ValuePath Path,
        int? Ref,
        ImmutableArray<Core.Types.Attribute> ValueAttributes,
        ImmutableArray<Core.Types.Attribute> PropertyAttributes);

    public class Serializer
    {
        private static readonly string DiaRootProperty = "dia";
        private static readonly string MetadataRootProperty = "metadata";
        private static readonly string MetadataRefProperty = "ref";
        private static readonly string MetadataValueAttributeProperty = "value-attribute";
        private static readonly string MetadataPropertyAttributeProperty = "property-attribute";

        public JObject Serialize(IDiaValue dia)
        {
            ArgumentNullException.ThrowIfNull(dia);

            var jobj = new JObject();
            var path = new ValuePath();
            var context = new SerializerContext();

            jobj[DiaRootProperty] = dia switch
            {
                Core.Types.Blob value => ValueSerializer.SerializeBlob(value, path, context),
                Core.Types.Boolean value => ValueSerializer.SerializeBool(value, path, context),
                Core.Types.Decimal value => ValueSerializer.SerializeDecimal(value, path, context),
                Core.Types.Duration value => ValueSerializer.SerializeDuration(value, path, context),
                Core.Types.Integer value => ValueSerializer.SerializeInteger(value, path, context),
                Core.Types.Record value => ValueSerializer.SerializeRecord(value, path, context),
                Core.Types.Sequence value => ValueSerializer.SerializeSequence(value, path, context),
                Core.Types.String value => ValueSerializer.SerializeString(value, path, context),
                Core.Types.Symbol value => ValueSerializer.SerializeSymbol(value, path, context),
                Core.Types.Timestamp value => ValueSerializer.SerializeTimestamp(value, path, context),
                _ => throw new InvalidOperationException(
                    $"Invalid dia value: {dia}")
            };
            jobj[MetadataRootProperty] = context.GenerateMetadata();

            return jobj;
        }

        public IDiaValue Deserialize(JObject jsonPacket)
        {
            ArgumentNullException.ThrowIfNull(jsonPacket);

            if (!jsonPacket.ContainsKey(DiaRootProperty)
                || !jsonPacket.ContainsKey(MetadataRootProperty))
                throw new ArgumentException(
                    $"Invalid json-packet: missing properties '{DiaRootProperty}', and '{MetadataRootProperty}'");

            // Extract metadata
            var metadataList = DeserializeMetadata(jsonPacket[MetadataRootProperty]!.As<JObject>());

            var context = new DeserializerContext(metadataList);
            var path = new ValuePath();

            return ValueDeserializer.DeserializeToken(jsonPacket[DiaRootProperty]!, path, context);
        }

        /// <summary>
        /// TODO: support missing path metadata objects - when data is missing, assume default <c> (Ref: null, ValueAtts: [], PropAtts: []) </c>.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        internal static FullMetadata[] DeserializeMetadata(JObject metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            return metadata.Properties()
                .Select(prop => (
                    Path: ValuePath.Of(prop.Name),
                    Obj: prop.Value.As<JObject>()))
                .Select(info => (
                    info.Path,
                    Ref: info.Obj[MetadataRefProperty]?
                        .Value<string>()?
                        .ApplyTo(text => int.Parse(text[2..], NumberStyles.HexNumber)),
                    ValueAttributes: DeserializeAttributes(info.Obj[MetadataValueAttributeProperty]?.As<JValue>()),
                    PropertyAttributes: DeserializeAttributes(info.Obj[MetadataPropertyAttributeProperty]?.As<JValue>())))
                .ToArray();
        }

        internal static ImmutableArray<Core.Types.Attribute> DeserializeAttributes(JValue? attributes)
        {
            if (attributes is null)
                return [];

            var text = attributes.Value<string>();

            if (text is null)
                return [];

            return AttributeParser.TryParseAttributes(text, out var diaAttributes)
                ? [.. diaAttributes!]
                : throw new InvalidOperationException($"Invalid attribute text: '{text}'");
        }
    }
}
