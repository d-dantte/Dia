using Axis.Dia.Core.Contracts;
using Axis.Luna.Common;
using Axis.Luna.Common.StringEscape;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Types
{
    public readonly struct Symbol :
        IRefValue<string>,
        IEquatable<Symbol>,
        INullContract<Symbol>,
        IRefEquatable<Symbol>,
        IValueEquatable<Symbol>,
        IDefaultContract<Symbol>
    {
        private static readonly CommonStringEscaper Escaper = new CommonStringEscaper();
        private static readonly Regex UnicodeControlCharacterPattern = new Regex(
            "\\p{C}",
            RegexOptions.Compiled);
        private static readonly Regex IdentifierPattern = new(
            "^[a-zA-Z_$][a-zA-Z0-9_$]*$",
            RegexOptions.Compiled);

        private readonly string? _ref;
        private readonly AttributeSet _attributes;

        #region Construction

        public Symbol(
            string? value,
            params Attribute[] attributes)
        {
            _ref = value;
            _attributes = attributes;
        }

        public static Symbol Of(
            string? value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static implicit operator Symbol(
            string? value)
            => new(value);

        #endregion

        #region DefaultContract
        public static Symbol Default => default;

        public bool IsDefault
            => _ref is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Symbol Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _ref is null;
        #endregion

        #region IRefValue

        public string? Value => _ref;

        public bool RefEquals(IRefValue<string> other)
        {
            return other is Symbol sym && ReferenceEquals(_ref, sym._ref);
        }

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Symbol;

        #endregion

        #region API
        public bool IsIdentifier
            => !IsDefault
            && IdentifierPattern.IsMatch(_ref!);
        #endregion

        #region Equatable

        public bool Equals(Symbol other) => ValueEquals(other);
        #endregion

        #region IValueEquatable
        public bool ValueEquals(Symbol other)
        {
            return EqualityComparer<string?>.Default.Equals(_ref, other.Value)
                && _attributes.ValueEquals(other.Attributes);
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
        public bool RefEquals(Symbol other) => ValueEquals(other);

        public int RefHash() => HashCode.Combine(
            typeof(Symbol).FullName,
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
            => obj is Symbol other && ValueEquals(other);

        public static bool operator ==(
            Symbol left,
            Symbol right)
            => left.ValueEquals(right);

        public static bool operator !=(
            Symbol left,
            Symbol right)
            => !left.ValueEquals(right);

        #endregion
    }
}
