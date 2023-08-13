using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    public readonly struct InstantValue :
        IStructValue<DateTimeOffset>,
        IDeepCopyable<InstantValue>,
        INullable<InstantValue>,
        IEquatable<InstantValue>,
        IValueEquatable<InstantValue, DateTimeOffset>
    {
        #region Local members
        private readonly DateTimeOffset? _value;

        private readonly Annotation[] _annotations;
        #endregion

        #region StructValue
        public DateTimeOffset? Value => _value;

        public DiaType Type => DiaType.Instant;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors
        public InstantValue(DateTimeOffset? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _value = value;
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain invalid values"))
                .ToArray();
        }

        public InstantValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static implicit operator InstantValue(DateTimeOffset? value) => new InstantValue(value);
        public static implicit operator InstantValue(DateTime? value) => new InstantValue(value);
        public static implicit operator InstantValue(TimeSpan? value) => new InstantValue(DateTimeOffset.Now + value);

        public static implicit operator InstantValue(DateTimeOffset value) => new InstantValue(value);
        public static implicit operator InstantValue(DateTime value) => new InstantValue(value);
        public static implicit operator InstantValue(TimeSpan value) => new InstantValue(DateTimeOffset.Now + value);

        public static InstantValue Of(DateTimeOffset? value) => Of(value, Array.Empty<Annotation>());

        public static InstantValue Of(
            DateTimeOffset? value,
            params Annotation[] annotations)
            => new InstantValue(value, annotations);
        #endregion

        #region DeepCopyable
        public InstantValue DeepCopy() => new InstantValue(_value, Annotations);
        #endregion

        #region Nullable
        public static InstantValue Null(
            params Annotation[] annotations)
            => new InstantValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(InstantValue other)
        {
            return _value == other._value;
        }
        #endregion

        #region Equatable
        public bool Equals(InstantValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other._annotations);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is InstantValue other && Equals(other);

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

        #region Misc

        public InstantValue Copy(Precision precision = Precision.Millisecond)
        {
            if (Value is null)
                return Null(_annotations);

            var ts = Value.Value;
            var milliseconds = int.Parse(ts.ToString("fffffff")) / 10000d;
            var dateTime = precision switch
            {
                Precision.Year => new DateTimeOffset(ts.Year, 1, 1, 0, 0, 0, Value.Value.Offset),
                Precision.Month => new DateTimeOffset(ts.Year, ts.Month, 1, 0, 0, 0, Value.Value.Offset),
                Precision.Day => new DateTimeOffset(ts.Year, ts.Month, ts.Day, 0, 0, 0, Value.Value.Offset),
                Precision.Minute => new DateTimeOffset(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute, 0, Value.Value.Offset),
                Precision.Second => new DateTimeOffset(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute, ts.Second, Value.Value.Offset),
                Precision.Millisecond => new DateTimeOffset(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute, ts.Second, Value.Value.Offset) + TimeSpan.FromMilliseconds(milliseconds),
                _ => throw new ArgumentException($"Invalid precision: {precision}")
            };

            return Of(dateTime, _annotations);
        }

        public InstantValue SwitchOffset(TimeSpan newOffset)
        {
            if (Value is null)
                return new InstantValue(null, _annotations);

            return new InstantValue(
                new DateTimeOffset(Value.Value.DateTime, newOffset),
                _annotations);
        }
        #endregion

        #region operators

        public static bool operator ==(InstantValue lhs, InstantValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(InstantValue lhs, InstantValue rhs) => !(lhs != rhs);
        #endregion


        #region Nested types
        public enum Precision
        {
            Year,
            Month,
            Day,
            Minute,
            Second,
            Millisecond
        }
        #endregion
    }
}
