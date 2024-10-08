using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Types
{
    public readonly struct Reference :
        IDiaType,
        IDefaultContract<Reference>,
        IValueEquatable<Reference>,
        IEquatable<Reference>
    {
        private readonly BigInteger _value;

        public BigInteger Ref => _value;

        public static DiaType ReferenceType => (DiaType)(byte)15;

        public DiaType Type => ReferenceType;

        #region IDefaultValueContract
        public static Reference Default => default;

        public bool IsDefault => _value.Equals(default(BigInteger));
        #endregion

        #region IValueEquatable
        public bool ValueEquals(Reference other)
        {
            return _value.Equals(other._value);
        }

        public int ValueHash()
        {
            return _value.GetHashCode();
        }
        #endregion

        #region Equatable

        public bool Equals(Reference other) => ValueEquals(other);
        #endregion

        #region overrides
        public override string ToString()
        {
            return $"[@Ref({Type}) {_value: X}]";
        }

        public override int GetHashCode() => ValueHash();

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Reference other && ValueEquals(other);

        public static bool operator ==(Reference left, Reference right) => left.Equals(right);

        public static bool operator !=(Reference left, Reference right) => !left.Equals(right);

        #endregion

        #region Construction
        public Reference(BigInteger value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            _value = value;
        }

        public static Reference Of(BigInteger value) => new(value);

        public static implicit operator Reference(BigInteger value) => new(value);
        public static implicit operator BigInteger(Reference @ref) => @ref._value;
        #endregion
    }
}
