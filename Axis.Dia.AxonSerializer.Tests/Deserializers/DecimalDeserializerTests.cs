using Axis.Dia.Axon.Deserializers;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class DecimalDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => DecimalDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => DecimalDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => DecimalDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            var result = DecimalDeserializer.Deserialize("456.765");
            Assert.AreEqual(456.765m, result.Value);

            result = DecimalDeserializer.Deserialize("@flag; -32.5e-12");
            Assert.AreEqual("-3.25E-11", result.Value.ToString());
            Assert.IsTrue(result.Attributes.Contains("flag"));
        }
    }
}
