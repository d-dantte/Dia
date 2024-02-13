using Axis.Dia.Core.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Types
{
    public readonly struct Attribute:
        IDiaType,
        IDefaultContract<Attribute>,
        IEquatable<Attribute>
    {
        private static readonly Regex KeyPattern = new(
            "^[a-zA-Z_](([.-])?[a-zA-Z0-9_])*\\z",
            RegexOptions.Compiled);

        public DiaType Type { get; }

        public string Key { get; }

        public string? Value { get; }

        public bool IsDefault => Key is null && Value is null;

        public static Attribute Default => default;

        #region Construction
        public Attribute(string key, string? value)
        {
            Key = key
                .ThrowIf(
                    string.IsNullOrEmpty,
                    _ => new ArgumentException("Invalid key: null/empty"))
                .ThrowIfNot(
                    KeyPattern.IsMatch,
                    _ => new FormatException(
                        $"Invalid key format: keys must match the patter '{KeyPattern}'"));

            Value = value;
        }

        public Attribute(string key)
        : this(key, null) { }

        public static Attribute Of(string key, string? value) => new(key, value);

        public static Attribute Of(string key) => new(key);

        public static Attribute[] Of(
            params string[] symbols)
            => symbols.Select(Of).ToArray();

        public static Attribute[] Of(
            params (string key, string? value)[] symbols)
            => symbols.Select(tuple => Of(tuple.key, tuple.value)).ToArray();


        public static implicit operator Attribute(string key) => Of(key);

        public static implicit operator Attribute((string key, string? value) tuple) => Of(tuple.key, tuple.value);
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[@{Type}, {Key}:{Value}]";
        }

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Attribute other && Equals(other);

        public bool Equals(
            Attribute other)
            => EqualityComparer<string>.Default.Equals(Key, other.Key)
            && EqualityComparer<string>.Default.Equals(Value, other.Value);

        public override int GetHashCode() => HashCode.Combine(Key, Value);

        public static bool operator ==(Attribute left, Attribute right) => left.Equals(right);

        public static bool operator !=(Attribute left, Attribute right) => !(left == right);
        #endregion
    }
}
