using Axis.Dia.Axon.Serializers;
using Axis.Dia.Axon;
using Axis.Dia.Core.Types;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class IntegerSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialize_ContextIsDefault_ThrowsArgumentException()
        {
            IntegerSerializer.Serialize(Integer.Null(), default);
        }

        [TestMethod]
        public void Serialize_ValueIsNull_ReturnsNullTypeText()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Integer.Null();

            var result = IntegerSerializer.Serialize(value, context);

            Assert.AreEqual("null.int", result);
        }

        [TestMethod]
        public void Serialize_ValueIsZero_ReturnsZeroDecimal()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Decimal)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(0);

            var result = IntegerSerializer.Serialize(value, context);

            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void Serialize_ValueIsZeroHexStyle_ReturnsZeroHex()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Hex)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(0);

            var result = IntegerSerializer.Serialize(value, context);

            Assert.AreEqual("0x0", result);
        }

        [TestMethod]
        public void Serialize_ValueIsZeroBinaryStyle_ReturnsZeroBinary()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Binary)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(0);

            var result = IntegerSerializer.Serialize(value, context);

            Assert.AreEqual("0b0", result);
        }

        [TestMethod]
        public void Serialize_ValueIsNonZeroDecimalStyle_ReturnsFormattedDecimal()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Decimal)
                .WithIntegerDigitSeparator(true)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(123456789);

            var result = IntegerSerializer.Serialize(value, context);

            Assert.AreEqual("123_456_789", result);
        }

        [TestMethod]
        public void Serialize_ValueIsNonZeroHexStyle_ReturnsFormattedHex()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Hex)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(255);

            var result = IntegerSerializer.Serialize(value, context);
            Assert.AreEqual("0xFF", result);
        }

        [TestMethod]
        public void Serialize_ValueIsNonZeroHexStyleDigitSeparator_ReturnsFormattedHex()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Hex)
                .WithIntegerDigitSeparator(true)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(25556678);

            var result = IntegerSerializer.Serialize(value, context);
            Assert.AreEqual("0x1_85_F6_C6", result);
        }

        [TestMethod]
        public void Serialize_ValueIsNonZeroBinaryStyle_ReturnsFormattedBinary()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Binary)
                .WithIntegerDigitSeparator(true)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(10);

            var result = IntegerSerializer.Serialize(value, context);

            Assert.AreEqual("0b1010", result);
        }

        [TestMethod]
        public void Serialize_ValueWithAttributes_ReturnsFormattedWithAttributes()
        {
            var options = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Decimal)
                .WithIntegerDigitSeparator(true)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.Integer.Of(42, "flag");

            var result = IntegerSerializer.Serialize(value, context);

            Assert.AreEqual("@flag; 42", result);
        }
    }
}
