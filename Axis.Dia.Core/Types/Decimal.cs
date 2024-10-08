using Axis.Dia.Core.Contracts;
using Axis.Luna.Numerics;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    /// <summary>
    /// TODO: support NAN, and +/-Infinity
    /// </summary>
    public readonly struct Decimal :
        IStructValue<BigDecimal>,
        IEquatable<Decimal>,
        INullContract<Decimal>,
        IValueEquatable<Decimal>,
        IDefaultContract<Decimal>
    {
        private readonly BigDecimal? _value;
        private readonly AttributeSet _attributes;

        #region Construction

        public Decimal(
            BigDecimal? value,
            params Attribute[] attributes)
        {
            _value = value;
            _attributes = attributes;
        }

        public static Decimal Of(
            BigDecimal? value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static implicit operator Decimal(BigDecimal? value) => new(value);

        #endregion

        #region DefaultContract
        public static Decimal Default => default;

        public bool IsDefault
            => _value is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Decimal Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _value is null;
        #endregion

        #region IStructValue

        public BigDecimal? Value => _value;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Decimal;

        #endregion

        #region Equatable

        public bool Equals(Decimal other) => ValueEquals(other);
        #endregion

        #region IValueEquatable
        public bool ValueEquals(Decimal other)
        {
            return EqualityComparer<BigDecimal?>.Default.Equals(_value, other.Value)
                && _attributes.Equals(other.Attributes);
        }

        public int ValueHash()
        {
            return _attributes.Aggregate(
                HashCode.Combine(_value),
                HashCode.Combine);
        }
        #endregion

        #region overrides

        public override string ToString()
        {
            return $"[@{Type} {_value?.ToString() ?? "*"}]";
        }

        public override int GetHashCode() => ValueHash();

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Decimal other && ValueEquals(other);

        public static bool operator ==(
            Decimal left,
            Decimal right)
            => left.Equals(right);

        public static bool operator !=(
            Decimal left,
            Decimal right)
            => !left.Equals(right);

        #endregion
    }
}
