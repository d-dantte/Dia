using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.StringEscape;
using Axis.Luna.Extensions;
using Axis.Luna.Numerics;
using Axis.Luna.Result;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;

namespace Axis.Dia.Json.Deserializers
{
    internal static class ValueDeserializer
    {
        private static readonly IStringEscaper StringEscaper = new CommonStringEscaper();

        internal static readonly string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";
        internal static readonly string DurationFormat = "dd\\.hh\\:mm\\:ss\\.fffffff";

        internal static readonly string ValueAttributeMetadataProperty = "$";
        internal static readonly string ValueRefMetadataProperty = "#";
        internal static readonly string MetadataInstanceProperty = "~";

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

        #region Blob
        internal static Blob DeserializeBlob(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (!CanonicalFormParser.TryParseCanonicalType(text, DiaType.Blob, TryConvertBlob, out var value))
                throw new InvalidOperationException($"Invalid {DiaType.Blob} format");

            return value.As<Core.Types.Blob>();
        }

        private static bool TryConvertBlob((CharSequence chars, AttributeSet atts) info, out IDiaValue blob)
        {
            try
            {
                blob = info.chars.Equals("null") switch
                {
                    false => Blob.Of(Convert.FromBase64String(info.chars), info.atts),
                    true => Blob.Null(info.atts)
                };
                return true;
            }
            catch
            {
                blob = Blob.Default;
                return false;
            }
        }
        #endregion

        #region Bool
        internal static Core.Types.Boolean DeserializeBool(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (text.StartsWith(BoolPrefix)
                && CanonicalFormParser.TryParseCanonicalType(text, DiaType.Bool, TryConvertBool, out var value))
                return value.As<Core.Types.Boolean>();

            else if (bool.TryParse(text.ToLower(), out var @bool))
                return Core.Types.Boolean.Of(@bool);

            else throw new InvalidOperationException($"Invalid {Core.DiaType.Bool} format");
        }

        private static bool TryConvertBool((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Boolean.Null(info.atts);
                return true;
            }
            else if (bool.TryParse(info.chars, out var @bool))
            {
                value = Core.Types.Boolean.Of(@bool, info.atts);
                return true;
            }
            else
            {
                value = Core.Types.Boolean.Default;
                return false;
            }
        }
        #endregion

        #region Decimal
        internal static Core.Types.Decimal DeserializeDecimal(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (text.StartsWith(DecimalPrefix)
                && CanonicalFormParser.TryParseCanonicalType(text, DiaType.Decimal, TryConvertDecimal, out var value))
                return value.As<Core.Types.Decimal>();

            else if (BigDecimal.TryParse(text.ToLower(), out IResult<BigDecimal> @decimal))
                return Core.Types.Decimal.Of(@decimal.Resolve());

            else throw new InvalidOperationException($"Invalid {Core.DiaType.Decimal} format");
        }

        private static bool TryConvertDecimal((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Decimal.Null(info.atts);
                return true;
            }
            else if (BigDecimal.TryParse(info.chars, out IResult<BigDecimal> result))
            {
                value = Core.Types.Decimal.Of(result.Resolve(), info.atts);
                return true;
            }
            else
            {
                value = Core.Types.Decimal.Default;
                return false;
            }
        }
        #endregion

        #region Duration
        internal static Core.Types.Duration DeserializeDuration(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (CanonicalFormParser.TryParseCanonicalType(text, DiaType.Duration, TryConvertDuration, out var value))
                return value.As<Core.Types.Duration>();

            else throw new InvalidOperationException($"Invalid {Core.DiaType.Duration} format");
        }

        private static bool TryConvertDuration((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Duration.Null(info.atts);
                return true;
            }
            else if (TimeSpan.TryParseExact(info.chars, DurationFormat, null, out var duration))
            {
                value = Core.Types.Duration.Of(duration, info.atts);
                return true;
            }
            else
            {
                value = Core.Types.Duration.Default;
                return false;
            }
        }
        #endregion

        #region Int
        internal static Core.Types.Integer DeserializeInt(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (text.StartsWith(IntPrefix)
                && CanonicalFormParser.TryParseCanonicalType(text, DiaType.Int, TryConvertInt, out var value))
                return value.As<Core.Types.Integer>();

            else if (text.StartsWith("0x")
                && BigInteger.TryParse(text[2..], NumberStyles.HexNumber, null, out var @int))
                return Core.Types.Integer.Of(@int);

            else if (BigInteger.TryParse(text.ToLower(), out @int))
                return Core.Types.Integer.Of(@int);

            else throw new InvalidOperationException($"Invalid {Core.DiaType.Int} format");
        }

        private static bool TryConvertInt((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            var chspan = info.chars.AsSpan();
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Integer.Null(info.atts);
                return true;
            }
            else if (chspan.StartsWith("0x") 
                && BigInteger.TryParse(chspan[2..], NumberStyles.HexNumber, null, out var @int))
            {
                value = Core.Types.Integer.Of(@int, info.atts);
                return true;
            }
            else if (BigInteger.TryParse(chspan, out @int))
            {
                value = Core.Types.Integer.Of(@int, info.atts);
                return true;
            }
            else
            {
                value = Core.Types.Integer.Default;
                return false;
            }
        }
        #endregion

        #region String
        internal static Core.Types.String DeserializeString(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (text.StartsWith(StringPrefix))
            {
                if (CanonicalFormParser.TryParseCanonicalType(text, DiaType.String, TryConvertString, out var value))
                    return value.As<Core.Types.String>();

                else throw new InvalidOperationException(
                    $"Invalid {Core.DiaType.String} format");
            }
            else
            {
                if (text.StartsWith("##"))
                    text = text[1..];

                return Core.Types.String.Of(text);
            }
        }

        private static bool TryConvertString((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.String.Null(info.atts);
                return true;
            }
            else if (info.chars.Length < 2
                || '\'' != info.chars[0]
                || '\'' != info.chars[^1])
            {
                value = Core.Types.String.Default;
                return false;
            }
            else
            {
                try
                {
                    value = StringEscaper
                        .UnescapeString(info.chars[1..^1])
                        .ApplyTo(str => Core.Types.String.Of(str, info.atts));
                    return true;
                }
                catch
                {
                    value = Core.Types.String.Default;
                    return false;
                }
            }
        }
        #endregion

        #region Symbol
        internal static Core.Types.Symbol DeserializeSymbol(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (text.StartsWith(SymbolPrefix)
                && CanonicalFormParser.TryParseCanonicalType(text, DiaType.Symbol, TryConvertSymbol, out var value))
                    return value.As<Symbol>();

            else throw new InvalidOperationException(
                $"Invalid {Core.DiaType.Symbol} format");
        }

        private static bool TryConvertSymbol((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Symbol.Null(info.atts);
                return true;
            }
            else if (info.chars.Length < 2
                || '\'' != info.chars[0]
                || '\'' != info.chars[^1])
            {
                value = Core.Types.Symbol.Default;
                return false;
            }
            else
            {
                try
                {
                    value = StringEscaper
                        .UnescapeString(info.chars[1..^1])
                        .ApplyTo(str => Core.Types.Symbol.Of(str, info.atts));
                    return true;
                }
                catch
                {
                    value = Core.Types.Symbol.Default;
                    return false;
                }
            }
        }
        #endregion

        #region Timestamp
        internal static Core.Types.Timestamp DeserializeTimestamp(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (CanonicalFormParser.TryParseCanonicalType(text, DiaType.Timestamp, TryConvertTimestamp, out var value))
                return value.As<Core.Types.Timestamp>();

            else throw new InvalidOperationException($"Invalid {Core.DiaType.Timestamp} format");
        }

        private static bool TryConvertTimestamp((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Timestamp.Null(info.atts);
                return true;
            }
            else if (DateTimeOffset.TryParseExact(info.chars, TimestampFormat, null, DateTimeStyles.None, out var timestamp))
            {
                value = Core.Types.Timestamp.Of(timestamp, info.atts);
                return true;
            }
            else
            {
                value = Core.Types.Timestamp.Default;
                return false;
            }
        }
        #endregion

        #region Ref
        internal static Ref DeserializeRef(string text, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(context);

            if (CanonicalFormParser.TryParseCanonicalType(text, CanonicalFormParser.RefPrefix, TryConvertRef, out var value))
                return value.As<Ref>();

            else throw new InvalidOperationException($"Invalid Ref format");
        }

        private static bool TryConvertRef((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (uint.TryParse(info.chars[2..].AsSpan(), NumberStyles.HexNumber, null, out var @ref))
            {
                value = new Ref(@ref);
                return true;
            }
            else
            {
                value = null!;
                return false;
            }
        }
        #endregion

        #region Sequence

        internal static Sequence DeserializeSequence(JArray array, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentNullException.ThrowIfNull(context);

            var metadata = GetSequenceMetadata(array);
            var sequence = array
                .Select((item, index) => (item, index))
                .Where(item => ((metadata.Ref.HasValue || !metadata.Attributes.IsDefault), item.index) switch
                {
                    (true, > 0) => true, // if we have metadata, skip the first element
                    (false, _) => true,  // if we don't have metadata, allow all values
                    _ => false
                }) 
                .Aggregate(Sequence.Of(metadata.Attributes), (seq, item) =>
                {
                    var arrayItem = Serializer.Deserialize(item.item, context);
                    if (arrayItem is Ref @ref)
                    {
                        @ref.Parent = seq;
                        @ref.Key = seq.Count;
                        context.AddRef(@ref);
                        arrayItem = Core.Types.String.Of(
                            item.item.Value<string>(),
                            Core.Types.Attribute.Of("ref"));
                    }

                    seq.Add(DiaValue.Of(arrayItem));
                    return seq;
                });

            if (metadata.Ref is uint @ref)
                context.ReferenceMap.GetOrAdd(@ref, _ => sequence);

            return sequence;
        }

        internal static Sequence DeserializeNullSequence(string canonicalSequenceText, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(canonicalSequenceText);
            ArgumentNullException.ThrowIfNull(context);

            if (CanonicalFormParser.TryParseCanonicalType(
                canonicalSequenceText,
                DiaType.Sequence,
                TryConvertNullSequence,
                out var value))
                return value.As<Core.Types.Sequence>();

            else throw new InvalidOperationException($"Invalid {Core.DiaType.Sequence} format");
        }

        internal static (uint? Ref, AttributeSet Attributes) GetSequenceMetadata(JArray array)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (array.IsEmpty())
                return default;

            return array[0] is JObject jobj && TryGetMetadata(jobj, out var metadata)
                ? (metadata.Ref, metadata.AttributeSet)
                : default;
        }

        private static bool TryConvertNullSequence((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Sequence.Null(info.atts);
                return true;
            }
            else
            {
                value = Core.Types.Sequence.Default;
                return false;
            }
        }
        #endregion

        #region Record

        internal static Record DeserializeRecord(JObject jobj, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(jobj);
            ArgumentNullException.ThrowIfNull(context);

            var (Ref, Attributes, PropertyAttributes) = GetRecordMetadata(jobj);
            var record = jobj
                .Properties()
                .Where(property => !MetadataInstanceProperty.Equals(property.Name))
                .Aggregate(Record.Of(Attributes), (rec, prop) =>
                {
                    var attributes = 
                        PropertyAttributes is not null
                        && PropertyAttributes.TryGetValue(prop.Name, out var atts)
                        ? atts : default;

                    var propertyName = Record.PropertyName.Of(prop.Name, attributes);
                    var propertyValue = Serializer.Deserialize(prop.Value, context);
                    if (propertyValue is Ref @ref)
                    {
                        @ref.Parent = rec;
                        @ref.Key = prop.Name;
                        context.AddRef(@ref);
                        propertyValue = Core.Types.String.Of(
                            prop.Value.Value<string>(),
                            Core.Types.Attribute.Of("ref"));
                    }

                    rec[propertyName] = DiaValue.Of(propertyValue);
                    return rec;
                });

            if (Ref is uint @ref)
                context.ReferenceMap.GetOrAdd(@ref, _ => record);

            return record;
        }

        internal static Record DeserializeNullRecord(string canonicalRecordText, DeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(canonicalRecordText);
            ArgumentNullException.ThrowIfNull(context);

            if (CanonicalFormParser.TryParseCanonicalType(
                canonicalRecordText,
                DiaType.Record,
                TryConvertNullRecord,
                out var value))
                return value.As<Record>();

            else throw new InvalidOperationException($"Invalid {DiaType.Record} format");
        }

        internal static (uint? Ref, AttributeSet Attributes, ImmutableDictionary<string, AttributeSet> PropertyAttributes) GetRecordMetadata(JObject jobj)
        {
            ArgumentNullException.ThrowIfNull(jobj);

            if (jobj.Count == 0)
                return default;

            return jobj[MetadataInstanceProperty] is JObject jjobj && TryGetMetadata(jjobj, out var metadata)
                ? metadata
                : default;
        }

        private static bool TryConvertNullRecord((CharSequence chars, AttributeSet atts) info, out IDiaValue value)
        {
            if (info.chars.Equals("null"))
            {
                value = Core.Types.Record.Null(info.atts);
                return true;
            }
            else
            {
                value = Core.Types.Record.Default;
                return false;
            }
        }
        #endregion

        internal static bool TryGetMetadata(
            JObject metadataObject,
            out (uint? Ref, AttributeSet AttributeSet, ImmutableDictionary<string, AttributeSet> PropertyAttributes) metadata)
        {
            ArgumentNullException.ThrowIfNull(metadataObject);

            var hex = NumberStyles.HexNumber;
            metadata = (
                Ref:
                    metadataObject.TryGetValue(ValueRefMetadataProperty, out var refToken)
                    && uint.TryParse(refToken!.Value<string>()?.Trim()[2..] ?? string.Empty, hex, null, out var v)
                    ? v : default(uint?),

                AttributeSet:
                    metadataObject.TryGetValue(ValueAttributeMetadataProperty, out var attToken)
                    && CanonicalFormParser.TryParseAttributes(attToken!.Value<string>() ?? string.Empty, out var info)
                    ? info.Attributes : default,
                    
                PropertyAttributes: metadataObject
                    .Properties()
                    .Where(prop => ValueRefMetadataProperty!= prop.Name && ValueAttributeMetadataProperty != prop.Name)
                    .Where(prop => prop.Name.StartsWith('$'))
                    .ToImmutableDictionary(
                        prop => prop.Name[1..],
                        prop => !CanonicalFormParser.TryParseAttributes(prop.Value.Value<string>()!, out var info)
                            ? throw new InvalidOperationException($"Invalid property attribute [property: {prop.Name}]")
                            : info.Attributes));

            return metadata.Ref.HasValue || !metadata.AttributeSet.IsDefault;
        }

        #region Nested types
        internal record Ref : IDiaValue
        {
            public DiaType Type => (DiaType)99;

            private readonly uint _ref;

            public Ref(uint @ref)
            {
                _ref = @ref;
            }

            public uint Value => _ref;

            public IDiaValue? Parent { get; set; }

            public object? Key { get; set; }

            public void Resolve(ReferenceMap map)
            {
                if (Parent is Sequence seq && Key is int index)
                    seq[index] = DiaValue.Of(map.Dereference(_ref));

                else if (Parent is Record rec && Key is string property)
                    rec[property] = DiaValue.Of(map.Dereference(_ref));

                else throw new InvalidOperationException(
                    $"Unresolvable ref [ref: {_ref:x}, parent: {Parent}, key: {Key}]");
            }
        }
        #endregion
    }
}
