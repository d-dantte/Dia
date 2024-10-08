using Axis.Dia.Core.Contracts;
using Axis.Dia.TypeConverter.Dia;
using Axis.Luna.Extensions;
using Axis.Luna.Numerics;
using System.Collections.Immutable;
using System.Numerics;

namespace Axis.Dia.TypeConverter.Tests.Dia
{
    [TestClass]
    public class SimpleConverterTests
    {
        [TestMethod]
        public void CanConvert_Tests()
        {
            var converter = new SimpleTypeConverter();
            var boolInfo = typeof(bool).ToTypeInfo();
            var objectInfo = typeof(object).ToTypeInfo();

            Assert.ThrowsException<ArgumentException>(
                () => converter.CanConvert(default));

            Assert.IsTrue(converter.CanConvert(boolInfo));
            Assert.IsFalse(converter.CanConvert(objectInfo));
        }

        [TestMethod]
        public void ToDia_WithInvalidSourcetype_Tests()
        {
            var converter = new SimpleTypeConverter();
            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeof(object).ToTypeInfo(), null, default!));
        }

        [TestMethod]
        public void ToDia_WithBoolean_Tests()
        {
            var converter = new SimpleTypeConverter();
            var typeInfo = typeof(bool).ToTypeInfo();
            var objectInfo = typeof(object).ToTypeInfo();

            var result = converter.ToDia(typeInfo, null, default!);
            Assert.IsNotNull(result);
            Assert.AreEqual(Core.DiaType.Bool, result.Type);
            Assert.IsTrue(result.As<INullable>().IsNull);

            result = converter.ToDia(typeInfo, true, default!);
            Assert.IsTrue(result.Is(out Core.Types.Boolean @bool));
            Assert.IsTrue(@bool.Value);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, "", default!));
        }

        [TestMethod]
        public void ToDia_WithString_Tests()
        {
            var converter = new SimpleTypeConverter();
            var typeInfo = typeof(string).ToTypeInfo();
            var objectInfo = typeof(object).ToTypeInfo();

            var result = converter.ToDia(typeInfo, null, default!);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Is(out Core.Types.String @string));
            Assert.IsTrue(@string.IsNull);

            result = converter.ToDia(typeInfo, "foo", default!);
            Assert.IsTrue(result.Is(out @string));
            Assert.AreEqual("foo", @string.Value);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, default!));
        }

        [TestMethod]
        public void ToDia_WithInteger_Tests()
        {
            var converter = new SimpleTypeConverter();
            var objectInfo = typeof(object).ToTypeInfo();

            // null
            var typeInfo = typeof(int).ToTypeInfo();
            var result = converter.ToDia(typeInfo, null, default!);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Is(out Core.Types.Integer @int));
            Assert.IsTrue(@int.IsNull);

            // byte
            result = converter.ToDia(typeInfo, byte.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(byte.MaxValue, @int.Value);

            // sbyte
            result = converter.ToDia(typeInfo, sbyte.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(sbyte.MaxValue, @int.Value);

            // short
            result = converter.ToDia(typeInfo, short.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(short.MaxValue, @int.Value);

            // ushort
            result = converter.ToDia(typeInfo, ushort.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(ushort.MaxValue, @int.Value);

            // int
            result = converter.ToDia(typeInfo, int.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(int.MaxValue, @int.Value);

            // uint
            result = converter.ToDia(typeInfo, uint.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(uint.MaxValue, @int.Value);

            // long
            result = converter.ToDia(typeInfo, long.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(long.MaxValue, @int.Value);

            // uint
            result = converter.ToDia(typeInfo, uint.MaxValue, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(uint.MaxValue, @int.Value);

            // BigInt
            result = converter.ToDia(typeInfo, BigInteger.One, default!);
            Assert.IsTrue(result.Is(out @int));
            Assert.AreEqual(BigInteger.One, @int.Value);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, default!));
        }

        [TestMethod]
        public void ToDia_WithDecimal_Tests()
        {
            var converter = new SimpleTypeConverter();
            var objectInfo = typeof(object).ToTypeInfo();

            // null
            var typeInfo = typeof(decimal).ToTypeInfo();
            var result = converter.ToDia(typeInfo, null, default!);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Is(out Core.Types.Decimal @decimal));
            Assert.IsTrue(@decimal.IsNull);

            // half
            result = converter.ToDia(typeInfo, Half.MaxValue, default!);
            Assert.IsTrue(result.Is(out @decimal));
            Assert.AreEqual(Half.MaxValue, @decimal.Value);

            // float
            result = converter.ToDia(typeInfo, float.MaxValue, default!);
            Assert.IsTrue(result.Is(out @decimal));
            Assert.AreEqual(float.MaxValue, @decimal.Value);

            // double
            result = converter.ToDia(typeInfo, double.MaxValue, default!);
            Assert.IsTrue(result.Is(out @decimal));
            Assert.AreEqual(double.MaxValue, @decimal.Value);

            // decimal
            result = converter.ToDia(typeInfo, decimal.MaxValue, default!);
            Assert.IsTrue(result.Is(out @decimal));
            Assert.AreEqual(decimal.MaxValue, @decimal.Value);

            // BigDecimal
            result = converter.ToDia(typeInfo, BigDecimal.One, default!);
            Assert.IsTrue(result.Is(out @decimal));
            Assert.AreEqual(BigDecimal.One, @decimal.Value);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, default!));
        }

        [TestMethod]
        public void ToDia_WithDateTime_Tests()
        {
            var converter = new SimpleTypeConverter();
            var objectInfo = typeof(object).ToTypeInfo();

            // null
            var typeInfo = typeof(DateTimeOffset).ToTypeInfo();
            var result = converter.ToDia(typeInfo, null, default!);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Is(out Core.Types.Timestamp timestamp));
            Assert.IsTrue(timestamp.IsNull);

            // DateTime
            var dtNow = DateTime.Now;
            result = converter.ToDia(typeInfo, dtNow, default!);
            Assert.IsTrue(result.Is(out timestamp));
            Assert.AreEqual(dtNow, timestamp.Value);

            // DateTimeOffset
            var dtoNow = DateTimeOffset.Now;
            result = converter.ToDia(typeInfo, dtoNow, default!);
            Assert.IsTrue(result.Is(out timestamp));
            Assert.AreEqual(dtoNow, timestamp.Value);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, default!));
        }

        [TestMethod]
        public void ToDia_WithTimespan_Tests()
        {
            var converter = new SimpleTypeConverter();
            var objectInfo = typeof(object).ToTypeInfo();

            // null
            var typeInfo = typeof(TimeSpan).ToTypeInfo();
            var result = converter.ToDia(typeInfo, null, default!);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Is(out Core.Types.Duration duration));
            Assert.IsTrue(duration.IsNull);

            // Timespan
            var ts = TimeSpan.FromSeconds(23344566);
            result = converter.ToDia(typeInfo, ts, default!);
            Assert.IsTrue(result.Is(out duration));
            Assert.AreEqual(ts, duration.Value);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, default!));
        }

        [TestMethod]
        public void ToDia_WithByteSequence_Tests()
        {
            var converter = new SimpleTypeConverter();
            var objectInfo = typeof(object).ToTypeInfo();

            // null
            var typeInfo = typeof(byte[]).ToTypeInfo();
            var result = converter.ToDia(typeInfo, null, default!);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Is(out Core.Types.Blob blob));
            Assert.IsTrue(blob.IsNull);

            // byte array
            var barr = new byte[] { 0, 1, 2, 3 };
            result = converter.ToDia(typeInfo, barr, default!);
            Assert.IsTrue(result.Is(out blob));
            CollectionAssert.AreEqual(barr, blob.Value);

            // immutable byte array
            var imbarr = ImmutableArray.Create<byte>(0, 1, 2, 3);
            result = converter.ToDia(typeInfo, imbarr, default!);
            Assert.IsTrue(result.Is(out blob));
            CollectionAssert.AreEqual(imbarr, blob.Value);

            // immutable byte list
            var imlist = ImmutableList.Create<byte>(0, 1, 2, 3);
            result = converter.ToDia(typeInfo, imlist, default!);
            Assert.IsTrue(result.Is(out blob));
            CollectionAssert.AreEqual(imlist, blob.Value);

            // byte list
            var list = new List<byte> { 0, 1, 2, 3 };
            result = converter.ToDia(typeInfo, list, default!);
            Assert.IsTrue(result.Is(out blob));
            CollectionAssert.AreEqual(list, blob.Value);

            // all other (ordered) enumerables
            var enm = Enumerable.Range(0, 4).Select(i => (byte)i);
            result = converter.ToDia(typeInfo, enm, default!);
            Assert.IsTrue(result.Is(out blob));
            Assert.IsTrue(enm.SequenceEqual(blob.Value!));

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, default!));
        }
    }
}
