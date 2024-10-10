using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Numerics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.Core
{
    /// <summary>
    /// TODO: Rename to `DiaValue`
    /// </summary>
    public readonly struct DiaValue :
        IDefaultContract<DiaValue>,
        IValueEquatable<DiaValue>,
        IRefEquatable<DiaValue>,
        IEquatable<DiaValue>
    {
        private readonly IDiaValue _payload;

        public IDiaValue Payload => _payload;

        /// <summary>
        /// The <see cref="DiaType"/> of the payload. If the payload is null, returns (DiaType)0;
        /// </summary>
        public DiaType Type => IsDefault ? 0 : _payload.Type;

        #region Default Contract
        public static DiaValue Default => default;

        public bool IsDefault => _payload is null;
        #endregion

        #region Construction
        private DiaValue(IDiaValue payload)
        {
            ArgumentNullException.ThrowIfNull(payload);
            _payload = payload;
        }

        public DiaValue(
            Types.Boolean payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Blob payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Integer payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Decimal payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Duration payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Timestamp payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.String payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Symbol payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Sequence payload)
            : this((IDiaValue)payload)
        { }

        public DiaValue(
            Types.Record payload)
            : this((IDiaValue)payload)
        { }
        #endregion

        #region Of
        public static DiaValue Of(
            IDiaValue value)
        {
            return value switch
            {
                Types.Boolean v => new(v),
                Types.Integer v => new(v),
                Types.Decimal v => new(v),
                Types.Duration v => new(v),
                Types.Timestamp v => new(v),
                Types.String v => new(v),
                Types.Symbol v => new(v),
                Types.Blob v => new(v),
                Types.Sequence v => new(v),
                Types.Record v => new(v),
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new InvalidOperationException(
                    $"Invalid {nameof(value)}: unknown type '{value!.GetType()}'")
            };
        }

        public static DiaValue Of(
            Types.Boolean value)
            => new(value);
        public static DiaValue Of(
            Types.Blob value)
            => new(value);
        public static DiaValue Of(
            Types.Integer value)
            => new(value);
        public static DiaValue Of(
            Types.Decimal value)
            => new(value);
        public static DiaValue Of(
            Types.Duration value)
            => new(value);
        public static DiaValue Of(
            Types.Timestamp value)
            => new(value);
        public static DiaValue Of(
            Types.String value)
            => new(value);
        public static DiaValue Of(
            Types.Symbol value)
            => new(value);
        public static DiaValue Of(
            Types.Sequence value)
            => new(value);
        public static DiaValue Of(
            Types.Record value)
            => new(value);

        #endregion

        #region Primitive Implicits
        public static implicit operator DiaValue(bool value) => new(new Types.Boolean(value));
        public static implicit operator DiaValue(byte value) => new(new Integer(value));
        public static implicit operator DiaValue(sbyte value) => new(new Integer(value));
        public static implicit operator DiaValue(short value) => new(new Integer(value));
        public static implicit operator DiaValue(ushort value) => new(new Integer(value));
        public static implicit operator DiaValue(int value) => new(new Integer(value));
        public static implicit operator DiaValue(uint value) => new(new Integer(value));
        public static implicit operator DiaValue(long value) => new(new Integer(value));
        public static implicit operator DiaValue(ulong value) => new(new Integer(value));
        public static implicit operator DiaValue(BigInteger value) => new(new Integer(value));
        public static implicit operator DiaValue(Half value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(float value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(double value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(decimal value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(BigDecimal value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(DateTimeOffset value) => new(new Timestamp(value));
        public static implicit operator DiaValue(DateTime value) => new(new Timestamp(value));
        public static implicit operator DiaValue(TimeSpan value) => new(new Duration(value));
        public static implicit operator DiaValue(string value) => new(new Types.String(value));
        public static implicit operator DiaValue(byte[] value) => new(new Blob(value));
        public static implicit operator DiaValue(ReadOnlySpan<byte> value) => new(new Blob(value.ToArray()));

        public static implicit operator DiaValue(bool? value) => new(new Types.Boolean(value));
        public static implicit operator DiaValue(byte? value) => new(new Integer(value));
        public static implicit operator DiaValue(sbyte? value) => new(new Integer(value));
        public static implicit operator DiaValue(short? value) => new(new Integer(value));
        public static implicit operator DiaValue(ushort? value) => new(new Integer(value));
        public static implicit operator DiaValue(int? value) => new(new Integer(value));
        public static implicit operator DiaValue(uint? value) => new(new Integer(value));
        public static implicit operator DiaValue(long? value) => new(new Integer(value));
        public static implicit operator DiaValue(ulong? value) => new(new Integer(value));
        public static implicit operator DiaValue(BigInteger? value) => new(new Integer(value));
        public static implicit operator DiaValue(Half? value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(float? value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(double? value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(decimal? value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(BigDecimal? value) => new(new Types.Decimal(value));
        public static implicit operator DiaValue(DateTimeOffset? value) => new(new Timestamp(value));
        public static implicit operator DiaValue(DateTime? value) => new(new Timestamp(value));
        public static implicit operator DiaValue(TimeSpan? value) => new(new Duration(value));

        public static implicit operator DiaValue(
            IDiaValue[] value)
            => new(new Sequence(value));
        public static implicit operator DiaValue(
            DiaValue[] value)
            => new(new Sequence(value));

        public static implicit operator DiaValue(
            KeyValuePair<string, IDiaValue>[] value)
            => new(new Record(value.Select(v => new Record.Property(v.Key, DiaValue.Of(v.Value)))));
        public static implicit operator DiaValue(
            KeyValuePair<string, DiaValue>[] value)
            => new(new Record(value.Select(v => new Record.Property(v.Key, v.Value))));
        #endregion

        #region Dia implicits
        public static implicit operator DiaValue(Types.Boolean value) => new(value);
        public static implicit operator DiaValue(Types.Blob value) => new(value);
        public static implicit operator DiaValue(Types.Integer value) => new(value);
        public static implicit operator DiaValue(Types.Decimal value) => new(value);
        public static implicit operator DiaValue(Types.Timestamp value) => new(value);
        public static implicit operator DiaValue(Types.Duration value) => new(value);
        public static implicit operator DiaValue(Types.String value) => new(value);
        public static implicit operator DiaValue(Types.Symbol value) => new(value);
        public static implicit operator DiaValue(Types.Sequence value) => new(value);
        public static implicit operator DiaValue(Types.Record value) => new(value);
        #endregion

        #region Asxxx
        public IDiaValue AsDiaValue() => _payload;

        public Types.Boolean AsBool()
        {
            return
                IsDefault ? default :
                Type == DiaType.Bool ? (Types.Boolean)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Bool}");
        }

        public Types.Blob AsBlob()
        {
            return
                IsDefault ? default :
                Type == DiaType.Blob ? (Types.Blob)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Blob}");
        }

        public Types.Integer AsInteger()
        {
            return
                IsDefault ? default :
                Type == DiaType.Int ? (Types.Integer)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Int}");
        }

        public Types.Decimal AsDecimal()
        {
            return
                IsDefault ? default :
                Type == DiaType.Decimal ? (Types.Decimal)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Decimal}");
        }

        public Types.Duration AsDuration()
        {
            return
                IsDefault ? default :
                Type == DiaType.Duration ? (Types.Duration)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Duration}");
        }

        public Types.Timestamp AsTimestamp()
        {
            return
                IsDefault ? default :
                Type == DiaType.Timestamp ? (Types.Timestamp)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Timestamp}");
        }

        public Types.String AsString()
        {
            return
                IsDefault ? default :
                Type == DiaType.String ? (Types.String)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.String}");
        }

        public Types.Symbol AsSymbol()
        {
            return
                IsDefault ? default :
                Type == DiaType.Symbol ? (Types.Symbol)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Symbol}");
        }

        public Types.Sequence AsSequence()
        {
            return
                IsDefault ? default :
                Type == DiaType.Sequence ? (Types.Sequence)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Sequence}");
        }

        public Types.Record AsRecord()
        {
            return
                IsDefault ? default :
                Type == DiaType.Record ? (Types.Record)_payload :
                throw new InvalidCastException($"Invalid cast: {Type} to {DiaType.Record}");
        }
        #endregion

        #region Is

        public bool Is(out Types.Boolean value)
        {
            (value, var @is) = _payload switch
            {
                Types.Boolean b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Blob value)
        {
            (value, var @is) = _payload switch
            {
                Types.Blob b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Integer value)
        {
            (value, var @is) = _payload switch
            {
                Types.Integer b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Decimal value)
        {
            (value, var @is) = _payload switch
            {
                Types.Decimal b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Duration value)
        {
            (value, var @is) = _payload switch
            {
                Types.Duration b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Timestamp value)
        {
            (value, var @is) = _payload switch
            {
                Types.Timestamp b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.String value)
        {
            (value, var @is) = _payload switch
            {
                Types.String b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Symbol value)
        {
            (value, var @is) = _payload switch
            {
                Types.Symbol b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Sequence value)
        {
            (value, var @is) = _payload switch
            {
                Types.Sequence b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        public bool Is(out Types.Record value)
        {
            (value, var @is) = _payload switch
            {
                Types.Record b => (b, true),
                _ => (default, false)
            };

            return @is;
        }

        #endregion

        #region IValueEquatable

        public bool ValueEquals(DiaValue other)
        {
            return (_payload, other._payload) switch
            {
                (Core.Types.Boolean v1, Core.Types.Boolean v2) => v1.ValueEquals(v2),
                (Core.Types.Integer v1, Core.Types.Integer v2) => v1.ValueEquals(v2),
                (Core.Types.Decimal v1, Core.Types.Decimal v2) => v1.ValueEquals(v2),
                (Core.Types.Duration v1, Core.Types.Duration v2) => v1.ValueEquals(v2),
                (Core.Types.Timestamp v1, Core.Types.Timestamp v2) => v1.ValueEquals(v2),
                (Core.Types.Blob v1, Core.Types.Blob v2) => v1.ValueEquals(v2),
                (Core.Types.String v1, Core.Types.String v2) => v1.ValueEquals(v2),
                (Core.Types.Symbol v1, Core.Types.Symbol v2) => v1.ValueEquals(v2),
                (Core.Types.Sequence v1, Core.Types.Sequence v2) => v1.ValueEquals(v2),
                (Core.Types.Record v1, Core.Types.Record v2) => v1.ValueEquals(v2),
                _ => false
            };
        }

        public int ValueHash()
        {
            return _payload switch
            {
                Core.Types.Boolean v1 => v1.ValueHash(),
                Core.Types.Integer v1 => v1.ValueHash(),
                Core.Types.Decimal v1 => v1.ValueHash(),
                Core.Types.Duration v1 => v1.ValueHash(),
                Core.Types.Timestamp v1 => v1.ValueHash(),
                Core.Types.Blob v1 => v1.ValueHash(),
                Core.Types.String v1 => v1.ValueHash(),
                Core.Types.Symbol v1 => v1.ValueHash(),
                Core.Types.Sequence v1 => v1.ValueHash(),
                Core.Types.Record v1 => v1.ValueHash(),
                _ => throw new InvalidOperationException(
                    $"Invalid type: non {typeof(IValueEquatable<>)} type found '{_payload?.GetType()}'")
            };
        }
        #endregion

        #region IRefEquatable

        public bool RefEquals(DiaValue other)
        {
            return (_payload, other._payload) switch
            {
                (Core.Types.String v1, Core.Types.String v2) => v1.RefEquals(v2),
                (Core.Types.Symbol v1, Core.Types.Symbol v2) => v1.RefEquals(v2),
                (Core.Types.Sequence v1, Core.Types.Sequence v2) => v1.RefEquals(v2),
                (Core.Types.Record v1, Core.Types.Record v2) => v1.RefEquals(v2),
                _ => false
            };
        }

        public int RefHash()
        {
            return _payload switch
            {
                Core.Types.String v1 => v1.RefHash(),
                Core.Types.Symbol v1 => v1.RefHash(),
                Core.Types.Sequence v1 => v1.RefHash(),
                Core.Types.Record v1 => v1.RefHash(),
                _ => throw new InvalidOperationException(
                    $"Invalid type: non {typeof(IRefEquatable<>)} type found '{_payload?.GetType()}'")
            };
        }
        #endregion

        #region Overrides
        public bool Equals(DiaValue other)
        {
            return EqualityComparer<IDiaValue>.Default.Equals(_payload, other._payload);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is DiaValue cv && ValueEquals(cv);
        }

        public override int GetHashCode() => _payload?.GetHashCode() ?? 0;

        public override string ToString()
        {
            return IsDefault ? null! : _payload.ToString()!;
        }

        public static bool operator ==(DiaValue left, DiaValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiaValue left, DiaValue right)
        {
            return !(left == right);
        }
        #endregion
    }
}
