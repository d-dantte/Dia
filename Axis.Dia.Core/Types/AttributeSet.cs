using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    public readonly struct AttributeSet :
        IEnumerable<Attribute>,
        IDefaultContract<AttributeSet>,
        IEquatable<AttributeSet>,
        IValueEquatable<AttributeSet>
    {
        private readonly ImmutableHashSet<Attribute> attributes;

        private IEnumerable<Attribute> Ordered => !IsDefault
            ? attributes.OrderBy(att => $"{att.Key}:{att.Value}")
            : [];

        #region DefaultContract

        public static AttributeSet Default => default;

        public bool IsDefault => attributes is null;
        #endregion

        #region IEnumerable
        public IEnumerator<Attribute> GetEnumerator()
        {
            return IsDefault
                ? Enumerable.Empty<Attribute>().GetEnumerator()
                : Ordered.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region API

        public ImmutableHashSet<string> AttributeKeys => !IsDefault
            ? attributes
                .Select(att => att.Key)
                .ToImmutableHashSet()
            : ImmutableHashSet<string>.Empty;

        public bool IsEmpty => IsDefault || attributes.IsEmpty;

        public bool Contains(
            Attribute attribute)
            => !IsDefault && attributes.Contains(attribute);

        public bool ContainsKey(
            string attributeKey)
            => !IsDefault && AttributeKeys.Contains(attributeKey);

        public int Count => IsDefault ? 0: attributes.Count;

        public bool TryGetAttribute(string key, out Attribute? attribute)
        {
            AssertNonDefault();
            attribute = attributes.FirstOrNull(att => att.Key.Equals(key));
            return attribute is not null;
        }

        public bool TryGetAttributes(string key, out ImmutableArray<Attribute> attributes)
        {
            AssertNonDefault();
            attributes = this.attributes
                .Where(att => att.Key.Equals(key))
                .ToImmutableArray();
            return !attributes.IsEmpty; 
        }

        private void AssertNonDefault()
        {
            if (IsDefault)
                throw new InvalidOperationException("Invalid instance: default");
        }
        #endregion

        #region overrides
        /// <summary>
        /// Performs a reference-equals operation
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AttributeSet other)
        {
            return EqualityComparer<ImmutableHashSet<Attribute>>.Default.Equals(
                attributes,
                other.attributes);
        }

        public bool ValueEquals(AttributeSet other)
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
            return !IsDefault
                ? Ordered.Aggregate(0, HashCode.Combine)
                : 0;
        }

        public int ValueHash() => GetHashCode();

        public static bool operator ==(
            AttributeSet left,
            AttributeSet right)
            => left.ValueEquals(right);

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
