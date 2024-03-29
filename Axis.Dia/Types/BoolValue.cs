﻿using Axis.Dia.Contracts;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    public readonly struct BoolValue :
        IStructValue<bool>,
        IDeepCopyable<BoolValue>,
        INullable<BoolValue>,
        IEquatable<BoolValue>,
        IValueEquatable<BoolValue>,
        IDiaAddressable<BoolValue>
    {
        #region Local members
        private readonly bool? _value;
        private readonly Guid _address;

        private readonly Annotation[] _annotations;
        #endregion

        #region DiaReference
        public Guid Address => _address;

        public BoolValue RelocateValue(Guid newAddress) => new(newAddress, _value, Annotations);
        #endregion

        #region StructValue
        public bool? Value => _value;

        public DiaType Type => DiaType.Bool;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors
        public BoolValue(Guid address, bool? value, params Annotation[] annotations)
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

        public BoolValue(bool? value, params Annotation[] annotations)
        : this(Guid.NewGuid(), value, annotations)
        { }

        public BoolValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static implicit operator BoolValue(bool? value) => new BoolValue(value);

        public static BoolValue Of(bool? value) => Of(value, Array.Empty<Annotation>());

        public static BoolValue Of(
            bool? value,
            params Annotation[] annotations)
            => new BoolValue(value, annotations);

        public static BoolValue Of(
            Guid address,
            bool? value,
            params Annotation[] annotations)
            => new BoolValue(address, value, annotations);
        #endregion

        #region DeepCopyable
        public BoolValue DeepCopy() => new BoolValue(_value, Annotations);
        #endregion

        #region Nullable
        public static BoolValue Null(
            params Annotation[] annotations)
            => new BoolValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(BoolValue other)
        {
            return _value == other._value;
        }
        #endregion

        #region Equatable
        public bool Equals(BoolValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other._annotations);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is BoolValue other && Equals(other);

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

            return $"[type:  {Type} , value: {_value?.ToString() ?? ("null")}, annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(BoolValue lhs, BoolValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(BoolValue lhs, BoolValue rhs) => !(lhs != rhs);
        #endregion
    }
}
