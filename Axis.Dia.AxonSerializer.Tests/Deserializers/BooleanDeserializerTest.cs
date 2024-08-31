using Axis.Dia.AxonSerializer.Deserializers;
using Axis.Pulsar.Core.CST;
namespace Axis.Dia.AxonSerializer.Tests.Deserializers
{
    [TestClass]
    public class BooleanDeserializerTest
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => BooleanDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => BooleanDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => BooleanDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            var result = BooleanDeserializer.Deserialize("true");
            Assert.AreEqual(true, result.Value);
            result = BooleanDeserializer.Deserialize("True");
            Assert.AreEqual(true, result.Value);
            result = BooleanDeserializer.Deserialize("TRUE");
            Assert.AreEqual(true, result.Value);
                
            result = BooleanDeserializer.Deserialize("false");
            Assert.AreEqual(false, result.Value);
            result = BooleanDeserializer.Deserialize("falsE");
            Assert.AreEqual(false, result.Value);
            result = BooleanDeserializer.Deserialize("@false; FALSE");
            Assert.AreEqual(false, result.Value);
            Assert.AreEqual(1, result.Attributes.Count);
            Assert.IsTrue(result.Attributes.Contains("false"));

            result = BooleanDeserializer.Deserialize("null.bool");
            Assert.IsTrue(result.IsNull);
            Assert.IsNull(result.Value);
        }
    }
}
