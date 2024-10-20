using Axis.Luna.Extensions;
using Axis.Luna.Result;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Json.Path
{
    public readonly struct ValuePath :
        IEquatable<ValuePath>,
        IResultParsable<ValuePath>
    {
        private readonly ImmutableArray<ISegment> segments;

        public ValuePath(params string[] segments)
        {
            ArgumentNullException.ThrowIfNull(segments);

            this.segments = segments
                .Select(ParseSegment)
                .ToImmutableArray();
        }

        public ValuePath(params ISegment[] segments)
        {
            ArgumentNullException.ThrowIfNull(segments);

            this.segments = segments
                .ThrowIfAny(
                    s => s is null,
                    _ => new ArgumentException($"Invalid segment: null"))
                .ToImmutableArray();
        }

        public static ValuePath Of(string notation) => "*".Equals(notation)
            ? default
            : Parse(notation).Resolve();

        public static ValuePath Of(params ISegment[] segments) => new(segments);

        public static ValuePath Of(params string[] segments) => new(segments);

        public static implicit operator ValuePath(string notation) => Of(notation);

        internal static ISegment ParseSegment(string segment)
        {
            if (string.IsNullOrEmpty(segment))
                throw new ArgumentException($"Invalid segment: null/empty");

            return (segment.Length, segment[0]) switch
            {
                (>= 2, ':') => Index.Parse(segment).Resolve(),
                (>= 2, '@') => Property.Parse(segment).Resolve(),
                _ => throw new FormatException($"Invalid segment: '{segment}'")
            };
        }

        #region Api
        public ValuePath Append(string segment) => Append(ParseSegment(segment));

        public ValuePath Append(ISegment segment)
        {
            ArgumentNullException.ThrowIfNull(segment);

            return segments.IsDefault switch
            {
                false => segments
                    .Append(segment)
                    .ToArray()
                    .ApplyTo(Of),

                true => new(segment)
            };
        }
        #endregion

        #region IEquatable
        public bool Equals(ValuePath other)
        {
            if (segments.IsDefault && other.segments.IsDefault)
                return true;

            if (segments.IsDefault ^ other.segments.IsDefault)
                return false;

            return segments.SequenceEqual(other.segments);
        }
        #endregion

        #region IResultParsable<ValuePath>
        public static bool TryParse(string text, out IResult<ValuePath> result)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            result = Result
                .Of(text)
                .Map(Split)
                .Map(parts => new ValuePath(parts.ToArray()));

            return result.IsDataResult();
        }

        internal static IEnumerable<string> Split(string text)
        {
            var offset = 1;
            for (int cnt = 0; cnt < text.Length; cnt++)
            {
                var @char = text[cnt];

                if('/'.Equals(@char))
                {
                    if (cnt == 0)
                        continue;

                    if ('\\'.Equals(text[cnt - 1]))
                        continue;

                    yield return text[offset..cnt];
                    offset = cnt + 1;
                }
            }

            if (offset < text.Length)
                yield return text[offset..];
        }

        public static IResult<ValuePath> Parse(string text)
        {
            _ = TryParse(text, out var result);
            return result;
        }
        #endregion

        #region Override
        public override string ToString()
        {
            return segments.IsDefault switch
            {
                true => "*",
                _ => $"/{segments.Select(s => s.Notation).JoinUsing("/")}"
            };
        }

        public override int GetHashCode()
        {
            return segments.IsDefault switch
            {
                true => 0,
                _ => segments.Aggregate(0, HashCode.Combine)
            };
        }

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is ValuePath other && Equals(other);

        public static bool operator ==(ValuePath left, ValuePath right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValuePath left, ValuePath right)
        {
            return !(left == right);
        }
        #endregion
    }
}
