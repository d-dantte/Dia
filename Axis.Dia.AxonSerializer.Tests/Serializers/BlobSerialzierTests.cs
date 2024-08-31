using Axis.Dia.Axon.Serializers;
using Axis.Dia.Axon;

namespace Axis.Dia.AxonSerializer.Tests.Serializers
{
    [TestClass]
    public class BlobSerialzierTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid context: default")]
        public void Serialize_ShouldThrowArgumentException_WhenContextIsDefault()
        {
            // Arrange
            var blob = Core.Types.Blob.Of([], []);

            // Act
            BlobSerializer.Serialize(blob, SerializerContext.Default);
        }

        [TestMethod]
        public void Serialize_ShouldReturnNullTypeText_WhenBlobIsNull()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var blob = Core.Types.Blob.Null();

            // Act
            var result = BlobSerializer.Serialize(blob, context);

            // Assert
            Assert.AreEqual($"null.blob", result);
        }

        [TestMethod]
        public void Serialize_ShouldReturnBase64Blob_WhenBlobIsNotNull()
        {
            // Arrange
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var blob = Core.Types.Blob.Of([1, 2, 3, 4], []);

            // Act
            var result = BlobSerializer.Serialize(blob, context);

            // Assert
            Assert.AreEqual("'Blob AQIDBA=='", result);
        }

        [TestMethod]
        public void Serialize_ShouldReturnIndentedBlob_WhenIndentationIsNotNone()
        {
            // Arrange
            var options = Options
                .Builder()
                .WithBlobSinglelineCharacterCount(20)
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .Build();
            var context = SerializerContext.Of(options);
            var blob = Core.Types.Blob.Of(
                [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20],
                []);

            // Act
            var result = BlobSerializer.Serialize(blob, context);

            // Assert
            var expected = "'Blob AQIDBAUGBwgJCgsMDQ4P'\r\n+ 'EBESExQ='";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_ShouldIncludeAttributes_WhenAttributesArePresent()
        {
            // Arrange
            var options = Options
                .Builder()
                .WithBlobSinglelineCharacterCount(100)
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .Build();
            var context = SerializerContext.Of(options);
            var blob = Core.Types.Blob.Of(
                [1, 2, 3, 4],
                ("attr1", "value1"),
                ("attr2", "value2"));

            // Act
            var result = BlobSerializer.Serialize(blob, context);

            // Assert
            var expected = "@attr1:value1; @attr2:value2; 'Blob AQIDBA=='";
            Assert.AreEqual(expected, result);
        }
    }
}
