using Axis.Dia.Core.Attributes;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Json.Deserializers;
using Axis.Dia.Json.Serializers;
using Axis.Luna.Extensions;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Json
{
    public class Serializer
    {
        public static JToken Serialize(IDiaValue dia)
        {
            ArgumentNullException.ThrowIfNull(dia);

            var jobj = new JObject();
            var context = new SerializerContext();

            return dia switch
            {
                Core.Types.Blob value => ValueSerializer.SerializeBlob(value, context),
                Core.Types.Boolean value => ValueSerializer.SerializeBool(value, context),
                Core.Types.Decimal value => ValueSerializer.SerializeDecimal(value, context),
                Core.Types.Duration value => ValueSerializer.SerializeDuration(value, context),
                Core.Types.Integer value => ValueSerializer.SerializeInteger(value, context),
                Core.Types.Record value => ValueSerializer.SerializeRecord(value, context),
                Core.Types.Sequence value => ValueSerializer.SerializeSequence(value, context),
                Core.Types.String value => ValueSerializer.SerializeString(value, context),
                Core.Types.Symbol value => ValueSerializer.SerializeSymbol(value, context),
                Core.Types.Timestamp value => ValueSerializer.SerializeTimestamp(value, context),
                _ => throw new InvalidOperationException(
                    $"Invalid dia value: {dia}")
            };
        }

        public static  IDiaValue Deserialize(JToken json) => Deserialize(json, new DeserializerContext());

        public static IDiaValue Deserialize(JToken token, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(token);
            ArgumentNullException.ThrowIfNull(context);

            var value = token switch
            {
                JArray array => ValueDeserializer.DeserializeSequence(array, context),
                JObject jobj => ValueDeserializer.DeserializeRecord(jobj, context),
                JToken => token.Type switch
                {
                    JTokenType.Boolean => ValueDeserializer.DeserializeBool(token.Value<string>()!, context),
                    JTokenType.Float => ValueDeserializer.DeserializeDecimal(token.Value<string>()!, context),
                    JTokenType.Integer => ValueDeserializer.DeserializeInt(token.Value<string>()!, context),
                    JTokenType.String => DeserializeString(token.As<JValue>(), context),

                    // Special cases
                    JTokenType.Bytes => Core.Types.Blob.Of(token.Value<byte[]>()!),
                    JTokenType.Date => Core.Types.Timestamp.Of(token.ToObject<DateTimeOffset>()),
                    JTokenType.TimeSpan => Core.Types.Duration.Of(token.ToObject<TimeSpan>()),

                    // All nulls are seen as #Record.null
                    JTokenType.Null => ValueDeserializer.DeserializeNullRecord(
                        $"{CanonicalFormParser.RecordPrefix}.null",
                        context), 

                    JTokenType.Guid => Core.Types.String.Of(
                        token.ToString(Newtonsoft.Json.Formatting.None),
                        WellKnownTypes.Guid.ToAttribute()),

                    JTokenType.Uri => Core.Types.String.Of(
                        token.ToString(Newtonsoft.Json.Formatting.None),
                        WellKnownTypes.Uri.ToAttribute()),

                    // Unsupported values
                    JTokenType.None
                    or JTokenType.Raw
                    or JTokenType.Array
                    or JTokenType.Object
                    or JTokenType.Comment
                    or JTokenType.Property
                    or JTokenType.Undefined
                    or JTokenType.Constructor
                    or _ => throw new InvalidOperationException(
                        $"Invalid jtoken: {token.Type}"),
                }
            };

            // resolve the refs
            context.ResolveRefs();

            return value;
        }

        internal static IDiaValue DeserializeString(JValue @string, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(@string);
            ArgumentNullException.ThrowIfNull(context);

            var text = @string.Value<string>()!;
            return
                text.StartsWith(CanonicalFormParser.BlobPrefix) ? ValueDeserializer.DeserializeBlob(text, context) :
                text.StartsWith(CanonicalFormParser.BoolPrefix) ? ValueDeserializer.DeserializeBool(text, context) :
                text.StartsWith(CanonicalFormParser.DecimalPrefix) ? ValueDeserializer.DeserializeDecimal(text, context) :
                text.StartsWith(CanonicalFormParser.DurationPrefix) ? ValueDeserializer.DeserializeDuration(text, context) :
                text.StartsWith(CanonicalFormParser.IntPrefix) ? ValueDeserializer.DeserializeInt(text, context) :
                text.StartsWith(CanonicalFormParser.RecordPrefix) ? ValueDeserializer.DeserializeNullRecord(text, context) :
                text.StartsWith(CanonicalFormParser.RefPrefix) ? ValueDeserializer.DeserializeRef(text, context) :
                text.StartsWith(CanonicalFormParser.SequencePrefix) ? ValueDeserializer.DeserializeNullSequence(text, context) :
                text.StartsWith(CanonicalFormParser.SymbolPrefix) ? ValueDeserializer.DeserializeSymbol(text, context) :
                text.StartsWith(CanonicalFormParser.TimestampPrefix) ? ValueDeserializer.DeserializeTimestamp(text, context) :
                ValueDeserializer.DeserializeString(text, context);
        }
    }
}
