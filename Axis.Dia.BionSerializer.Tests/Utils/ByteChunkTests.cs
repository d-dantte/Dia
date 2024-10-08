using Axis.Dia.BionSerializer.Utils;

namespace Axis.Dia.BionSerializer.Tests.Utils
{
    [TestClass]
    public class ByteChunkTests
    {
        [TestMethod]
        public void Constructor_ShouldCreateEmptyChunk_WhenNoBytesProvided()
        {
            // Arrange & Act
            var byteChunk = new ByteChunks();

            // Assert
            Assert.AreEqual(0, byteChunk.Length);
            Assert.IsTrue(byteChunk.IsDefault);
        }

        [TestMethod]
        public void Constructor_ShouldInitializeWithGivenBytes()
        {
            // Arrange
            byte[] bytes = [1, 2, 3];
            byte[] bytes2 = new byte[70_000];
            Random.Shared.NextBytes(bytes2);

            // Act
            var byteChunk = new ByteChunks(bytes);
            var byteChunk2 = new ByteChunks(bytes2);

            // Assert
            Assert.AreEqual(5, byteChunk.Length);
            CollectionAssert.AreEqual(bytes, byteChunk.ToRawBytes());
            Assert.AreEqual(70_004, byteChunk2.Length);
            CollectionAssert.AreEqual(bytes2, byteChunk2.ToRawBytes());
        }

        [TestMethod]
        public void Indexer_ShouldThrowException_WhenAccessingDefaultChunk()
        {
            // Arrange
            var byteChunk = ByteChunks.Default;

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(
                () => byteChunk[0]);
        }

        [TestMethod]
        public void Indexer_ShouldReturnCorrectByte()
        {
            // Arrange
            byte[] bytes = { 1, 2, 3 };
            var byteChunk = new ByteChunks(bytes);

            // Act & Assert
            Assert.AreEqual(3, byteChunk[0]);
            Assert.AreEqual(0, byteChunk[1]);
            Assert.AreEqual(1, byteChunk[2]);
            Assert.AreEqual(2, byteChunk[3]);
            Assert.AreEqual(3, byteChunk[4]);
        }

        [TestMethod]
        public void IsEmpty_ShouldReturnTrue_WhenChunkIsEmpty()
        {
            // Arrange
            var byteChunk = ByteChunks.Empty;

            // Act & Assert
            Assert.IsTrue(byteChunk.IsEmpty);
        }

        [TestMethod]
        public void IsEmpty_ShouldReturnFalse_WhenChunkIsNotEmpty()
        {
            // Arrange
            var byteChunk = new ByteChunks(new byte[] { 1, 2, 3 });

            // Act & Assert
            Assert.IsFalse(byteChunk.IsEmpty);
        }

        [TestMethod]
        public void Slice_ShouldReturnCorrectSlice()
        {
            // Arrange
            byte[] bytes = { 1, 2, 3, 4, 5 };
            var byteChunk = new ByteChunks(bytes);

            // Act
            var slice = byteChunk.Slice(1, 3);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0, 1, 2 }, slice);
        }

        [TestMethod]
        public void Slice_ShouldThrowException_WhenSlicingDefaultChunk()
        {
            // Arrange
            var byteChunk = ByteChunks.Default;

            // Act
            Assert.ThrowsException<InvalidOperationException>(
                () => byteChunk.Slice(0, 1));
        }

        [TestMethod]
        public void ValueEquals_ShouldReturnTrue_WhenChunksAreEqual()
        {
            // Arrange
            byte[] bytes = { 1, 2, 3 };
            var byteChunk1 = new ByteChunks(bytes);
            var byteChunk2 = new ByteChunks(bytes);

            // Act & Assert
            Assert.IsTrue(byteChunk1.ValueEquals(byteChunk2));
        }

        [TestMethod]
        public void ValueEquals_ShouldReturnFalse_WhenChunksAreNotEqual()
        {
            // Arrange
            var byteChunk1 = new ByteChunks(new byte[] { 1, 2, 3 });
            var byteChunk2 = new ByteChunks(new byte[] { 4, 5, 6 });

            // Act & Assert
            Assert.IsFalse(byteChunk1.ValueEquals(byteChunk2));
        }

        [TestMethod]
        public void GetEnumerator_ShouldReturnCorrectSequence()
        {
            // Arrange
            byte[] bytes = { 1, 2, 3 };
            var byteChunk = new ByteChunks(bytes);

            // Act
            var enumerator = byteChunk.GetEnumerator();

            // Assert
            var result = new List<byte>();
            while (enumerator.MoveNext())
            {
                result.Add(enumerator.Current);
            }

            byte[] expected = [3, 0, .. bytes];
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ToRawBytes_ShouldReturnCorrectRawBytes()
        {
            // Arrange
            byte[] bytes = { 1, 2, 3 };
            var byteChunk = new ByteChunks(bytes);

            // Act
            var rawBytes = byteChunk.ToRawBytes();

            // Assert
            CollectionAssert.AreEqual(bytes, rawBytes);
        }

        [TestMethod]
        public void Equals_ShouldCompareUnderlyingReference()
        {
            // Arrange
            byte[] bytes = { 1, 2, 3 };
            var byteChunk1 = new ByteChunks(bytes);
            var byteChunk2 = new ByteChunks(bytes);

            // Act & Assert
            Assert.IsFalse(byteChunk1.Equals(byteChunk2));
            Assert.IsTrue(byteChunk1.Equals(byteChunk1));
        }

        [TestMethod]
        public void ToString_ShouldReturnExpectedStringRepresentation()
        {
            // Arrange
            var byteChunk = new ByteChunks([1, 2, 3, 4, 5]);
            var byteChunk2 = new ByteChunks([1]);

            // Act
            var defaultResult = ByteChunks.Default.ToString();
            var emptyResult = ByteChunks.Empty.ToString();
            var result = byteChunk.ToString();
            var result2 = byteChunk2.ToString();

            // Assert
            Assert.AreEqual("ByteChunk{*}", defaultResult);
            Assert.AreEqual("ByteChunk{[0000 0000, 0000 0000]}", emptyResult);
            Assert.AreEqual("ByteChunk{[..., 0000 0010, 0000 0001, 0000 0000, 0000 0101]}", result);
            Assert.AreEqual("ByteChunk{[0000 0001, 0000 0000, 0000 0001]}", result2);
        }
    }
}
