using Axis.Dia.Core.Contracts;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    public readonly struct Blob :
        IEquatable<Blob>,
        INullContract<Blob>,
        IValueEquatable<Blob>,
        IDefaultContract<Blob>,
        IStructValue<ImmutableArray<byte>>
    {
        private readonly ImmutableArray<byte>? _value;
        private readonly AttributeSet _attributes;

        #region Construction

        public Blob(
            IEnumerable<byte>? value,
            params Attribute[] attributes)
        {
            _value = value?.ToImmutableArray();
            _attributes = attributes;
        }

        public static Blob Of(
            IEnumerable<byte>? value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static Blob Of(
            params byte[] bytes)
            => new(bytes, []);

        public static implicit operator Blob(byte[]? value) => new(value);

        public static implicit operator Blob(List<byte>? value) => new(value);

        public static implicit operator Blob(ImmutableArray<byte>? value) => new(value);

        #endregion

        #region DefaultContract
        public static Blob Default => default;

        public bool IsDefault
            => _value is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Blob Null(params Types.Attribute[] attributes) => new(null, attributes);

        public bool IsNull => _value is null;
        #endregion

        #region IStructValue

        public ImmutableArray<byte>? Value => _value;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Blob;

        #endregion

        #region Equatable

        public bool Equals(Blob other)
        {
            return EqualityComparer<ImmutableArray<byte>?>.Default.Equals(
                _value,
                other._value);
        }
        #endregion

        #region IValueEquatable
        public bool ValueEquals(Blob other)
        {
            if (IsNull ^ other.IsNull)
                return false;

            if (!_attributes.ValueEquals(other.Attributes))
                return false;

            if (IsNull && other.IsNull)
                return true;

            return _value!.Value.SequenceEqual(other.Value!.Value);
        }

        public int ValueHash()
        {
            var attHash = _attributes.Aggregate(0, HashCode.Combine);
            var valueHash = _value?
                .Aggregate(0, HashCode.Combine)
                ?? 0;

            return HashCode.Combine(attHash, valueHash);
        }
        #endregion

        #region overrides

        public override string ToString()
        {
            return $"[@{Type} length: {_value?.Length.ToString() ?? "*"}]";
        }

        public override int GetHashCode() => ValueHash();

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Blob other && ValueEquals(other);

        public static bool operator ==(Blob left, Blob right) => left.ValueEquals(right);

        public static bool operator !=(Blob left, Blob right) => !left.ValueEquals(right);

        #endregion

        public bool IsEmpty => _value?.IsEmpty ?? true;
    }
}
