using Axis.Dia.Axon.Deserializers;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class StringDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => StringDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => StringDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => StringDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            // Singleline String
            var result = StringDeserializer.Deserialize("\"\"");
            Assert.AreEqual(string.Empty, result.Value!);

            result = StringDeserializer.Deserialize("\"abra ka dabra\"");
            Assert.AreEqual("abra ka dabra", result.Value!);

            result = StringDeserializer.Deserialize("@flag; \"ab\\nra\" + \" ka\" + \" dabra\"");
            Assert.AreEqual("ab\nra ka dabra", result.Value!);
            Assert.IsTrue(result.Attributes.Contains("flag"));

            // MLString
            result = StringDeserializer.Deserialize("``");
            Assert.AreEqual(string.Empty, result.Value!);

            result = StringDeserializer.Deserialize("`stuff here`");
            Assert.AreEqual("stuff here", result.Value!);

            result = StringDeserializer.Deserialize(@"`stuff
here`");
            Assert.AreEqual("stuff\r\nhere", result.Value!);

            result = StringDeserializer.Deserialize(@"@flag;`stuff \
    here`");
            Assert.AreEqual("stuff here", result.Value!);
            Assert.IsTrue(result.Attributes.Contains("flag"));

            // null
            result = StringDeserializer.Deserialize("@helb;null.string");
            Assert.IsTrue(result.IsNull);
            Assert.IsTrue(result.Attributes.Contains("helb"));
        }
    }
}
