using System.Diagnostics.CodeAnalysis;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Core.Types
{
    public readonly struct Boolean :
        IStructValue<bool>,
        IEquatable<Boolean>,
        INullContract<Boolean>,
        IValueEquatable<Boolean>,
        IDefaultContract<Boolean>
    {
        private readonly bool? _value;
        private readonly AttributeSet _attributes;

        #region Construction

        public Boolean(
            bool? value,
            params Attribute[] attributes)
        {
            _value = value;
            _attributes = attributes;
        }

        public static Boolean Of(
            bool? value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static implicit operator Boolean(bool? value) => new(value);

        #endregion

        #region DefaultContract
        public static Boolean Default => default;

        public bool IsDefault
            => _value is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Boolean Null(params Types.Attribute[] attributes) => new(null, attributes);

        public bool IsNull => _value is null;
        #endregion

        #region IStructValue

        public bool? Value => _value;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Bool;

        #endregion

        #region Equatable

        public bool Equals(Boolean other) => ValueEquals(other);
        #endregion

        #region IValueEquatable
        public bool ValueEquals(Boolean other)
        {
            return EqualityComparer<bool?>.Default.Equals(_value, other.Value)
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
            => obj is Boolean other && ValueEquals(other);

        public static bool operator ==(Boolean left, Boolean right) => left.Equals(right);

        public static bool operator !=(Boolean left, Boolean right) => !left.Equals(right);

        #endregion
    }
}
