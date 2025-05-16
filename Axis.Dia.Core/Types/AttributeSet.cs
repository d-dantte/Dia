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
        private readonly ImmutableHashSet<Attribute>? _attributes;

        private IEnumerable<Attribute> Ordered => !IsDefault
            ? _attributes!.OrderBy(att => $"{att.Key}:{att.Value}")
            : [];

        #region DefaultContract

        public static AttributeSet Default => default;

        public bool IsDefault => _attributes is null;
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
            ? _attributes
                .Select(att => att.Key)
                .ToImmutableHashSet()
            : ImmutableHashSet<string>.Empty;

        public bool IsEmpty => IsDefault || _attributes.IsEmpty;

        public bool Contains(
            Attribute attribute)
            => !IsDefault && _attributes.Contains(attribute);

        public bool ContainsKey(
            string attributeKey)
            => !IsDefault && AttributeKeys.Contains(attributeKey);

        public int Count => IsDefault ? 0: _attributes.Count;

        public bool TryGetAttribute(string key, out Attribute? attribute)
        {
            AssertNonDefault();
            attribute = _attributes.FirstOrNull(att => att.Key.Equals(key));
            return attribute is not null;
        }

        public bool TryGetAttributes(string key, out ImmutableArray<Attribute> attributes)
        {
            AssertNonDefault();
            attributes = this._attributes
                .Where(att => att.Key.Equals(key))
                .ToImmutableArray();
            return !attributes.IsEmpty; 
        }

        public Core.Types.Attribute this[Index index]
        {
            get
            {
                AssertNonDefault();
                return _attributes.ToArray()[index];
            }
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
                _attributes,
                other._attributes);
        }

        public bool ValueEquals(AttributeSet other)
        {
            if (IsDefault && other.IsDefault)
                return true;

            if (IsDefault ^ other.IsDefault)
                return false;

           return _attributes.Count == other._attributes.Count
                && _attributes.SetEquals(other._attributes);
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
        {
            if (attributes is null || attributes.Length == 0)
                _attributes = null;


            else _attributes = [.. attributes
                .ThrowIfAny(
                    att => att.IsDefault,
                    _ => new ArgumentException($"Invalid attribute: default"))];
        }

        public AttributeSet(IEnumerable<Attribute> attributes)
            : this([.. attributes])
        {
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

        public static implicit operator Attribute[](
            AttributeSet attributes)
            => [.. attributes._attributes ?? []];

        public static implicit operator ImmutableArray<Attribute>(
            AttributeSet attributes)
            => [.. attributes._attributes ?? []];
        #endregion
    }
}
