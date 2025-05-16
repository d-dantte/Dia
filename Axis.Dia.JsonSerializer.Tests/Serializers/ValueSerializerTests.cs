using Axis.Dia.Core.Types;
using Axis.Dia.Json.Serializers;
using Axis.Luna.Numerics;
using Axis.Luna.Result;

namespace Axis.Dia.Json.Tests.Serializers
{
    [TestClass]
    public class ValueSerializerTests
    {
        private SerializerContext _context = null!;

        [TestInitialize]
        public void Setup()
        {
            _context = new SerializerContext();
        }

        #region SerializeBlob

        [TestMethod]
        public void SerializeBlob_IsNull_And_NoAttributes_ReturnsExpected()
        {
            // Arrange
            var blob = Blob.Null();

            // Act
            var result = ValueSerializer.SerializeBlob(blob, _context);

            // Assert
            Assert.AreEqual("#Blob.null", result.ToString());
        }

        [TestMethod]
        public void SerializeBlob_IsNull_And_WithAttributes_ReturnsExpected()
        {
            // Arrange
            var blob = Blob.Null(new Core.Types.Attribute("alt", "42"));

            // Act
            var result = ValueSerializer.SerializeBlob(blob, _context);

            // Assert
            Assert.AreEqual("#Blob.null[@alt:42;]", result.ToString());
        }

        [TestMethod]
        public void SerializeBlob_NotNull_And_NoAttributes_ReturnsExpected()
        {
            // Arrange
            var data = new byte[] { 1, 2, 3 };
            var blob = Blob.Of(data);

            // Act
            var result = ValueSerializer.SerializeBlob(blob, _context);

            // Assert
            Assert.AreEqual($"#Blob {Convert.ToBase64String(data)}", result.ToString());
        }

        [TestMethod]
        public void SerializeBlob_NotNull_And_WithAttributes_ReturnsExpected()
        {
            // Arrange
            var data = new byte[] { 1, 2, 3, 4 };
            var blob = Blob.Of(data, Core.Types.Attribute.Of("k", "v"));

            // Act
            var result = ValueSerializer.SerializeBlob(blob, _context);

            // Assert
            Assert.AreEqual($"#Blob[@k:v;] {Convert.ToBase64String(data)}", result.ToString());
        }

        #endregion

        #region SerializeBool

        [TestMethod]
        public void SerializeBool_ShouldSerialize_NullNoAttributes()
        {
            // Arrange
            var value = Core.Types.Boolean.Null();

            // Act
            var result = ValueSerializer.SerializeBool(value, _context);

            // Assert
            Assert.AreEqual("#Bool.null", result.ToString());
        }

        [TestMethod]
        public void SerializeBool_ShouldSerialize_NullWithAttributes()
        {
            // Arrange
            var value = Core.Types.Boolean.Null(
                Core.Types.Attribute.Of("key", "value"));

            // Act
            var result = ValueSerializer.SerializeBool(value, _context);

            // Assert
            Assert.AreEqual("#Bool.null[@key:value;]", result.ToString());
        }

        [TestMethod]
        public void SerializeBool_ShouldSerialize_ValueFalseNoAttributes()
        {
            // Arrange
            var value = new Core.Types.Boolean(false);

            // Act
            var result = ValueSerializer.SerializeBool(value, _context);

            // Assert
            Assert.AreEqual("False", result.ToString());
        }

        [TestMethod]
        public void SerializeBool_ShouldSerialize_ValueTrueNoAttributes()
        {
            // Arrange
            var value = new Core.Types.Boolean(true);

            // Act
            var result = ValueSerializer.SerializeBool(value, _context);

            // Assert
            Assert.AreEqual("True", result.ToString());
        }

        [TestMethod]
        public void SerializeBool_ShouldSerialize_ValueTrueWithAttributes()
        {
            // Arrange
            var value = new Core.Types.Boolean(
                true, Core.Types.Attribute.Of("flag"));

            // Act
            var result = ValueSerializer.SerializeBool(value, _context);

            // Assert
            Assert.AreEqual("#Bool[@flag;] True", result.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeBool_ShouldThrow_OnNullContext()
        {
            // Arrange
            var value = new Core.Types.Boolean(true);

            // Act
            _ = ValueSerializer.SerializeBool(value, null!);
        }

        #endregion

        #region SerializeDecimal

        [TestMethod]
        public void SerializeDecimal_WithNullValueAndAttributes_ReturnsExpectedString()
        {
            // Arrange
            var value = Core.Types.Decimal.Null(Core.Types.Attribute.Of("a", "b"));
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDecimal(value, context);

            // Assert
            Assert.AreEqual("#Decimal.null[@a:b;]", result.ToString());
        }

        [TestMethod]
        public void SerializeDecimal_WithNullValueAndNoAttributes_ReturnsExpectedString()
        {
            // Arrange
            var value = Core.Types.Decimal.Null();
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDecimal(value, context);

            // Assert
            Assert.AreEqual("#Decimal.null", result.ToString());
        }

        [TestMethod]
        public void SerializeDecimal_WithFiniteValueAndAttributes_ReturnsExpectedString()
        {
            // Arrange
            var value = Core.Types.Decimal.Of(123.45m, Core.Types.Attribute.Of("x", "y"));
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDecimal(value, context);

            // Assert
            Assert.AreEqual("#Decimal[@x:y;] 1.2345E2", result.ToString());
        }

        [TestMethod]
        public void SerializeDecimal_WithFiniteValueAndNoAttributes_ReturnsDoubleJValue()
        {
            // Arrange
            var value = Core.Types.Decimal.Of(123.45m);
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDecimal(value, context);

            // Assert
            Assert.AreEqual(123.45, (double)result);
        }

        [TestMethod]
        public void SerializeDecimal_WithLargeValueAndAttributes_ReturnsScientificNotation()
        {
            // Arrange
            var value = Core.Types.Decimal.Of(BigDecimal.Parse("1.0E400").Resolve(), Core.Types.Attribute.Of("foo", "bar"));
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDecimal(value, context);

            // Assert
            Assert.AreEqual(result.ToString(), "#Decimal[@foo:bar;] 1.0E400");
        }

        [TestMethod]
        public void SerializeDecimal_WithLargeValueAndNoAttributes_ReturnsScientificNotation()
        {
            // Arrange
            var value = Core.Types.Decimal.Of(BigDecimal.Parse("1.0E400").Resolve());
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDecimal(value, context);

            // Assert
            Assert.AreEqual(result.ToString(), "#Decimal 1.0E400");
        }

        #endregion

        #region SerializeDuration

        [TestMethod]
        public void SerializeDuration_WithNullValueAndAttributes_ReturnsExpectedString()
        {
            // Arrange
            var value = Core.Types.Duration.Null(Core.Types.Attributes.Of("x", "y"));
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDuration(value, context);

            // Assert
            Assert.AreEqual("#Duration.null[@x:y;]", result.ToString());
        }

        [TestMethod]
        public void SerializeDuration_WithNullValueAndNoAttributes_ReturnsExpectedString()
        {
            // Arrange
            var value = Core.Types.Duration.Null();
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDuration(value, context);

            // Assert
            Assert.AreEqual("#Duration.null", result.ToString());
        }

        [TestMethod]
        public void SerializeDuration_WithFiniteValueAndAttributes_ReturnsExpectedString()
        {
            // Arrange
            var duration = TimeSpan.FromHours(2.5); // 2h30m
            var value = Core.Types.Duration.Of(duration, Core.Types.Attributes.Of("a", "b"));
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDuration(value, context);

            // Assert
            Assert.AreEqual($"#Duration[@a:b;] {duration:c}", result.ToString());
        }

        [TestMethod]
        public void SerializeDuration_WithFiniteValueAndNoAttributes_ReturnsTimeSpanJValue()
        {
            // Arrange
            var duration = TimeSpan.FromMinutes(90);
            var value = Core.Types.Duration.Of(duration);
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDuration(value, context);

            // Assert
            Assert.AreEqual(duration.ToString("c"), result.ToString());
        }

        [TestMethod]
        public void SerializeDuration_WithZeroValue_ReturnsExpectedFormat()
        {
            // Arrange
            var duration = TimeSpan.Zero;
            var value = Core.Types.Duration.Of(duration);
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDuration(value, context);

            // Assert
            Assert.AreEqual("00:00:00", result.ToString());
        }

        [TestMethod]
        public void SerializeDuration_WithNegativeValueAndAttributes_ReturnsExpectedString()
        {
            // Arrange
            var duration = TimeSpan.FromMinutes(-45);
            var value = Core.Types.Duration.Of(duration, Core.Types.Attributes.Of("n", "v"));
            var context = new SerializerContext();

            // Act
            var result = ValueSerializer.SerializeDuration(value, context);

            // Assert
            Assert.AreEqual($"#Duration[@n:v;] {duration:c}", result.ToString());
        }

        #endregion
    }
}
