using Axis.Luna.Extensions;
using Axis.Luna.Result;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Json.Path
{
    public readonly struct Property :
        ISegment,
        IEquatable<Property>,
        IResultParsable<Property>
    {
        internal static readonly char PropertyNotationPrefixChar = '@';

        private readonly string name;

        public string Name => name;

        public bool IsDefault => name is null;

        public string Notation => name switch
        {
            string n => $"{PropertyNotationPrefixChar}{n.Replace("/", "\\/")}",
            null => "*"
        };

        private Property(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            this.name = name;
        }

        public static Property Of(string notation) => Parse(notation).Resolve();

        public static implicit operator Property(string notation) => Of(notation);

        internal static string UnescapeString(string @string)
        {
            if (@string is null)
                return @string!;

            return @string.Replace("\\/", "/");
        }

        #region IResultParsable<Property>
        public static bool TryParse(string text, out IResult<Property> result)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);

            var trimmed = text.Trim();

            if (!PropertyNotationPrefixChar.Equals(trimmed[0]))
            {
                result = Result.Of<Property>(
                    new FormatException($"Invalid prefix-char: {trimmed[0]}"));
                return false;
            }
            else
            {
                result = Result
                    .Of(trimmed[1..])
                    .Map(UnescapeString)
                    .Map(txt => new Property(txt));
                return true;
            }
        }

        public static IResult<Property> Parse(string text)
        {
            _ = TryParse(text, out var result);
            return result;
        }
        #endregion

        #region IEquatable<Property>
        public bool Equals(Property other)
        {
            return EqualityComparer<string>.Default.Equals(name, other.name);
        }
        #endregion

        #region Overrides
        public override string ToString() => Notation;

        public override int GetHashCode() => HashCode.Combine(name);

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Property indx && Equals(indx);

        public static bool operator ==(Property left, Property right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Property left, Property right)
        {
            return !(left == right);
        }
        #endregion
    }
}
