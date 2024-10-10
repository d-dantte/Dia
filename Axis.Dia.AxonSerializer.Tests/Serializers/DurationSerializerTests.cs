using Axis.Dia.Axon.Serializers;
using Axis.Dia.Axon;
using Axis.Dia.Core.Types;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class DurationSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialize_ShouldThrowArgumentException_WhenContextIsDefault()
        {
            // Arrange
            var durationValue = Duration.Of(TimeSpan.FromHours(1.2));
            var context = SerializerContext.Default;

            // Act
            DurationSerializer.Serialize(durationValue, context);
        }

        [TestMethod]
        public void Serialize_ShouldReturnNullTypeText_WhenValueIsNull()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var durationValue = Duration.Null();

            // Act
            var result = DurationSerializer.Serialize(durationValue, context);

            // Assert
            Assert.AreEqual("null.duration", result);
        }

        [TestMethod]
        public void Serialize_ShouldReturnFormattedDuration_WhenValueIsNotNull()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);

            // 1 day, 2 hours, 3 minutes, 4 seconds, 567 milliseconds
            var durationValue = Duration.Of(new TimeSpan(1, 2, 3, 4, 567));

            // Act
            var result = DurationSerializer.Serialize(durationValue, context);

            // Assert
            Assert.AreEqual("'Duration 01.02:03:04.5670000'", result);
        }

        [TestMethod]
        public void Serialize_ShouldIncludeAttributes_WhenAttributesAreNotEmpty()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var durationValue = Core.Types.Duration.Of(
                new TimeSpan(0, 2, 3, 4, 567), // 1 day, 2 hours, 3 minutes, 4 seconds, 567 milliseconds
                "flag");

            // Act
            var result = DurationSerializer.Serialize(durationValue, context);

            // Assert
            Assert.AreEqual("@flag; 'Duration 00.02:03:04.5670000'", result);
        }
    }
}
