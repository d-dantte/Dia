
using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class DecimalSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialize_ShouldThrowArgumentException_WhenContextIsDefault()
        {
            // Arrange
            var context = SerializerContext.Default;
            var decimalValue = Core.Types.Decimal.Of(123.45m);

            // Act
            DecimalSerializer.Serialize(decimalValue, context);
        }

        [TestMethod]
        public void Serialize_ShouldReturnNullTypeText_WhenValueIsNull()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var decimalValue = Core.Types.Decimal.Null();

            // Act
            var result = DecimalSerializer.Serialize(decimalValue, context);

            // Assert
            Assert.AreEqual("null.decimal", result);
        }

        [TestMethod]
        public void Serialize_ShouldReturnDecimalText_WhenValueIsNotNullAndNonScientific()
        {
            // Arrange
            var options = Options
                .Builder()
                .WithDecimalNotation(Options.DecimalNotation.NonScientific)
                .Build();
            var context = SerializerContext.Of(options);
            var decimalValue = Core.Types.Decimal.Of(123.45m);

            // Act
            var result = DecimalSerializer.Serialize(decimalValue, context);

            // Assert
            Assert.AreEqual("123.45", result);
        }

        [TestMethod]
        public void Serialize_ShouldReturnDecimalText_WhenValueIsNotNullAndScientific()
        {
            // Arrange
            var options = Options
                .Builder()
                .WithDecimalNotation(Options.DecimalNotation.Scientific)
                .Build();
            var context = SerializerContext.Of(options);
            var decimalValue = Core.Types.Decimal.Of(123.45m);

            // Act
            var result = DecimalSerializer.Serialize(decimalValue, context);

            // Assert
            Assert.AreEqual("1.2345E2", result);
        }

        [TestMethod]
        public void Serialize_ShouldIncludeAttributes_WhenAttributesAreNotEmpty()
        {
            // Arrange
            var options = Options
                .Builder()
                .WithDecimalNotation(Options.DecimalNotation.Scientific)
                .Build();
            var context = SerializerContext.Of(options);
            var decimalValue = Core.Types.Decimal.Of(123.45m, "flag");

            // Act
            var result = DecimalSerializer.Serialize(decimalValue, context);

            // Assert
            Assert.AreEqual("@flag; 1.2345E2", result);
        }
    }
}
