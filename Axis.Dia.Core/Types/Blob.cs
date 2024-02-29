using Axis.Dia.Core.Utils;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    public readonly struct Blob :
        IStructValue<ImmutableArray<byte>>,
        IEquatable<Blob>,
        INullContract<Blob>,
        IDefaultContract<Blob>
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

        public static implicit operator Blob(
            byte[]? value)
            => new(value);

        public static implicit operator Blob(
            List<byte>? value)
            => new(value);

        #endregion

        #region DefaultContract
        public static Blob Default => default;

        public bool IsDefault
            => _value is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Blob Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _value is null;
        #endregion

        #region IStructValue

        public ImmutableArray<byte>? Value => _value;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Blob;

        #endregion

        #region Equatable

        public override string ToString()
        {
            return $"[@{Type} length: {_value?.Length.ToString() ?? "*"}]";
        }

        public bool Equals(
            Blob other)
        {
            if (IsNull ^ other.IsNull)
                return false;

            if (!_attributes.Equals(other.Attributes))
                return false;

            if (IsNull && other.IsNull)
                return true;

            return _value!.Value.SequenceEqual(other.Value!.Value);
        }
        #endregion

        #region overrides
        public override int GetHashCode()
        {
            var attHash = _attributes.Aggregate(0, HashCode.Combine);
            var valueHash = _value?
                .Aggregate(0, HashCode.Combine)
                ?? 0;

            return HashCode.Combine(attHash, valueHash);
        }

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Blob other && Equals(other);

        public static bool operator ==(
            Blob left,
            Blob right)
            => left.Equals(right);

        public static bool operator !=(
            Blob left,
            Blob right)
            => !left.Equals(right);

        #endregion
    }
}
