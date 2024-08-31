using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Types
{
    public readonly struct Attribute:
        IDiaType,
        IDefaultContract<Attribute>,
        IEquatable<Attribute>
    {
        internal static readonly Regex KeyPattern = new(
            "^[a-zA-Z_](([.-])?[a-zA-Z0-9_])*\\z",
            RegexOptions.Compiled);

        public DiaType Type => DiaType.Attribute;

        public string Key { get; }

        public string? Value { get; }

        public bool HasValue => Value is not null;

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
            params string[] attributes)
            => attributes.Select(Of).ToArray();

        public static Attribute[] Of(
            params (string key, string? value)[] attributes)
            => attributes.Select(tuple => Of(tuple.key, tuple.value)).ToArray();


        public static implicit operator Attribute(string key) => Of(key);

        public static implicit operator Attribute((string key, string? value) tuple) => Of(tuple.key, tuple.value);
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[@{Type} {Key}:{Value}]";
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

    public readonly struct AttributeSet :
        IEnumerable<Attribute>,
        IDefaultContract<AttributeSet>,
        IEquatable<AttributeSet>
    {
        private readonly ImmutableHashSet<Attribute> attributes;

        #region DefaultContract

        public static AttributeSet Default => default;

        public bool IsDefault => attributes is null;
        #endregion

        #region IEnumerable
        public IEnumerator<Attribute> GetEnumerator()
        {
            return IsDefault
                ? Enumerable.Empty<Attribute>().GetEnumerator()
                : attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region API

        public ImmutableHashSet<string> AttributeKeys => attributes
            .Select(att => att.Key)
            .ToImmutableHashSet();

        public bool IsEmpty => IsDefault || attributes.Count == 0;

        public bool Contains(
            Attribute attribute)
            => !IsDefault && attributes.Contains(attribute);

        public bool ContainsKey(
            string attributeKey)
            => !IsDefault && AttributeKeys.Contains(attributeKey);

        public int Count => IsDefault ? 0: attributes.Count;

        public bool TryGetAttribute(string key, out Attribute? attribute)
        {
            attribute = attributes.FirstOrNull(att => att.Key.Equals(key));
            return attribute is not null;
        }

        public bool TryGetAttributes(string key, out ImmutableArray<Attribute> attributes)
        {
            attributes = this.attributes
                .Where(att => att.Key.Equals(key))
                .ToImmutableArray();
            return !attributes.IsEmpty; 
        }
        #endregion

        #region overrides
        public bool Equals(AttributeSet other)
        {
            if (IsDefault && other.IsDefault)
                return true;

            if (IsDefault ^ other.IsDefault)
                return false;

           return attributes.Count == other.attributes.Count
                && attributes.SetEquals(other.attributes);
        }

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is AttributeSet other && Equals(other);

        public override int GetHashCode()
        {
            return IsDefault ? 0 : attributes
                .OrderBy(att => $"{att.Key}:{att.Value}")
                .Aggregate(0, HashCode.Combine);
        }

        public static bool operator ==(
            AttributeSet left,
            AttributeSet right)
            => left.Equals(right);

        public static bool operator !=(
            AttributeSet left,
            AttributeSet right)
            => !(left == right);

        #endregion

        #region Construction
        public AttributeSet(params Attribute[] attributes)
            : this(attributes.AsEnumerable())
        {
        }

        public AttributeSet(IEnumerable<Attribute> attributes)
        {
            this.attributes = attributes
                .ThrowIfNull(() => new ArgumentNullException(nameof(attributes)))
                .ThrowIfAny(
                    att => att.IsDefault,
                    _ => new ArgumentException($"Invalid attribute: default"))
                .ToImmutableHashSet();
        }

        public static AttributeSet Of(
            params Attribute[] attributes)
            => new(attributes);

        public static AttributeSet Of(
            IEnumerable<Attribute> attributes)
            => new(attributes);

        public static implicit operator AttributeSet(
            Attribute[] attributes)
            => new(attributes);
        #endregion
    }
}
