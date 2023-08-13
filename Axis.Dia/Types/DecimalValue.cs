using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.Types
{
    public readonly struct DecimalValue :
        IStructValue<BigDecimal>,
        IDeepCopyable<DecimalValue>,
        INullable<DecimalValue>,
        IEquatable<DecimalValue>,
        IValueEquatable<DecimalValue, BigDecimal>
    {
        #region Local members
        private readonly BigDecimal? _value;

        private readonly Annotation[] _annotations;
        #endregion

        #region StructValue
        public BigDecimal? Value => _value;

        public DiaType Type => DiaType.Decimal;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors
        public DecimalValue(BigDecimal? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _value = value;
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain invalid values"))
                .ToArray();
        }

        public DecimalValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static implicit operator DecimalValue(BigDecimal? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(BigInteger? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(byte? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(sbyte? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(short? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(ushort? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(int? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(uint? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(long? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(ulong? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(Half? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(float? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(double? value) => new DecimalValue(value);
        public static implicit operator DecimalValue(decimal? value) => new DecimalValue(value);

        public static implicit operator DecimalValue(BigDecimal value) => new DecimalValue(value);
        public static implicit operator DecimalValue(BigInteger value) => new DecimalValue(value);
        public static implicit operator DecimalValue(byte value) => new DecimalValue(value);
        public static implicit operator DecimalValue(sbyte value) => new DecimalValue(value);
        public static implicit operator DecimalValue(short value) => new DecimalValue(value);
        public static implicit operator DecimalValue(ushort value) => new DecimalValue(value);
        public static implicit operator DecimalValue(int value) => new DecimalValue(value);
        public static implicit operator DecimalValue(uint value) => new DecimalValue(value);
        public static implicit operator DecimalValue(long value) => new DecimalValue(value);
        public static implicit operator DecimalValue(ulong value) => new DecimalValue(value);
        public static implicit operator DecimalValue(Half value) => new DecimalValue(value);
        public static implicit operator DecimalValue(float value) => new DecimalValue(value);
        public static implicit operator DecimalValue(double value) => new DecimalValue(value);
        public static implicit operator DecimalValue(decimal value) => new DecimalValue(value);

        public static DecimalValue Of(BigDecimal? value) => Of(value, Array.Empty<Annotation>());

        public static DecimalValue Of(
            BigDecimal? value,
            params Annotation[] annotations)
            => new DecimalValue(value, annotations);
        #endregion

        #region DeepCopyable
        public DecimalValue DeepCopy() => new DecimalValue(_value, Annotations);
        #endregion

        #region Nullable
        public static DecimalValue Null(
            params Annotation[] annotations)
            => new DecimalValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(DecimalValue other)
        {
            return _value == other._value;
        }
        #endregion

        #region Equatable
        public bool Equals(DecimalValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other._annotations);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is DecimalValue other && Equals(other);

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

            return $"[type: {Type}, value: {_value?.ToString() ?? ("null")}, annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(DecimalValue lhs, DecimalValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(DecimalValue lhs, DecimalValue rhs) => !(lhs != rhs);
        #endregion
    }
}
