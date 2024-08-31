using Axis.Dia.AxonSerializer.Deserializers;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.AxonSerializer.Tests.Deserializers
{
    [TestClass]
    public class BlobDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => BlobDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => BlobDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => BlobDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            var result = BlobDeserializer.Deserialize("'B AQIDBAU='");
            Assert.AreEqual(5, result.Value!.Value.Length);

            var expectedValue = new byte[] { 1, 2, 3, 4, 5 };
            CollectionAssert.AreEqual(expectedValue, result.Value!.Value);

            result = BlobDeserializer.Deserialize("'B AQI' + 'DBAU='");
            Assert.AreEqual(5, result.Value!.Value.Length);
            CollectionAssert.AreEqual(expectedValue, result.Value!.Value);

            result = BlobDeserializer.Deserialize("@flag; 'B AQI' + 'DBAU='");
            Assert.AreEqual(5, result.Value!.Value.Length);
            Assert.AreEqual(1, result.Attributes.Count);
            Assert.IsTrue(result.Attributes.Contains("flag"));
            CollectionAssert.AreEqual(expectedValue, result.Value!.Value);

            result = BlobDeserializer.Deserialize("'blob '");
            Assert.AreEqual(0, result.Value!.Value.Length);

            result = BlobDeserializer.Deserialize("null.blob");
            Assert.IsTrue(result.IsNull);

            result = BlobDeserializer.Deserialize("@bleh:helb; null.blob");
            Assert.IsTrue(result.IsNull);
            Assert.IsTrue(result.Attributes.Contains(("bleh", "helb")));
        }
    }
}
