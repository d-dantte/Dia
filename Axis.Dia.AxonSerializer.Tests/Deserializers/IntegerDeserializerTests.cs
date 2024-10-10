using Axis.Dia.Axon.Deserializers;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class IntegerDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => IntegerDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => IntegerDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => IntegerDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            // Regular int
            var result = IntegerDeserializer.Deserialize("6543564");
            Assert.AreEqual(6543564, result.Value!);
            result = IntegerDeserializer.Deserialize("-6543564");
            Assert.AreEqual(-6543564, result.Value!);
            result = IntegerDeserializer.Deserialize("-6_543_564");
            Assert.AreEqual(-6543564, result.Value!);
            result = IntegerDeserializer.Deserialize("@flag; @galf; -6_543_564");
            Assert.AreEqual(-6543564, result.Value!);
            Assert.IsTrue(result.Attributes.Contains("flag"));
            Assert.IsTrue(result.Attributes.Contains("galf"));

            // Binary int
            result = IntegerDeserializer.Deserialize("0b101");
            Assert.AreEqual(5, result.Value!);
            result = IntegerDeserializer.Deserialize("0B10_0111_0001_0000");
            Assert.AreEqual(10000, result.Value!);
            result = IntegerDeserializer.Deserialize("@bleh; -0B1001110001_0000");
            Assert.AreEqual(-10000, result.Value!);
            Assert.IsTrue(result.Attributes.Contains("bleh"));

            // Hex int
            result = IntegerDeserializer.Deserialize("0xA");
            Assert.AreEqual(10, result.Value!);
            result = IntegerDeserializer.Deserialize("0xf_A");
            Assert.AreEqual(250, result.Value!);
            result = IntegerDeserializer.Deserialize("@bleh; -0XFF_2A_11_AC");
            Assert.AreEqual(-4280947116, result.Value!);
            Assert.IsTrue(result.Attributes.Contains("bleh"));

            // Null int
            result = IntegerDeserializer.Deserialize("null.int");
            Assert.IsTrue(result.IsNull);
            result = IntegerDeserializer.Deserialize("@h:x; null.int");
            Assert.IsTrue(result.IsNull);
            Assert.IsTrue(result.Attributes.Contains(("h", "x")));
        }
    }
}
