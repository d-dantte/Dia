using Axis.Dia.Core.Utils;
using Axis.Luna.Common.Numerics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    public readonly struct Decimal :
        IStructValue<BigDecimal>,
        IEquatable<Decimal>,
        INullContract<Decimal>,
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

        public static implicit operator Decimal(
            BigDecimal? value)
            => new(value);

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

        public override string ToString()
        {
            return $"[@{Type} {_value?.ToString() ?? "*"}]";
        }

        public bool Equals(
            Decimal other)
            => EqualityComparer<BigDecimal?>.Default.Equals(_value, other.Value)
            && _attributes.Equals(other.Attributes);
        #endregion

        #region overrides
        public override int GetHashCode()
        {
            return _attributes.Aggregate(
                HashCode.Combine(_value),
                HashCode.Combine);
        }

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Decimal other && Equals(other);

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
