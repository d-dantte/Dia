using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Axis.Dia.Types
{
    public readonly struct Annotation :
        IDiaType,
        IDefaultValueProvider<Annotation>,
        IDeepCopyable<Annotation>,
        IEquatable<Annotation>
    {
        private static readonly Regex AttributePattern = new(
            "^(?'key'[a-zA-Z_](([.-])?[a-zA-Z0-9_])*):(?'value'.+)\\z",
            RegexOptions.Compiled);

        private static readonly Regex IdentifierPattern = new(
            "^[a-zA-Z_](([.-])?[a-zA-Z0-9_])*\\z",
            RegexOptions.Compiled);

        #region Fields
        private readonly string _value;
        #endregion

        #region Properties
        public DiaType Type => DiaType.Annotation;

        public string Text => _value;

        /// <summary>
        /// Indicates if this annotation conforms to the symbol's Identifier pattern
        /// </summary>
        public bool IsIdentifier => IdentifierPattern.IsMatch(_value);

        /// <summary>
        /// Indicates if this symbol conforms to the attribute pattern <c>key:value</c>
        /// </summary>
        public bool IsAttribute => AttributePattern.IsMatch(_value);
        #endregion

        #region Construction
        public Annotation(string annotation)
        {
            _value = annotation.ThrowIf(
                string.IsNullOrEmpty,
                _ => new ArgumentException("Invalid annotation symbol"));
        }

        public static Annotation Of(string annotation) => new(annotation);

        public static Annotation Of(char[] annotation) => new(new string(annotation));

        public static Annotation Of(Span<char> annotation) => new(new string(annotation.ToArray()));

        public static Annotation[] Of(
            params string[] symbols)
            => symbols.Select(Of).ToArray();


        public static implicit operator Annotation(char[] symbol) => Annotation.Of(symbol);
        public static implicit operator Annotation(Span<char> symbol) => Annotation.Of(symbol);
        public static implicit operator Annotation(string symbol) => Annotation.Of(symbol);
        #endregion

        #region DefaultProvider
        public bool IsDefault => _value == null;

        public static Annotation Default => default;
        #endregion

        #region DeepCopyable
        public Annotation DeepCopy() => IsDefault ? default: new Annotation(_value);
        #endregion

        #region Equatable
        public bool Equals(Annotation other) => _value.NullOrEquals(other._value);
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[{_value}]";
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Annotation other
                && Equals(other);
        }

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(Annotation left, Annotation right) => left.Equals(right);

        public static bool operator !=(Annotation left, Annotation right) => !(left == right);
        #endregion

        #region API
        /// <summary>
        /// Returns this annotation, deconstructed into it's attribute form, if it indeed is an attribute.
        /// </summary>
        /// <param name="attribute">the output attribute</param>
        /// <returns>true if this is an attribute and the deconstruction is successful, false otherwise.</returns>
        public bool TryGetAttribute(out Attribute attribute)
        {
            var kvpMatch = AttributePattern.Match(_value);
            if (kvpMatch.Success)
            {
                attribute = new Attribute(
                    kvpMatch.Groups["key"].Value,
                    EscapeSequenceGroup.SymbolEscapeGroup.Unescape(kvpMatch.Groups["value"].Value)!);
                return true;
            }

            attribute = default;
            return false;
        }

        public Attribute GetAttribute()
        {
            if (TryGetAttribute(out var attribute))
                return attribute;

            throw new InvalidOperationException($"The annotation is not an attribute: '{Text}'");
        }

        /// <summary>
        /// Returns this annotation's value if it comforms to the Identifier pattern, otherwise, returns a null string.
        /// </summary>
        /// <param name="identifier">the output identifier if the pattern matches</param>
        /// <returns>True if this is an identifier, false otherwise</returns>
        public bool TryGetIdentifier(out string? identifier)
        {
            if (IsIdentifier)
            {
                identifier = _value;
                return true;
            }

            identifier = null!;
            return false;
        }

        public string GetIdentifier()
        {
            if (TryGetIdentifier(out var id))
                return id!;

            throw new InvalidOperationException($"The annotation is not an identifier: '{Text}'");
        }
        #endregion

        #region nested type
        public readonly struct Attribute: IDefaultValueProvider<Attribute>
        {
            /// <summary>
            /// The key
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// The value
            /// </summary>
            public string Value { get; }

            #region DefaultProvider

            public bool IsDefault => Key is null && Value is null;

            public static Attribute Default => default;

            #endregion

            /// <summary>
            /// Creates a new instance of the attribute.
            /// </summary>
            /// <param name="key">The key - cannot be null</param>
            /// <param name="value">The value - cannot be null</param>
            /// <exception cref="ArgumentNullException">If any of the arguments is null</exception>
            public Attribute(string key, string value)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                return obj is Attribute other
                    && Key.Equals(other.Key)
                    && Value.Equals(other.Value);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, Value);
            }

            public override string ToString()
            {
                return $"[{Key}: {Value}]";
            }

            public static bool operator ==(Attribute a, Attribute b) => a.Equals(b);

            public static bool operator !=(Attribute a, Attribute b) => !(a == b);
        }
        #endregion
    }
}
