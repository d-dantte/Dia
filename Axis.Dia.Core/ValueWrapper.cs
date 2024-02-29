using Axis.Dia.Core.Types;
using Axis.Luna.Common.Numerics;
using System.Numerics;

namespace Axis.Dia.Core
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct ValueWrapper
    {
        public IDiaValue Value { get; }

        public ValueWrapper(IDiaValue value)
        {
            ArgumentNullException.ThrowIfNull(value);
            Value = value;
        }

        public static ValueWrapper Of(
            IDiaValue value)
            => new(value);
                
        public static implicit operator ValueWrapper(bool value) => new ValueWrapper(new Types.Boolean(value));
        public static implicit operator ValueWrapper(byte value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(sbyte value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(short value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(ushort value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(int value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(uint value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(long value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(ulong value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(BigInteger value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(Half value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(float value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(double value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(decimal value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(BigDecimal value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(DateTimeOffset value) => new ValueWrapper(new Timestamp(value));
        public static implicit operator ValueWrapper(DateTime value) => new ValueWrapper(new Timestamp(value));
        public static implicit operator ValueWrapper(string value) => new ValueWrapper(new Types.String(value));
        public static implicit operator ValueWrapper(byte[] value) => new ValueWrapper(new Blob(value));
        public static implicit operator ValueWrapper(ReadOnlySpan<byte> value) => new ValueWrapper(new Blob(value.ToArray()));
        public static implicit operator ValueWrapper(IDiaValue[] value) => new ValueWrapper(new Sequence(value));


        public static implicit operator ValueWrapper(bool? value) => new ValueWrapper(new Types.Boolean(value));
        public static implicit operator ValueWrapper(byte? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(sbyte? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(short? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(ushort? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(int? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(uint? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(long? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(ulong? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(BigInteger? value) => new ValueWrapper(new Integer(value));
        public static implicit operator ValueWrapper(Half? value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(float? value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(double? value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(decimal? value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(BigDecimal? value) => new ValueWrapper(new Types.Decimal(value));
        public static implicit operator ValueWrapper(DateTimeOffset? value) => new ValueWrapper(new Timestamp(value));
        public static implicit operator ValueWrapper(DateTime? value) => new ValueWrapper(new Timestamp(value));


        public static implicit operator ValueWrapper(Types.Boolean value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Integer value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Types.Decimal value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Timestamp value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Types.String value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Symbol value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Blob value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Sequence value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(Record value) => new ValueWrapper(value);
        
    }
}
