using Axis.Luna.Result;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Json.Path
{
    public readonly struct Index :
        ISegment,
        IEquatable<Index>,
        IComparable<Index>,
        IResultParsable<Index>
    {
        internal static readonly char IndexNotationPrefixChar = ':';

        private readonly int value;

        public int Value => value;

        public bool IsDefault => value == 0;

        public string Notation => $"{IndexNotationPrefixChar}{value}";

        private Index(int index)
        {
            value = index;
        }

        public static Index Of(int index) => new(index);

        public static implicit operator Index(int index) => new(index);
        public static implicit operator Index(string notation) => Parse(notation).Resolve();

        #region IResultParsable<Index>
        public static bool TryParse(string text, out IResult<Index> result)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);

            var trimmed = text.Trim();

            if (!IndexNotationPrefixChar.Equals(trimmed[0]))
            {
                result = Result.Of<Index>(
                    new FormatException($"Invalid prefix-char: {trimmed[0]}"));
                return false;
            }
            else if (int.TryParse(trimmed[1..], out var index))
            {
                result = Result.Of<Index>(index);
                return true;
            }
            else
            {
                result = Result.Of<Index>(
                    new FormatException($"Invalid index value: {trimmed[1..]}"));
                return false;
            }
        }

        public static IResult<Index> Parse(string text)
        {
            _ = TryParse(text, out var result);
            return result;
        }
        #endregion

        #region IEquatable<Index>
        public bool Equals(Index other)
        {
            return value == other.value;
        }
        #endregion

        #region IComparable
        public int CompareTo(Index other)
        {
            return value.CompareTo(other.value);
        }
        #endregion

        #region Overrides
        public override string ToString() => Notation;

        public override int GetHashCode() => value.GetHashCode();

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Index indx && Equals(indx);

        public static bool operator ==(Index left, Index right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Index left, Index right)
        {
            return !(left == right);
        }

        public static bool operator <(Index left, Index right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Index left, Index right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Index left, Index right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Index left, Index right)
        {
            return left.CompareTo(right) >= 0;
        }
        #endregion
    }
}
