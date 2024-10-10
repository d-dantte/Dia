using Axis.Dia.Core.Contracts;
using Axis.Dia.TypeConverter.Clr;
using System.Collections.Immutable;
using System.Numerics;

namespace Axis.Dia.TypeConverter.Tests.Clr
{
    [TestClass]
    public class SimpleTypeConverterTests
    {
        [TestMethod]
        public void CanConvert_Tests()
        {
            var converter = new SimpleTypeConverter();

            Assert.ThrowsException<ArgumentException>(
                () => converter.CanConvert(Core.DiaType.Int, default));

            // unknown
            Assert.IsFalse(converter.CanConvert((Core.DiaType)(-1), typeof(int).ToTypeInfo()));

            // bool
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Bool, typeof(bool).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Bool, typeof(int).ToTypeInfo()));

            // blob
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Blob, typeof(byte[]).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Blob, typeof(int).ToTypeInfo()));

            // Decimal
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Decimal, typeof(decimal).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Decimal, typeof(int).ToTypeInfo()));

            // duration
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Duration, typeof(TimeSpan).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Duration, typeof(int).ToTypeInfo()));

            // int
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Int, typeof(long).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Int, typeof(float).ToTypeInfo()));

            // Record
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Record, typeof(Dictionary<string, object>).ToTypeInfo()));

            // sequence
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Sequence, typeof(IDiaType[]).ToTypeInfo()));
        }

        [TestMethod]
        public void ToClr_Tests()
        {
            var converter = new SimpleTypeConverter();

            var typeInfo = typeof(bool).ToTypeInfo();

            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToClr(null!, typeInfo, null!));

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToClr(Core.Types.Integer.Null(), typeInfo, null!));

            Assert.IsNull(converter.ToClr(
                Core.Types.Boolean.Null(),
                typeInfo,
                null!));

            // bool
            Assert.AreEqual(true, converter.ToClr(Core.Types.Boolean.Of(true), typeInfo, null!));

            // string
            typeInfo = typeof(string).ToTypeInfo();
            Assert.AreEqual("stuff", converter.ToClr(Core.Types.String.Of("stuff"), typeInfo, null!));

            // symbol
            typeInfo = typeof(string).ToTypeInfo();
            Assert.AreEqual("stuff", converter.ToClr(Core.Types.Symbol.Of("stuff"), typeInfo, null!));

            #region Integer
            typeInfo = typeof(byte).ToTypeInfo();
            var result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<byte>(result);
            Assert.AreEqual((byte)5, result);

            typeInfo = typeof(sbyte).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<sbyte>(result);
            Assert.AreEqual((sbyte)5, result);

            typeInfo = typeof(short).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<short>(result);
            Assert.AreEqual((short)5, result);

            typeInfo = typeof(ushort).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<ushort>(result);
            Assert.AreEqual((ushort)5, result);

            typeInfo = typeof(int).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<int>(result);
            Assert.AreEqual(5, result);

            typeInfo = typeof(uint).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<uint>(result);
            Assert.AreEqual((uint)5, result);

            typeInfo = typeof(long).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<long>(result);
            Assert.AreEqual((long)5, result);

            typeInfo = typeof(ulong).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<ulong>(result);
            Assert.AreEqual((ulong)5, result);

            typeInfo = typeof(BigInteger).ToTypeInfo();
            result = converter.ToClr(Core.Types.Integer.Of(5), typeInfo, null!);
            Assert.IsInstanceOfType<BigInteger>(result);
            Assert.AreEqual((BigInteger)5, result);
            #endregion

            #region Decimal
            typeInfo = typeof(Half).ToTypeInfo();
            result = converter.ToClr(Core.Types.Decimal.Of(5.12m), typeInfo, null!);
            Assert.IsInstanceOfType<Half>(result);
            Assert.AreEqual((Half)5.12m, result);

            typeInfo = typeof(Single).ToTypeInfo();
            result = converter.ToClr(Core.Types.Decimal.Of(5.12m), typeInfo, null!);
            Assert.IsInstanceOfType<Single>(result);
            Assert.AreEqual((Single)5.12m, result);

            typeInfo = typeof(Double).ToTypeInfo();
            result = converter.ToClr(Core.Types.Decimal.Of(5.12m), typeInfo, null!);
            Assert.IsInstanceOfType<Double>(result);
            Assert.AreEqual((Double)5.12m, result);

            typeInfo = typeof(decimal).ToTypeInfo();
            result = converter.ToClr(Core.Types.Decimal.Of(5.12m), typeInfo, null!);
            Assert.IsInstanceOfType<decimal>(result);
            Assert.AreEqual((decimal)5.12m, result);
            #endregion

            // Date Time
            var now = DateTimeOffset.Now;
            typeInfo = typeof(DateTimeOffset).ToTypeInfo();
            result = converter.ToClr(Core.Types.Timestamp.Of(now), typeInfo, null!);
            Assert.IsInstanceOfType<DateTimeOffset>(result);
            Assert.AreEqual(now, result);

            typeInfo = typeof(DateTime).ToTypeInfo();
            result = converter.ToClr(Core.Types.Timestamp.Of(now), typeInfo, null!);
            Assert.IsInstanceOfType<DateTime>(result);
            Assert.AreEqual(now.DateTime, result);

            // Time Span
            var fiveHours = TimeSpan.FromHours(5);
            typeInfo = typeof(TimeSpan).ToTypeInfo();
            result = converter.ToClr(Core.Types.Duration.Of(fiveHours), typeInfo, null!);
            Assert.IsInstanceOfType<TimeSpan>(result);
            Assert.AreEqual(fiveHours, result);

            #region Blob
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            typeInfo = typeof(byte[]).ToTypeInfo();
            result = converter.ToClr(Core.Types.Blob.Of(bytes), typeInfo, null!);
            Assert.IsInstanceOfType<byte[]>(result);
            CollectionAssert.AreEqual(bytes, (byte[])result);

            typeInfo = typeof(ImmutableArray<byte>).ToTypeInfo();
            result = converter.ToClr(Core.Types.Blob.Of(bytes), typeInfo, null!);
            Assert.IsInstanceOfType<ImmutableArray<byte>>(result);
            CollectionAssert.AreEqual(bytes, (ImmutableArray<byte>)result);

            typeInfo = typeof(ImmutableList<byte>).ToTypeInfo();
            result = converter.ToClr(Core.Types.Blob.Of(bytes), typeInfo, null!);
            Assert.IsInstanceOfType<ImmutableList<byte>>(result);
            CollectionAssert.AreEqual(bytes, (ImmutableList<byte>)result);

            typeInfo = typeof(IEnumerable<byte>).ToTypeInfo();
            result = converter.ToClr(Core.Types.Blob.Of(bytes), typeInfo, null!);
            Assert.IsInstanceOfType<IEnumerable<byte>>(result);
            Assert.IsTrue(bytes.SequenceEqual((IEnumerable<byte>)result));
            #endregion

        }
    }
}
