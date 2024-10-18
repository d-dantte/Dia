using Axis.Luna.Common;

namespace Axis.Dia.Json.Deserializers
{
    internal class AttributeParser
    {
        public static bool TryParseAttributes(CharSequenceReader reader, out Core.Types.Attribute[]? attributes)
        {
            ArgumentNullException.ThrowIfNull(reader);

            var index = reader.CurrentIndex;
            var atts = new List<Core.Types.Attribute>();
            while (TryParseAttribute(reader, out var attribute))
            {
                atts.Add(attribute!.Value);

                // optional space between attributes
                _ = TryParseLineSpace(reader, out _);
            }

            // Did we exhaust the reader?
            if (reader.CanRead)
            {
                attributes = null;
                _ = reader.Reset(index);
                return false;
            }

            attributes = atts.ToArray();
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

                else lineSpace = lineSpace.Expand(1);
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

        private static bool IsLineSpace(
            CharSequence @char)
            => @char.All(char.IsWhiteSpace);

        private static bool IsAtSymbol(
            CharSequence @char)
            => @char.Length == 1 && '@'.Equals(@char[0]);

        private static bool IsAttributeValue(CharSequence @chars)
        {
            // If the last char is the end delimiter ';' and the previous char isn't an escape char, pass
            // else fail
            return !';'.Equals(chars[^1])
                || '\\'.Equals(chars[^2]);
        }
    }

    internal class CharSequenceReader
    {
        private int index = 0;
        private readonly CharSequence text;

        public int CurrentIndex => index;

        public bool CanRead => index < text.Length;

        public CharSequenceReader(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            this.text = text;
        }

        public static implicit operator CharSequenceReader(string text) => new(text);

        public CharSequenceReader Reset(int index = 0)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);

            this.index = index;
            return this;
        }

        public CharSequenceReader Back(int steps = 1)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(steps, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(steps, index);

            index -= steps;
            return this;
        }

        public CharSequenceReader Advance(int steps = 1)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(steps, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(steps, text.Length - index);

            index += steps;
            return this;
        }

        #region Read
        /// <summary>
        /// Reads exactly <paramref name="count"/> characters from the unerlying string, at the current index.
        /// If <paramref name="count"/> characters cannot be read, the Buffer's index is not advanced.
        /// </summary>
        /// <param name="count">Number of characters to be read</param>
        /// <param name="chars">Characters that were read</param>
        /// <returns>True if exactly <paramref name="count"/> characters were read, false otherwise.</returns>
        public bool TryReadExactly(int count, out CharSequence chars)
        {
            if (TryPeekExactly(count, out chars))
            {
                index += chars.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads at most <paramref name="maxReadCount"/> number of characters from the underlying string, at the current index.
        /// If read was successful, advance the buffer's index by however many characters were read.
        /// </summary>
        /// <param name="maxReadCount">Maximum number of characters to be read</param>
        /// <param name="chars">The characters that were read</param>
        /// <returns>True if at least 1 character (up to <paramref name="maxReadCount"/>) was read, false otherwise.</returns>
        public bool TryRead(int maxReadCount, out CharSequence chars)
        {
            if (TryPeek(maxReadCount, out chars))
            {
                index += chars.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// In addition to reading the characters, invoke the predicate.
        /// </summary>
        /// <param name="count">Exact number of characters to be read</param>
        /// <param name="predicate">The predicate to invoke on successful read</param>
        /// <param name="chars">The characters that were read</param>
        /// <returns>True if both read and predicate succeeded, False otherwise</returns>
        public bool TryReadExactly(
            int count,
            Func<CharSequence, bool> predicate,
            out CharSequence chars)
        {
            if (TryPeekExactly(count, predicate, out chars))
            {
                index += chars.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// In addition to reading the characters, invoke the predicate.
        /// </summary>
        /// <param name="maxReadCount">Max number of characters to be read</param>
        /// <param name="predicate">The predicate to invoke on successful read</param>
        /// <param name="chars">The characters that were read</param>
        /// <returns>True if both read and predicate succeeded, False otherwise</returns>
        public bool TryRead(
            int maxReadCount,
            Func<CharSequence, bool> predicate,
            out CharSequence chars)
        {
            if (TryPeek(maxReadCount, predicate, out chars))
            {
                index += chars.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Continually expands the target sequence, calling <paramref name="openPattern"/> on every expansion, till it returns false,
        /// then assigns the passing sequence to <paramref name="chars"/>.
        /// </summary>
        /// <param name="openPattern">The predicate to apply to the expanding sequence</param>
        /// <param name="chars">The passing sequence</param>
        /// <returns>True if we were able to match at least one char, false otherwise</returns>
        public bool TryRead(
            Func<CharSequence, bool> openPattern,
            out CharSequence chars)
        {
            if (TryPeek(openPattern, out chars))
            {
                index += chars.Length;
                return true;
            }

            return false;
        }
        #endregion

        #region Peek
        /// <summary>
        /// Peeks exactly <paramref name="count"/> characters from the unerlying string, at the current index.
        /// </summary>
        /// <param name="count">Number of characters to be peeked</param>
        /// <param name="chars">Characters that were peeked</param>
        /// <returns>True if exactly <paramref name="count"/> characters were peeked, false otherwise.</returns>
        public bool TryPeekExactly(int count, out CharSequence chars)
        {
            chars = default;

            if (count > text.Length - index)
                return false;

            chars = text[index..(index + count)];
            return true;
        }

        /// <summary>
        /// Peeks at most <paramref name="maxPeekCount"/> number of characters from the underlying string, at the current index.
        /// </summary>
        /// <param name="maxPeekCount">Maximum number of characters to be peeked</param>
        /// <param name="chars">The characters that were peeked</param>
        /// <returns>True if at least 1 character (up to <paramref name="maxPeekCount"/>) was peeked, false otherwise.</returns>
        public bool TryPeek(int maxPeekCount, out CharSequence chars)
        {
            chars = default;
            var maxCount = text.Length - index;

            if (maxCount == 0)
                return false;

            if (maxPeekCount > maxCount)
                maxPeekCount = maxCount;

            chars = text[index..(index + maxPeekCount)];
            return true;
        }

        /// <summary>
        /// In addition to successfully peeking <paramref name="count"/> characters, invoke the given predicate.
        /// </summary>
        /// <param name="count">The number of characters to be peeked</param>
        /// <param name="predicate">The predicate to invoke if the peek operation was successful</param>
        /// <param name="chars">The characters that were peeked. Only ever <c>default</c> if the peek operation failed</param>
        /// <returns>True if peek and the predicate were both successful, False otherwise</returns>
        public bool TryPeekExactly(
            int count,
            Func<CharSequence, bool> predicate,
            out CharSequence chars)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return TryPeekExactly(count, out chars) && predicate.Invoke(chars);
        }

        /// <summary>
        /// In addition to successfully peeking <paramref name="count"/> characters, invoke the given predicate.
        /// </summary>
        /// <param name="count">The max number of characters to be peeked</param>
        /// <param name="predicate">The predicate to invoke if the peek operation was successful</param>
        /// <param name="chars">The characters that were peeked. Only ever <c>default</c> if the peek operation failed</param>
        /// <returns>True if peek and the predicate were both successful, False otherwise</returns>
        public bool TryPeek(
            int maxReadCount,
            Func<CharSequence, bool> predicate,
            out CharSequence chars)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return TryPeek(maxReadCount, out chars) && predicate.Invoke(chars);
        }

        /// <summary>
        /// Continually expands the target sequence, calling <paramref name="openPattern"/> on every expansion, till it returns false,
        /// then assigns the passing sequence to <paramref name="chars"/>.
        /// </summary>
        /// <param name="openPattern">The predicate to apply to the expanding sequence</param>
        /// <param name="chars">The passing sequence</param>
        /// <returns>True if we were able to match at least one char, false otherwise</returns>
        public bool TryPeek(
            Func<CharSequence, bool> openPattern,
            out CharSequence chars)
        {
            ArgumentNullException.ThrowIfNull(openPattern);

            var seq = text[index..index];
            int count = 0;
            while (TryPeekExactly(count + 1, out var peeked)
                && openPattern.Invoke(peeked))
            {
                count++;
                seq = seq.Expand(1);
            }

            if (seq.Length > 0)
            {
                chars = seq;
                return true;
            }

            chars = default;
            return false;
        }
        #endregion
    }

    internal static class CharSequenceExtension
    {
        public static CharSequence Expand(this CharSequence chars, int charCount)
        {
            if (charCount < 0)
                return chars.Contract(Math.Abs(charCount));

            else if (charCount + chars.Segment.Offset + chars.Segment.Count <= chars.Ref.Length)
                return CharSequence.Of(chars.Ref, chars.Segment.Offset, chars.Segment.Count + charCount);

            else throw new ArgumentOutOfRangeException(
                nameof(charCount),
                "Expanding beyond the limit of the sequence is forbidden");
        }

        public static CharSequence Contract(this CharSequence chars, int charCount)
        {
            if (charCount < 0)
                return chars.Expand(Math.Abs(charCount));

            else if (charCount < chars.Segment.Count)
                return CharSequence.Of(chars.Ref, chars.Segment.Offset, chars.Segment.Count - charCount);

            else throw new ArgumentOutOfRangeException(
                nameof(charCount),
                "Subtracting beyond the limit of this sequence is forbidden");
        }
    }
}
