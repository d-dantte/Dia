using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.Types
{
    public readonly struct IntValue :
        IStructValue<BigInteger>,
        IDeepCopyable<IntValue>,
        INullable<IntValue>,
        IEquatable<IntValue>,
        IValueEquatable<IntValue>,
        IDiaAddressable<IntValue>
    {
        #region Local members
        private readonly BigInteger? _value;
        private readonly Guid _address;

        private readonly Annotation[] _annotations;
        #endregion

        #region DiaReference
        public Guid Address => _address;

        public IntValue RelocateValue(Guid newAddress) => new(newAddress, _value, Annotations);
        #endregion

        #region StructValue
        public BigInteger? Value => _value;

        public DiaType Type => DiaType.Int;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors
        public IntValue(Guid address, BigInteger? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _address = address.ThrowIfDefault(
                _ => new ArgumentException($"Invalid {nameof(address)}: default"));
            _value = value;
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"Invalid {nameof(annotations)}: contains default"))
                .ToArray();
        }
        
        public IntValue(BigInteger? value, params Annotation[] annotations)
        : this(Guid.NewGuid(), value, annotations)
        { }

        public IntValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static implicit operator IntValue(BigInteger? value) => new IntValue(value);
        public static implicit operator IntValue(byte? value) => new IntValue(value);
        public static implicit operator IntValue(sbyte? value) => new IntValue(value);
        public static implicit operator IntValue(short? value) => new IntValue(value);
        public static implicit operator IntValue(ushort? value) => new IntValue(value);
        public static implicit operator IntValue(int? value) => new IntValue(value);
        public static implicit operator IntValue(uint? value) => new IntValue(value);
        public static implicit operator IntValue(long? value) => new IntValue(value);
        public static implicit operator IntValue(ulong? value) => new IntValue(value);

        public static implicit operator IntValue(BigInteger value) => new IntValue(value);
        public static implicit operator IntValue(byte value) => new IntValue(value);
        public static implicit operator IntValue(sbyte value) => new IntValue(value);
        public static implicit operator IntValue(short value) => new IntValue(value);
        public static implicit operator IntValue(ushort value) => new IntValue(value);
        public static implicit operator IntValue(int value) => new IntValue(value);
        public static implicit operator IntValue(uint value) => new IntValue(value);
        public static implicit operator IntValue(long value) => new IntValue(value);
        public static implicit operator IntValue(ulong value) => new IntValue(value);

        public static IntValue Of(BigInteger? value) => Of(value, Array.Empty<Annotation>());

        public static IntValue Of(
            Guid address,
            BigInteger? value,
            params Annotation[] annotations)
            => new IntValue(address, value, annotations);
        public static IntValue Of(
            BigInteger? value,
            params Annotation[] annotations)
            => new IntValue(value, annotations);
        #endregion

        #region DeepCopyable
        public IntValue DeepCopy() => new IntValue(_value, Annotations);
        #endregion

        #region Nullable
        public static IntValue Null(
            params Annotation[] annotations)
            => new IntValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(IntValue other)
        {
            return _value == other._value;
        }
        #endregion

        #region Equatable
        public bool Equals(IntValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other._annotations);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is IntValue other && Equals(other);

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

        public static bool operator ==(IntValue lhs, IntValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(IntValue lhs, IntValue rhs) => !(lhs != rhs);
        #endregion
    }
}
