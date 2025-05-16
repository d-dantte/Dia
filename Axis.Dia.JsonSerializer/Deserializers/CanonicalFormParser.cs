using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Common;

namespace Axis.Dia.Json.Deserializers
{
    using AttributeInfo = (Core.Types.AttributeSet Attributes, int Count);

    internal delegate bool TryOperation<TIn, TOut>(TIn @in, out TOut @out);

    /// <summary>
    /// Parses attributes in the form '[@abc; @xyz:value;  @x12:val\;ue;]'
    /// </summary>
    internal class CanonicalFormParser
    {
        #region prefixes
        public static readonly string BlobPrefix = "#Blob";
        public static readonly string BoolPrefix = "#Bool";
        public static readonly string TimestampPrefix = "#Timestamp";
        public static readonly string DurationPrefix = "#Duration";
        public static readonly string SymbolPrefix = "#Symbol";
        public static readonly string StringPrefix = "#String";
        public static readonly string IntPrefix = "#Int";
        public static readonly string DecimalPrefix = "#Decimal";
        public static readonly string SequencePrefix = "#Sequence";
        public static readonly string RecordPrefix = "#Record";
        public static readonly string RefPrefix = "#Ref";
        #endregion

        internal static string ToPrefix(DiaType type)
        {
            return type switch
            {
                DiaType.Bool => BoolPrefix,
                DiaType.Blob => BlobPrefix,
                DiaType.Decimal => DecimalPrefix,
                DiaType.Duration => DurationPrefix,
                DiaType.Int => IntPrefix,
                DiaType.Record => RecordPrefix,
                DiaType.Sequence => SequencePrefix,
                DiaType.String => StringPrefix,
                DiaType.Symbol => SymbolPrefix,
                DiaType.Timestamp => TimestampPrefix,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(type), $"Invalid type: undefined")
            };
        }

        public static bool TryParseCanonicalType(
            CharSequenceReader reader,
            Core.DiaType diaType,
            TryOperation<(CharSequence, AttributeSet), IDiaValue> mapper,
            out IDiaValue value)
            => TryParseCanonicalType(reader, ToPrefix(diaType), mapper, out value);

        public static bool TryParseCanonicalType(
            CharSequenceReader reader,
            string typePrefix,
            TryOperation<(CharSequence, AttributeSet), IDiaValue> mapper,
            out IDiaValue value)
        {
            ArgumentNullException.ThrowIfNull(reader);
            ArgumentNullException.ThrowIfNull(mapper);
            ArgumentException.ThrowIfNullOrWhiteSpace(typePrefix);

            var index = reader.CurrentIndex;
            value = default!;

            // Do we have the prefix?
            if (!reader.TryRead(typePrefix.Length, out var prefix)
                || !prefix.Equals(typePrefix))
                return Fail(reader, index);

            // null?
            var nullLiteral = ".null";
            var tempIndex = reader.CurrentIndex;
            if (reader.TryRead(nullLiteral.Length, out var nullValue)
                && !nullValue.Equals(nullLiteral))
            {
                reader.Reset(tempIndex);
                nullValue = default;
            }

            // whitespace?
            _ = TryParseLineSpace(reader, out _);

            // attributes?
            if (!TryParseAttributes(reader, out var attInfo))
                attInfo = default;

            // whitespace?
            _ = TryParseLineSpace(reader, out _);

            // value
            var valueCharacters = nullValue.IsDefault
                ? reader.CharSequence[reader.CurrentIndex..]
                : nullValue[1..];

            if (!mapper.Invoke((valueCharacters, attInfo.Attributes), out value))
                return Fail(reader, index);

            return true;
        }

        public static bool TryParseAttributes(CharSequenceReader reader, out AttributeInfo info)
        {
            ArgumentNullException.ThrowIfNull(reader);

            info = default;
            var index = reader.CurrentIndex;

            if (!reader.CanRead)
                return false;

            // first char is '['
            if (!reader.TryReadExactly(1, out var chr) && '[' != chr[0])
                return false;

            var atts = new List<Core.Types.Attribute>();
            while (TryParseAttribute(reader, out var attribute))
            {
                atts.Add(attribute!.Value);

                // optional space between attributes
                _ = TryParseLineSpace(reader, out _);
            }

            if (!reader.CanRead
                || (reader.TryReadExactly(1, out chr) && ']' != chr[0]))
                return Fail(reader, index);

            info = (
                Attributes: Core.Types.AttributeSet.Of(atts),
                Count: reader.CurrentIndex - index);

            return true;
        }

        public static bool TryParseLineSpace(CharSequenceReader reader, out CharSequence lineSpace)
        {
            ArgumentNullException.ThrowIfNull(reader);

            lineSpace = default;
            while (reader.TryReadExactly(1, IsLineSpace, out var chars))
            {
                if (lineSpace.IsDefault)
                    lineSpace = chars;

                else lineSpace = lineSpace += 1;
            }

            return !lineSpace.IsDefault;
        }

        public static bool TryParseAttribute(CharSequenceReader reader, out Core.Types.Attribute? attribute)
        {
            ArgumentNullException.ThrowIfNull(reader);

            var index = reader.CurrentIndex;
            attribute = null;

            // read '@'
            if (!reader.TryReadExactly(1, IsAtSymbol, out _))
                return Fail(reader, index);

            // read key
            var delims = new HashSet<char> { ':', ';' };
            if (!reader.TryRead(chars => !delims.Contains(chars[^1]), out var key)
                || !Core.Types.Attribute.KeyPattern.IsMatch(key.AsSpan()))
                return Fail(reader, index);

            // read delim
            if (!reader.TryRead(1, chars => delims.Contains(chars[0]), out var delim))
                return Fail(reader, index);

            // delim = ';' 
            if (';'.Equals(delim[0]))
            {
                attribute = Core.Types.Attribute.Of(key.ToString());
                return true;
            }

            // delim is ':', read value
            if (!reader.TryRead(IsAttributeValue, out var value))
                return Fail(reader, index);

            // read ';'
            if (!reader.TryReadExactly(1, chars => ';'.Equals(chars[0]), out delim))
                return Fail(reader, index);

            attribute = Core.Types.Attribute.Of(key.ToString(), value.ToString().Replace("\\;", ";"));
            return true;
        }

        private static bool Fail(CharSequenceReader reader, int resetIndex)
        {
            _ = reader.Reset(resetIndex);
            return false;
        }

        private static bool IsLineSpace(CharSequence @char) => @char.All(char.IsWhiteSpace);

        private static bool IsAtSymbol(CharSequence @char) => @char.Length == 1 && '@'.Equals(@char[0]);

        private static bool IsAttributeValue(CharSequence @chars)
        {
            // If the last char is the end delimiter ';' and the previous char isn't an escape char, pass
            // else fail
            return !';'.Equals(chars[^1])
                || '\\'.Equals(chars[^2]);
        }
    }
}
