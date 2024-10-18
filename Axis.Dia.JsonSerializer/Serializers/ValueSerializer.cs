using Axis.Dia.Core.Types;
using Axis.Dia.Json.Path;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Json.Serializers
{
    internal class ValueSerializer
    {
        private static readonly string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
        private static readonly string DurationFormat = "dd\\.hh\\:mm\\:ss\\.fffffff";

        internal static JToken SerializeRecord(Record record, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.ReferenceMap.TryAddRef(record, out var @ref))
            {
                // serialize value attributes
                context.SerializeValueAttributes(record.Attributes, path);

                // serialize the ref
                context.SerializeRef(@ref.JsonHash, path);

                if (record.IsNull)
                    return new JValue("#Record.null");

                else
                {
                    return record
                        .Select(prop =>
                        {
                            var propPath = path.Append($"{Path.Property.PropertyNotationPrefixChar}{prop.Name.Name}");

                            // serialize property attribute
                            context.SerializePropertyAttributes(prop.Name.Attributes, propPath);

                            return (prop.Name.Name, Value: prop.Value.Payload switch
                            {
                                Core.Types.Blob value => SerializeBlob(value, propPath, context),
                                Core.Types.Boolean value => SerializeBool(value, propPath, context),
                                Core.Types.Decimal value => SerializeDecimal(value, propPath, context),
                                Core.Types.Duration value => SerializeDuration(value, propPath, context),
                                Core.Types.Integer value => SerializeInteger(value, propPath, context),
                                Core.Types.Record value => SerializeRecord(value, propPath, context),
                                Core.Types.Sequence value => SerializeSequence(value, propPath, context),
                                Core.Types.String value => SerializeString(value, propPath, context),
                                Core.Types.Symbol value => SerializeSymbol(value, propPath, context),
                                Core.Types.Timestamp value => SerializeTimestamp(value, propPath, context),
                                _ => throw new InvalidOperationException(
                                    $"Invalid dia value: {prop.Value.Payload}")
                            });
                        })
                        .Aggregate(new JObject(), (jobj, prop) =>
                        {
                            jobj[prop.Name] = prop.Value;
                            return jobj;
                        });                    
                }
            }
            else
            {
                return new JValue($"#Ref {@ref.JsonHash:x}");
            }
        }

        internal static JToken SerializeSequence(Sequence sequence, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.ReferenceMap.TryAddRef(sequence, out var @ref))
            {
                // serialize value attributes
                context.SerializeValueAttributes(sequence.Attributes, path);

                // serialize the ref
                context.SerializeRef(@ref.JsonHash, path);

                if (sequence.IsNull)
                    return new JValue("#Sequence.null");

                else
                {
                    return sequence
                        .Select((item, index) =>
                        {
                            var propPath = path.Append($"{Path.Index.IndexNotationPrefixChar}{index}");

                            return item.Payload switch
                            {
                                Core.Types.Blob value => SerializeBlob(value, propPath, context),
                                Core.Types.Boolean value => SerializeBool(value, propPath, context),
                                Core.Types.Decimal value => SerializeDecimal(value, propPath, context),
                                Core.Types.Duration value => SerializeDuration(value, propPath, context),
                                Core.Types.Integer value => SerializeInteger(value, propPath, context),
                                Core.Types.Record value => SerializeRecord(value, propPath, context),
                                Core.Types.Sequence value => SerializeSequence(value, propPath, context),
                                Core.Types.String value => SerializeString(value, propPath, context),
                                Core.Types.Symbol value => SerializeSymbol(value, propPath, context),
                                Core.Types.Timestamp value => SerializeTimestamp(value, propPath, context),
                                _ => throw new InvalidOperationException(
                                    $"Invalid dia value: {item.Payload}")
                            };
                        })
                        .Aggregate(new JArray(), (jobj, item) =>
                        {
                            jobj.Add(item);
                            return jobj;
                        });
                }
            }
            else
            {
                return new JValue($"#Ref {@ref.JsonHash:x}");
            }
        }

        internal static JToken SerializeTimestamp(Timestamp value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#Timestamp.null");

            else 
                return new JValue($"#Timestamp {value.Value!.Value.ToString(TimestampFormat)}");
        }

        internal static JToken SerializeSymbol(Symbol value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#Symbol.null");

            else
                return new JValue($"#Symbol {value.Value}");
        }

        internal static JToken SerializeString(Core.Types.String value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#String.null");

            else
                return new JValue(value.Value);
        }

        internal static JToken SerializeInteger(Integer value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#Int.null");

            else if (value.Value < int.MaxValue)
                return new JValue((int)value.Value);

            else
                return new JValue($"#Int {value.Value:x}");
        }

        internal static JToken SerializeDuration(Duration value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#Duration.null");

            else
                return new JValue($"#Duration {value.Value!.Value.ToString(DurationFormat)}");
        }

        internal static JToken SerializeDecimal(Core.Types.Decimal value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#Decimal.null");

            else if (value.Value < double.MaxValue)
                return new JValue((double)value.Value);

            else
                return new JValue($"#Decimal {value.Value!.Value.ToScientificString()}");
        }

        internal static JToken SerializeBool(Core.Types.Boolean value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#Bool.null");

            else
                return new JValue(value.Value);
        }

        internal static JToken SerializeBlob(Blob value, ValuePath path, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // serialize value attributes
            context.SerializeValueAttributes(value.Attributes, path);

            if (value.IsNull)
                return new JValue("#Blob.null");

            else
                return new JValue($"#Blob {Convert.ToBase64String(value.Value!.Value.ToArray())}");
        }
    }
}
