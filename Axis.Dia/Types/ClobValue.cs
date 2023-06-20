using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    public readonly struct ClobValue :
        IRefValue<string>,
        IDeepCopyable<ClobValue>,
        INullable<ClobValue>,
        IEquatable<ClobValue>,
        IValueEquatable<ClobValue, string>
    {
        #region Local members
        private readonly string? _value;

        private readonly Annotation[] _annotations;
        #endregion

        #region RefValue
        public string? Value => _value;

        public DiaType Type => DiaType.Clob;

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
        public ClobValue(string? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _value = value;
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann is null,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain null"))
                .ToArray();
        }

        public ClobValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static implicit operator ClobValue(string? value) => new ClobValue(value);
        public static implicit operator ClobValue(char[] value) => new ClobValue(new string(value));
        public static implicit operator ClobValue(Span<char> value) => new ClobValue(new string(value));

        public static ClobValue Of(
            string? value,
            params Annotation[] annotations)
            => new ClobValue(value, annotations);
        #endregion

        #region DeepCopyable
        public ClobValue DeepCopy() => new ClobValue(_value, Annotations);
        #endregion

        #region Nullable
        public static ClobValue Null(
            params Annotation[] annotations)
            => new ClobValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(ClobValue other)
        {
            return _value == other._value;
        }
        #endregion

        #region Equatable
        public bool Equals(ClobValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other._annotations);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is ClobValue other && Equals(other);

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

        public static bool operator ==(ClobValue lhs, ClobValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(ClobValue lhs, ClobValue rhs) => !(lhs != rhs);
        #endregion
    }
}
