using Axis.Dia.Core.Utils;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    public readonly struct String :
        IRefValue<string>,
        IEquatable<String>,
        INullContract<String>,
        IDefaultContract<String>
    {
        private readonly string? _value;
        private readonly AttributeSet _attributes;

        #region Construction

        public String(
            string? value,
            params Attribute[] attributes)
        {
            _value = value;
            _attributes = attributes;
        }

        public static String Of(
            string? value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static implicit operator String(
            string? value)
            => new(value);

        #endregion

        #region DefaultContract
        public static String Default => default;

        public bool IsDefault
            => _value is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static String Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _value is null;
        #endregion

        #region IRefValue

        public string? Value => _value;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.String;

        #endregion

        #region Equatable

        public override string ToString()
        {
            var text = _value?
                .Take(20)
                .Select(CommonExtensions.EscapeUnicodeControlCharacter)
                .ApplyTo(chars => string.Join("", chars.ToArray()))
                ?? "null";

            var length = _value?.Length.ToString() ?? "*";

            return $"[@{Type} length: {length}, value: {text}]";
        }

        public bool Equals(
            String other)
            => EqualityComparer<string>.Default.Equals(_value, other.Value)
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
            => obj is String other && Equals(other);

        public static bool operator ==(
            String left,
            String right)
            => left.Equals(right);

        public static bool operator !=(
            String left,
            String right)
            => !left.Equals(right);

        #endregion
    }
}
