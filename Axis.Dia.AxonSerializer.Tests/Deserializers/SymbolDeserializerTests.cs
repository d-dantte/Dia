using Axis.Dia.Axon.Deserializers;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class SymbolDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => SymbolDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => SymbolDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => SymbolDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            var result = SymbolDeserializer.Deserialize("'Symbol Random_text'");
            Assert.AreEqual("Random_text", result.Value);

            result = SymbolDeserializer.Deserialize("'sym text \\n and \\xac'");
            Assert.AreEqual("text \n and \xac", result.Value);

            result = SymbolDeserializer.Deserialize("@flag;'s text \\n and \\xac'");
            Assert.AreEqual("text \n and \xac", result.Value);
            Assert.IsTrue(result.Attributes.Contains("flag"));

            result = SymbolDeserializer.Deserialize("@things; null.symbol");
            Assert.IsTrue(result.IsNull);
            Assert.IsTrue(result.Attributes.Contains("things"));
        }
    }
}
