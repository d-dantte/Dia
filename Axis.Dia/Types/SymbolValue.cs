using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Axis.Dia.Types
{
    /// <summary>
    /// Symbols are special "strings" containing ONLY ascii printable characters
    /// </summary>
    public readonly struct SymbolValue :
        IRefValue<string>,
        IDeepCopyable<SymbolValue>,
        IDefaultValueProvider<SymbolValue>,
        INullable<SymbolValue>,
        IEquatable<SymbolValue>,
        IValueEquatable<SymbolValue, string>
    {
        public static readonly Regex IdentifierPattern = new(
            "^[a-zA-Z_](([.-])?[a-zA-Z0-9_])*\\z",
            RegexOptions.Compiled);

        #region Fields
        private readonly string? _value;
        #endregion

        #region Properties
        private readonly Annotation[] _annotations;

        /// <summary>
        /// Indicates if this symbol conforms to the Identifier pattern
        /// </summary>
        public bool IsIdentifier => !IsNull && IdentifierPattern.IsMatch(_value!);
        #endregion

        #region API
        /// <summary>
        /// Returns this symbol's value if it comforms to the Identifier pattern, otherwise, returns null
        /// </summary>
        /// <param name="identifier">the output identifier if the pattern matches</param>
        /// <returns>True if this is an identifier, false otherwise</returns>
        public bool TryGetIdentifier(out string? identifier)
        {
            if (IsIdentifier)
            {
                identifier = _value!;
                return true;
            }

            identifier = null!;
            return false;
        }
        #endregion

        #region RefValue
        public string? Value => _value;

        public DiaType Type => DiaType.Symbol;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors
        public SymbolValue(string? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _value = ValidateSymbolString(value);
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain invalid values"))
                .ToArray();
        }

        public SymbolValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static SymbolValue Of(string? value) => Of(value, Array.Empty<Annotation>());

        public static SymbolValue Of(string? value, params Annotation[] annotations) => new SymbolValue(value, annotations);

        public static implicit operator SymbolValue(string? value) => Of(value);

        public static implicit operator SymbolValue(char[] value) => Of(new string(value));

        public static implicit operator SymbolValue(Span<char> value) => Of(new string(value.ToArray()));
        #endregion

        #region DeepCopyable
        public SymbolValue DeepCopy() => new SymbolValue(_value, Annotations);
        #endregion

        #region Nullable
        public static SymbolValue Null(
            params Annotation[] annotations)
            => new SymbolValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(SymbolValue other)
        {
            return _value == other._value;
        }
        #endregion

        #region Equatable
        public bool Equals(SymbolValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other._annotations);
        }
        #endregion

        #region DefaultProvider
        public bool IsDefault => _value == null && _annotations == null;

        public static SymbolValue Default => default;
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is SymbolValue other && Equals(other);

        public override int GetHashCode()
        {
            return Annotations.Aggregate(
                _value?.GetHashCode() ?? 0,
                (code, next) => HashCode.Combine(code, next));
        }

        public override string ToString()
        {
            var attText = Annotations
                .Select(att => att.ToString()!)
                .JoinUsing(", ");

            return $"[type: {Type}, value: '{_value?.ToString() ?? ("null")}', annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(SymbolValue lhs, SymbolValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(SymbolValue lhs, SymbolValue rhs) => !(lhs != rhs);
        #endregion

        #region Helpers
        private static string? ValidateSymbolString(string? symbol)
        {
            if (symbol is null)
                return null;

            if (string.Empty.Equals(symbol))
                throw new ArgumentException($"Symbol text cannot be empty");

            return symbol
                .ThrowIfAny(
                    c => !char.IsAscii(c) || c < 32,
                    _ => new ArgumentOutOfRangeException(
                        nameof(symbol),
                        $"The argument contains non-printable ascii characters"))
                .ApplyTo(chars => new string(chars.ToArray()));
        }
        #endregion
    }
}
