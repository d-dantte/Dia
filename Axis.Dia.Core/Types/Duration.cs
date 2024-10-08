using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    public readonly struct Duration :
        IStructValue<TimeSpan>,
        IEquatable<Duration>,
        INullContract<Duration>,
        IDefaultContract<Duration>
    {
        private readonly long? _nanoSeconds;
        private readonly AttributeSet _attributes;

        #region Construction

        public Duration(
            TimeSpan? value,
            params Attribute[] attributes)
            : this(value?.Ticks * 100, attributes)
        {
        }

        public Duration(
            long? nanoSeconds,
            params Attribute[] attributes)
        {
            _nanoSeconds = nanoSeconds;
            _attributes = attributes;
        }

        public static Duration Of(
            TimeSpan value,
            params Attribute[] attributes)
            => new(value, attributes);

        public static implicit operator Duration(
            TimeSpan value)
            => new(value);

        public static Duration Of(
            long? nanoSeconds,
            params Attribute[] attributes)
            => new(nanoSeconds, attributes);

        public static implicit operator Duration(
            long? nanoSeconds)
            => new(nanoSeconds);

        #endregion

        #region DefaultContract
        public static Duration Default => default;

        public bool IsDefault
            => _nanoSeconds is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Duration Null(params
            Types.Attribute[] attributes)
            => new((TimeSpan?)null, attributes);

        public bool IsNull => _nanoSeconds is null;
        #endregion

        #region IStructValue

        public TimeSpan? Value => _nanoSeconds?.ApplyTo(n => TimeSpan.FromTicks(n/100));

        public long? NanoSeconds => _nanoSeconds;

        public AttributeSet Attributes => _attributes;

        public DiaType Type => DiaType.Duration;

        #endregion

        #region Equatable

        public bool Equals(Duration other) => ValueEquals(other);
        #endregion

        #region IValueEquatable
        public bool ValueEquals(Duration other)
        {
            return EqualityComparer<long?>.Default.Equals(_nanoSeconds, other._nanoSeconds)
                && _attributes.Equals(other.Attributes);
        }

        public int ValueHash()
        {
            return _attributes.Aggregate(
                HashCode.Combine(_nanoSeconds),
                HashCode.Combine);
        }
        #endregion

        #region overrides

        public override string ToString()
        {
            return $"[@{Type} {_nanoSeconds?.ToString() ?? "*"}]";
        }

        public override int GetHashCode()
        {
            return _attributes.Aggregate(
                HashCode.Combine(_nanoSeconds),
                HashCode.Combine);
        }

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Duration other && Equals(other);

        public static bool operator ==(
            Duration left,
            Duration right)
            => left.Equals(right);

        public static bool operator !=(
            Duration left,
            Duration right)
            => !left.Equals(right);

        #endregion
    }
}
