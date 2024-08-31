using Axis.Dia.Core.Contracts;
using Axis.Luna.Common;
using Axis.Luna.Common.StringEscape;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Types
{
    public readonly struct String :
        IRefValue<string>,
        IEquatable<String>,
        INullContract<String>,
        IRefEquatable<String>,
        IValueEquatable<String>,
        IDefaultContract<String>
    {
        private static readonly CommonStringEscaper Escaper = new CommonStringEscaper();
        private static readonly Regex UnicodeControlCharacterPattern = new Regex(
            "\\p{C}",
            RegexOptions.Compiled);

        private readonly string? _ref;
        private readonly AttributeSet _attributes;

        #region Construction

        public String(
            string? value,
            params Attribute[] attributes)
        {
            _ref = value;
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
            => _ref is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static String Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _ref is null;
        #endregion

        #region IRefValue

        public string? Value => _ref;

        public bool RefEquals(IRefValue<string> other)
        {
            return other is String str && ReferenceEquals(_ref, str._ref);
        }

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.String;

        #endregion

        #region Equatable

        public bool Equals(String other) => ValueEquals(other);
        #endregion

        #region IValueEquatable
        public bool ValueEquals(String other)
        {
            return EqualityComparer<string?>.Default.Equals(_ref, other.Value)
                && _attributes.Equals(other.Attributes);
        }

        public int ValueHash()
        {
            return _attributes.Aggregate(
                HashCode.Combine(_ref),
                HashCode.Combine);
        }
        #endregion

        #region IRefEquatable

        /// <summary>
        /// String is a special ref-case. Equality is ALWAYS based off the value.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool RefEquals(String other) => ValueEquals(other);

        public int RefHash() => HashCode.Combine(
            typeof(String).FullName,
            _ref?.GetHashCode() ?? 0);
        #endregion

        #region overrides

        public override string ToString()
        {
            var text = _ref?
                .Take(20)
                .ApplyTo(chrs => new string(chrs.ToArray()))
                .ApplyTo(CharSequence.Of)
                .ApplyTo(seq => Escaper.Escape(seq, c => UnicodeControlCharacterPattern.IsMatch(c.ToString())))
                .ApplyTo(chars => string.Join("", chars.ToArray()))
                ?? "null";

            var length = _ref?.Length.ToString() ?? "*";

            return $"[@{Type} length: {length}, value: {text}]";
        }

        public override int GetHashCode()
        {
            return _attributes.Aggregate(
                HashCode.Combine(_ref),
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
