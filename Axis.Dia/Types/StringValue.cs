using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    public readonly struct StringValue :
        IRefValue<string>,
        IDeepCopyable<StringValue>,
        INullable<StringValue>,
        IEquatable<StringValue>,
        IValueEquatable<StringValue, string>
    {
        #region Local members
        private readonly string? _value;

        private readonly Annotation[] _annotations;
        #endregion

        #region RefValue
        public string? Value => _value;

        public DiaType Type => DiaType.String;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors

        /// <summary>
        /// NOTE: the string assumes that its <paramref name="value"/> consists of an unescaped, raw sequence of characters.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="annotations"></param>
        public StringValue(string? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _value = value;
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain invalid values"))
                .ToArray();
        }

        public StringValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static implicit operator StringValue(string? value) => new StringValue(value);
        public static implicit operator StringValue(char[] value) => new StringValue(new string(value));
        public static implicit operator StringValue(Span<char> value) => new StringValue(new string(value));

        public static StringValue Of(string? value) => Of(value, Array.Empty<Annotation>());

        public static StringValue Of(
            string? value,
            params Annotation[] annotations)
            => new StringValue(value, annotations);
        #endregion

        #region DeepCopyable
        public StringValue DeepCopy() => new StringValue(_value, Annotations);
        #endregion

        #region Nullable
        public static StringValue Null(
            params Annotation[] annotations)
            => new StringValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(StringValue other)
        {
            return _value == other._value;
        }
        #endregion

        #region Equatable
        public bool Equals(StringValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other.Annotations);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is StringValue other && Equals(other);

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

            return $"[type: {Type}, value: {_value?.ToString() ?? ("null")}', annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(StringValue lhs, StringValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(StringValue lhs, StringValue rhs) => !(lhs != rhs);
        #endregion
    }
}
