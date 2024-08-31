using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;

namespace Axis.Dia.AxonSerializer.Tests.Serializers
{
    [TestClass]
    public class BooleanSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid context: default")]
        public void Serialize_ShouldThrowArgumentException_WhenContextIsDefault()
        {
            // Arrange
            var booleanValue = Core.Types.Boolean.Of(true);
            var context = SerializerContext.Default;

            // Act
            BooleanSerializer.Serialize(booleanValue, context);
        }

        [TestMethod]
        public void Serialize_ShouldReturnNullTypeText_WhenValueIsNull()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var booleanValue = Core.Types.Boolean.Null();

            // Act
            var result = BooleanSerializer.Serialize(booleanValue, context);

            // Assert
            Assert.AreEqual("null.bool", result);
        }

        [TestMethod]
        public void Serialize_ShouldReturnTrue_WhenValueIsTrue()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var booleanValue = Core.Types.Boolean.Of(true);

            // Act
            var result = BooleanSerializer.Serialize(booleanValue, context);

            // Assert
            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void Serialize_ShouldReturnFalse_WhenValueIsFalse()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var booleanValue = Core.Types.Boolean.Of(false);

            // Act
            var result = BooleanSerializer.Serialize(booleanValue, context);

            // Assert
            Assert.AreEqual("false", result);
        }

        [TestMethod]
        public void Serialize_ShouldIncludeAttributes_WhenAttributesAreNotEmpty()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var booleanValue = Core.Types.Boolean.Of(true, "flag");

            // Act
            var result = BooleanSerializer.Serialize(booleanValue, context);

            // Assert
            Assert.AreEqual("@flag; true", result);
        }
    }
}
