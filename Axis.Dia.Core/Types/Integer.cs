﻿using Axis.Dia.Core.Utils;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.Core.Types
{
    public readonly struct Integer :
        IStructValue<BigInteger>,
        IEquatable<Integer>,
        INullContract<Integer>,
        IDefaultContract<Integer>
    {
        private readonly BigInteger? _value;
        private readonly AttributeSet _attributes;

        #region Construction

        public Integer(
            BigInteger? value,
            params Attribute[] attributes)
        {
            _value = value;
            _attributes = attributes;
        }

        public static Integer Of(
            BigInteger? value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static implicit operator Integer(
            BigInteger? value)
            => new(value);

        #endregion

        #region DefaultContract
        public static Integer Default => default;

        public bool IsDefault
            => _value is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Integer Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _value is null;
        #endregion

        #region IStructValue

        public BigInteger? Value => _value;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Int;

        #endregion

        #region Equatable

        public override string ToString()
        {
            return $"[@{Type} {_value?.ToString() ?? "*"}]";
        }

        public bool Equals(
            Integer other)
            => EqualityComparer<BigInteger?>.Default.Equals(_value, other.Value)
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
            => obj is Integer other && Equals(other);

        public static bool operator ==(
            Integer left,
            Integer right)
            => left.Equals(right);

        public static bool operator !=(
            Integer left,
            Integer right)
            => !left.Equals(right);

        #endregion
    }
}
