using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Dia.Json.Path;
using Axis.Luna.Extensions;
using Axis.Luna.Numerics;
using Axis.Luna.Result;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Numerics;

namespace Axis.Dia.Json.Deserializers
{
    internal class ValueDeserializer
    {
        private static readonly string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
        private static readonly string DurationFormat = "dd\\.hh\\:mm\\:ss\\.fffffff";

        #region prefixes
        private static readonly string BlobPrefix = "#Blob";
        private static readonly string BoolPrefix = "#Bool";
        private static readonly string TimestampPrefix = "#Timestamp";
        private static readonly string DurationPrefix = "#Duration";
        private static readonly string SymbolPrefix = "#Symbol";
        private static readonly string StringPrefix = "#String";
        private static readonly string IntPrefix = "#Int";
        private static readonly string DecimalPrefix = "#Decimal";
        private static readonly string SequencePrefix = "#Sequence";
        private static readonly string RecordPrefix = "#Record";
        private static readonly string RefPrefix = "#Ref";
        #endregion

        internal static Blob DeserializeBlob(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (!text.StartsWith(BlobPrefix))
                throw new ArgumentException(
                    $"Invalid {nameof(text)}: blob prefix missing");

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, BlobPrefix) switch
            {
                true => Blob.Null(valueAttributes),
                false => Blob.Of(
                    Convert.FromBase64String(text[(BlobPrefix.Length + 1)..]),
                    valueAttributes)
            };
        }

        internal static Core.Types.Boolean DeserializeBool(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, BoolPrefix) switch
            {
                true => Core.Types.Boolean.Null(valueAttributes),
                false => Core.Types.Boolean.Of(
                    bool.Parse(text),
                    valueAttributes)
            };
        }

        internal static Timestamp DeserializeTimestamp(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, TimestampPrefix) switch
            {
                true => Timestamp.Null(valueAttributes),
                false => Timestamp.Of(
                    DateTimeOffset.ParseExact(text[(TimestampPrefix.Length + 1)..], TimestampFormat, null),
                    valueAttributes)
            };
        }

        internal static Duration DeserializeDuration(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, DurationPrefix) switch
            {
                true => Duration.Null(valueAttributes),
                false => Duration.Of(
                    TimeSpan.ParseExact(text[(DurationPrefix.Length + 1)..], DurationFormat, null),
                    valueAttributes)
            };
        }

        internal static Symbol DeserializeSymbol(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, SymbolPrefix) switch
            {
                true => Symbol.Null(valueAttributes),
                false => Symbol.Of(
                    text[(SymbolPrefix.Length + 1)..],
                    valueAttributes)
            };
        }

        internal static Core.Types.String DeserializeString(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, StringPrefix) switch
            {
                true => Core.Types.String.Null(valueAttributes),
                false => text.Length switch
                {
                    <= 1 => Core.Types.String.Of(text, valueAttributes),
                    _ => text[..2] switch
                    {
                        "##" => Core.Types.String.Of(text[1..], valueAttributes),
                        _ => Core.Types.String.Of(text, valueAttributes)
                    }
                }
            };
        }

        internal static IDiaValue DeserializeRef(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return text[7..]
                .ApplyTo(@ref => int.Parse(@ref, NumberStyles.HexNumber))
                .ApplyTo(context.ReferenceMap.Dereference);
        }

        internal static Integer DeserializeInteger(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, IntPrefix) switch
            {
                true => Integer.Null(valueAttributes),
                false => text.StartsWith($"{IntPrefix} ") switch
                {
                    true => Integer.Of(
                        BigInteger.Parse(text[(IntPrefix.Length + 3)..], System.Globalization.NumberStyles.HexNumber),
                        valueAttributes),

                    false => Integer.Of(
                        BigInteger.Parse(text),
                        valueAttributes)
                }
            };
        }

        internal static Core.Types.Decimal DeserializeDecimal(string text, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return IsNull(text, DecimalPrefix) switch
            {
                true => Core.Types.Decimal.Null(valueAttributes),
                false => text.StartsWith($"{DecimalPrefix} ") switch
                {
                    true => Core.Types.Decimal.Of(
                        BigDecimal.Parse(text[(IntPrefix.Length + 1)..]).Resolve(),
                        valueAttributes),

                    false => Core.Types.Decimal.Of(
                        BigDecimal.Parse(text).Resolve(),
                        valueAttributes)
                }
            };
        }

        internal static Sequence DeserializeSequence(JArray? array, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return array switch
            {
                null => Sequence.Null(valueAttributes),
                JArray jarray => context.MetadataMap[path]
                    .ApplyTo(metadata => context.ReferenceMap.GetOrAdd(
                        metadata.Ref!.Value,
                        @ref => Sequence.Of([], [.. metadata.ValueAttributes])))
                    .ApplyTo(seq => jarray.Aggregate(seq.As<Sequence>(), (seq, token, index) =>
                    {
                        var indexPath = path.Append(Path.Index.Of(index));
                        seq.Add(DiaValue.Of(DeserializeToken(token, indexPath, context)));
                        return seq;
                    }))
            };
        }

        internal static Record DeserializeRecord(JObject? @object, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(@object);
            ArgumentNullException.ThrowIfNull(context);

            var valueAttributes = context.GetMetadataValueAttributes(path);
            return @object switch
            {
                null => Record.Null(valueAttributes),
                JObject jobject => context
                    .GetMetadataRef(path)
                    .ApplyTo(@ref => context.ReferenceMap.GetOrAdd(
                        @ref!.Value,
                        @ref => Record.Of([], [.. context.GetMetadataValueAttributes(path)])))
                    .ApplyTo(rec => jobject.Properties().Aggregate(rec.As<Record>(), (rec, prop) =>
                    {
                        var propPath = path.Append(Property.Of($"@{prop.Name}"));
                        var diaProp = Record.PropertyName.Of(
                            prop.Name,
                            context.GetMetadataPropertyAttributes(propPath));

                        rec[diaProp] = DiaValue.Of(DeserializeToken(prop.Value, propPath, context));
                        return rec;
                    }))
            };
        }


        internal static IDiaValue DeserializeToken(JToken token, ValuePath path, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(token);
            ArgumentNullException.ThrowIfNull(context);

            var info = ExtractValue(token);
            return info.Type switch
            {
                DiaType.Blob => DeserializeBlob(info.Value.As<string>(), path, context),
                DiaType.Bool => DeserializeBool(info.Value.As<string>(), path, context),
                DiaType.Decimal => DeserializeDecimal(info.Value.As<string>(), path, context),
                DiaType.Duration => DeserializeDuration(info.Value.As<string>(), path, context),
                DiaType.Int => DeserializeInteger(info.Value.As<string>(), path, context),
                DiaType.Record => DeserializeRecord(info.Value.As<JObject>(), path, context),
                DiaType.Sequence => DeserializeSequence(info.Value.As<JArray>(), path, context),
                DiaType.String => DeserializeString(info.Value.As<string>(), path, context),
                DiaType.Symbol => DeserializeSymbol(info.Value.As<string>(), path, context),
                DiaType.Timestamp => DeserializeTimestamp(info.Value.As<string>(), path, context),
                DiaType dt when ((DiaType)15).Equals(dt) => DeserializeRef(info.Value.As<string>(), path, context),
                _ => throw new InvalidOperationException(
                    $"Invalid dia-type: {info.Type}")
            };
        }

        private static bool IsNull(string text, string prefix)
        {
            ArgumentNullException.ThrowIfNull(text);

            return $"{prefix}.null".Equals(text);
        }

        private static (DiaType Type, object Value) ExtractValue(JToken token)
        {
            ArgumentNullException.ThrowIfNull(token);

            return token.Type switch
            {
                JTokenType.Boolean => (DiaType.Bool, token.Value<string>()!),
                JTokenType.Integer => (DiaType.Int, token.Value<string>()!),
                JTokenType.Float => (DiaType.Decimal, token.Value<string>()!),
                JTokenType.Array => (DiaType.Sequence, token),
                JTokenType.Object => (DiaType.Record, token),
                JTokenType.Bytes => (DiaType.Blob, token),
                JTokenType.String => token.ToString() switch
                {
                    string text when text.StartsWith(BlobPrefix) => (DiaType.Blob, text),
                    string text when text.StartsWith(BoolPrefix) => (DiaType.Bool, text),
                    string text when text.StartsWith(DecimalPrefix) => (DiaType.Decimal, text),
                    string text when text.StartsWith(DurationPrefix) => (DiaType.Duration, text),
                    string text when text.StartsWith(IntPrefix) => (DiaType.Int, text),
                    string text when text.StartsWith(RecordPrefix) => (DiaType.Record, text),
                    string text when text.StartsWith(SequencePrefix) => (DiaType.Sequence, text),
                    string text when text.StartsWith(StringPrefix) => (DiaType.String, text),
                    string text when text.StartsWith(SymbolPrefix) => (DiaType.Symbol, text),
                    string text when text.StartsWith(TimestampPrefix) => (DiaType.Timestamp, text),
                    string text when text.StartsWith(RefPrefix) => ((DiaType)15, text),
                    string text => (DiaType.String, text)
                },
                _ => throw new InvalidOperationException($"Invalid token type: {token.Type}")
            };
        }
    }
}
