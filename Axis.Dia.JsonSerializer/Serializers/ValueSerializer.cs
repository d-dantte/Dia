using Axis.Dia.Core.Types;
using Axis.Luna.Common.StringEscape;
using Axis.Luna.Extensions;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Json.Serializers
{
    internal class ValueSerializer
    {
        private static readonly string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
        private static readonly string DurationFormat = "dd\\.hh\\:mm\\:ss\\.fffffff";

        internal static readonly string ValueAttributeMetadataProperty = "$";
        internal static readonly string ValueRefMetadataProperty = "#";
        internal static readonly string MetadataInstanceProperty = "~";
        private static readonly CommonStringEscaper StringEscaper = new();

        internal static JToken SerializeRecord(Record record, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.ReferenceMap.TryAddRef(record, out var @ref))
            {
                if (record.IsNull)
                {
                    return record.Attributes.Count switch
                    {
                        0 => new JValue("#Record.null"),
                        _ => new JValue($"#Record.null{SerializeAttributes(record.Attributes)}")
                    };
                }
                else
                {
                    var metadata = new JObject
                    {
                        [ValueRefMetadataProperty] = $"0x{@ref.JsonHash:x}"
                    };

                    if (record.Attributes.Count > 0)
                        metadata[ValueAttributeMetadataProperty] = SerializeAttributes(record.Attributes);

                    var json = new JObject
                    {
                        [MetadataInstanceProperty] = metadata
                    };

                    return record
                        .Select(prop =>
                        {
                            // serialize property attribute
                            if (prop.Name.Attributes.Count > 0)
                                metadata[$"${prop.Name.Name}"] = SerializeAttributes(prop.Name.Attributes);

                            return (prop.Name.Name, Value: prop.Value.Payload switch
                            {
                                Core.Types.Blob value => SerializeBlob(value, context),
                                Core.Types.Boolean value => SerializeBool(value, context),
                                Core.Types.Decimal value => SerializeDecimal(value, context),
                                Core.Types.Duration value => SerializeDuration(value, context),
                                Core.Types.Integer value => SerializeInteger(value, context),
                                Core.Types.Record value => SerializeRecord(value, context),
                                Core.Types.Sequence value => SerializeSequence(value, context),
                                Core.Types.String value => SerializeString(value, context),
                                Core.Types.Symbol value => SerializeSymbol(value, context),
                                Core.Types.Timestamp value => SerializeTimestamp(value, context),
                                _ => throw new InvalidOperationException(
                                    $"Invalid dia value: {prop.Value.Payload}")
                            });
                        })
                        .Aggregate(json, (jobj, prop) =>
                        {
                            jobj[prop.Name] = prop.Value;
                            return jobj;
                        });
                }
            }
            else
            {
                return new JValue($"#Ref 0x{@ref.JsonHash:x}");
            }
        }

        internal static JToken SerializeSequence(Sequence sequence, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.ReferenceMap.TryAddRef(sequence, out var @ref))
            {
                if (sequence.IsNull)
                {
                    return sequence.Attributes.Count switch
                    {
                        0 => new JValue("#Sequence.null"),
                        _ => new JValue($"#Sequence.null {SerializeAttributes(sequence.Attributes)}")
                    };
                }
                else
                {
                    var metadata = new JObject
                    {
                        [ValueRefMetadataProperty] = $"0x{@ref.JsonHash:x}"
                    };

                    if (sequence.Attributes.Count > 0)
                        metadata[ValueAttributeMetadataProperty] = SerializeAttributes(sequence.Attributes);

                    var json = new JArray
                    {
                        metadata
                    };

                    return sequence
                        .Select((item, index) =>
                        {
                            return item.Payload switch
                            {
                                Core.Types.Blob value => SerializeBlob(value, context),
                                Core.Types.Boolean value => SerializeBool(value, context),
                                Core.Types.Decimal value => SerializeDecimal(value, context),
                                Core.Types.Duration value => SerializeDuration(value, context),
                                Core.Types.Integer value => SerializeInteger(value, context),
                                Core.Types.Record value => SerializeRecord(value, context),
                                Core.Types.Sequence value => SerializeSequence(value, context),
                                Core.Types.Symbol value => SerializeSymbol(value, context),
                                Core.Types.Timestamp value => SerializeTimestamp(value, context),
                                Core.Types.String value => SerializeString(value, context),
                                _ => throw new InvalidOperationException(
                                    $"Invalid dia value: {item.Payload}")
                            };
                        })
                        .Aggregate(json, (jarr, item) =>
                        {
                            jarr.Add(item);
                            return jarr;
                        });
                }
            }
            else
            {
                return new JValue($"#Ref 0x{@ref.JsonHash:x}");
            }
        }

        internal static JToken SerializeTimestamp(Timestamp value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var text = (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => $"#Timestamp.null{SerializeAttributes(value.Attributes)}",
                (true, false) => $"#Timestamp.null",
                (false, true) => $"#Timestamp{SerializeAttributes(value.Attributes)} {value.Value!.Value.ToString(TimestampFormat)}",
                (false, false) => $"#Timestamp {value.Value!.Value.ToString(TimestampFormat)}"
            };

            return new JValue(text);
        }

        internal static JToken SerializeSymbol(Symbol value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var text = (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => $"#Symbol.null{SerializeAttributes(value.Attributes)}",
                (true, false) => $"#Symbol.null",
                (false, true) => $"#Symbol{SerializeAttributes(value.Attributes)} '{value.Value}'",
                (false, false) => $"#Symbol '{value.Value}'"
            };

            return new JValue(text);
        }

        internal static JToken SerializeString(Core.Types.String value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var text = (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => $"#String.null{SerializeAttributes(value.Attributes)}",
                (true, false) => $"#String.null",
                (false, true) => $"#String{SerializeAttributes(value.Attributes)} '{value.Value}'",
                (false, false) => value.Value!.StartsWith("#String") ? $"#{value.Value}" : value.Value
            };

            return new JValue(text);
        }

        internal static JToken SerializeInteger(Integer value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => new JValue($"#Int.null{SerializeAttributes(value.Attributes)}"),
                (true, false) => new JValue($"#Int.null"),
                (_, bool hasAttributes) => (value.Value <= int.MaxValue, hasAttributes) switch
                {
                    (true, true) =>  new JValue($"#Int{SerializeAttributes(value.Attributes)} {value.Value}"),
                    (true, false) => new JValue((int)value.Value!),
                    (false, true) => new JValue($"#Int{SerializeAttributes(value.Attributes)} {value.Value:x}"),
                    (false, false) => new JValue($"#Int {value.Value:x}"),
                }
            };
        }

        internal static JToken SerializeDuration(Duration value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var text = (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => $"#Duration.null{SerializeAttributes(value.Attributes)}",
                (true, false) => $"#Duration.null",
                (false, true) => $"#Duration{SerializeAttributes(value.Attributes)} {value.Value!.Value.ToString(DurationFormat)}",
                (false, false) => $"#Duration {value.Value}"
            };

            return new JValue(text);
        }

        internal static JToken SerializeDecimal(Core.Types.Decimal value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var dec = value.Value!.Value.ToScientificString();
            return (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => new JValue($"#Decimal.null{SerializeAttributes(value.Attributes)}"),
                (true, false) => new JValue($"#Decimal.null"),
                (_, bool hasAttributes) => (value.Value <= double.MaxValue, hasAttributes) switch
                {
                    (true, true) => new JValue($"#Decimal{SerializeAttributes(value.Attributes)} {dec}"),
                    (true, false) => new JValue((double)value.Value!),
                    (false, true) => new JValue($"#Decimal{SerializeAttributes(value.Attributes)} {dec}"),
                    (false, false) => new JValue($"#Decimal {dec}"),
                }
            };
        }

        internal static JToken SerializeBool(Core.Types.Boolean value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => new JValue($"#Bool.null{SerializeAttributes(value.Attributes)}"),
                (true, false) => new JValue($"#Bool.null"),
                (false, true) => new JValue($"#Bool{SerializeAttributes(value.Attributes)} {value.Value}"),
                (false, false) => new JValue(value.Value!.Value)
            };
        }

        internal static JToken SerializeBlob(Blob value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var text = (value.IsNull, value.Attributes.Count > 0) switch
            {
                (true, true) => $"#Blob.null{SerializeAttributes(value.Attributes)}",
                (true, false) => $"#Blob.null",
                (false, true) => $"#Blob{SerializeAttributes(value.Attributes)} {Convert.ToBase64String(value.Value!.Value.ToArray())}",
                (false, false) => $"#Blob {Convert.ToBase64String(value.Value!.Value.ToArray())}"
            };

            return new JValue(text);
        }

        private static string SerializeAttributes(AttributeSet attributes)
        {
            return attributes
                .Select(att => att.IsScalar switch
                {
                    true => $"@{att.Key};",
                    false => $"@{att.Key}:{StringEscaper.Escape(att.Value!, IsAttributeEscapableCharacter)};"
                })
                .JoinUsing(" ")
                .ApplyTo(str => $"[{str}]");
        }

        private static bool IsAttributeEscapableCharacter(char c)
        {
            return c switch
            {
                '\b' => true,
                '\f' => true,
                '\n' => true,
                '\r' => true,
                '\t' => true,
                '\\' => true,
                '\"' => true,
                '\'' => true,
                _ => false
            };
        }
    }
}
