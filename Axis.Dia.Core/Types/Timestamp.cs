using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    public readonly struct Timestamp :
        IStructValue<DateTimeOffset>,
        IEquatable<Timestamp>,
        INullContract<Timestamp>,
        IDefaultContract<Timestamp>
    {
        private readonly DateTimeOffset? _value;
        private readonly AttributeSet _attributes;

        #region Construction

        public Timestamp(
            DateTimeOffset? value,
            params Attribute[] attributes)
        {
            _value = value;
            _attributes = attributes;
        }

        public static Timestamp Of(
            DateTimeOffset? value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static implicit operator Timestamp(
            DateTimeOffset? value)
            => new(value);

        #endregion

        #region DefaultContract
        public static Timestamp Default => default;

        public bool IsDefault
            => _value is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Timestamp Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _value is null;
        #endregion

        #region IStructValue

        public DateTimeOffset? Value => _value;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Int;

        #endregion

        #region Equatable

        public override string ToString()
        {
            return $"[@{Type} {_value?.ToString() ?? "*"}]";
        }

        public bool Equals(
            Timestamp other)
            => EqualityComparer<DateTimeOffset?>.Default.Equals(_value, other.Value)
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
            => obj is Timestamp other && Equals(other);

        public static bool operator ==(
            Timestamp left,
            Timestamp right)
            => left.Equals(right);

        public static bool operator !=(
            Timestamp left,
            Timestamp right)
            => !left.Equals(right);

        #endregion
    }
}
