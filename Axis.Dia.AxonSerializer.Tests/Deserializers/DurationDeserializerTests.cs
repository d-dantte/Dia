using Axis.Dia.AxonSerializer.Deserializers;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.AxonSerializer.Tests.Deserializers
{
    [TestClass]
    public class DurationDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => DurationDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => DurationDeserializer.Deserialize(default(ISymbolNode)!));

            Assert.ThrowsException<ArgumentException>(
                () => DurationDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            var result = DurationDeserializer.Deserialize("'D 23:45:06'");
            Assert.AreEqual(TimeSpan.Parse("23:45:06"), result.Value);

            result = DurationDeserializer.Deserialize("@flag; 'duration 2.00:11:56'");
            Assert.AreEqual(TimeSpan.Parse("2.00:11:56"), result.Value);
            Assert.IsTrue(result.Attributes.Contains("flag"));

            result = DurationDeserializer.Deserialize("@flag; 'duration 32.10:11:50.543654'");
            Assert.AreEqual(TimeSpan.Parse("32.10:11:50.543654"), result.Value);
        }
    }
}
