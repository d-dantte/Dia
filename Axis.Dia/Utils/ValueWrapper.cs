using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Numerics;
using System.Numerics;

namespace Axis.Dia.Utils
{

    public readonly struct ValueWrapper
    {
        public IDiaValue Value { get; }

        public ValueWrapper(IDiaValue value)
        {
            Value = value;
        }


        public static implicit operator ValueWrapper(bool value) => new ValueWrapper(new BoolValue(value));
        public static implicit operator ValueWrapper(byte value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(sbyte value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(short value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(ushort value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(int value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(uint value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(long value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(ulong value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(BigInteger value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(Half value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(float value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(double value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(decimal value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(BigDecimal value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(DateTimeOffset value) => new ValueWrapper(new InstantValue(value));
        public static implicit operator ValueWrapper(DateTime value) => new ValueWrapper(new InstantValue(value));
        public static implicit operator ValueWrapper(TimeSpan value) => new ValueWrapper((InstantValue)value);
        public static implicit operator ValueWrapper(string value) => new ValueWrapper(new StringValue(value));
        public static implicit operator ValueWrapper(byte[] value) => new ValueWrapper(new BlobValue(value));
        public static implicit operator ValueWrapper(Span<byte> value) => new ValueWrapper(new BlobValue(value.ToArray()));
        public static implicit operator ValueWrapper(IDiaValue[] value) => new ValueWrapper(new ListValue(value));
        public static implicit operator ValueWrapper(Guid value) => new ValueWrapper(ReferenceValue.Of(value));


        public static implicit operator ValueWrapper(bool? value) => new ValueWrapper(new BoolValue(value));
        public static implicit operator ValueWrapper(byte? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(sbyte? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(short? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(ushort? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(int? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(uint? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(long? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(ulong? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(BigInteger? value) => new ValueWrapper(new IntValue(value));
        public static implicit operator ValueWrapper(Half? value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(float? value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(double? value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(decimal? value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(BigDecimal? value) => new ValueWrapper(new DecimalValue(value));
        public static implicit operator ValueWrapper(DateTimeOffset? value) => new ValueWrapper(new InstantValue(value));
        public static implicit operator ValueWrapper(DateTime? value) => new ValueWrapper(new InstantValue(value));
        public static implicit operator ValueWrapper(TimeSpan? value) => new ValueWrapper((InstantValue)value);


        public static implicit operator ValueWrapper(BoolValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(IntValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(DecimalValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(InstantValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(StringValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(SymbolValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(ClobValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(BlobValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(ListValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(RecordValue value) => new ValueWrapper(value);
        public static implicit operator ValueWrapper(ReferenceValue value) => new ValueWrapper(value);
    }
}
