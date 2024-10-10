using Axis.Dia.Axon.Deserializers;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class TimestampDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => TimestampDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => TimestampDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => TimestampDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            var format = "yyyy-MM-dd HH:mm:ss.fffffff";

            var result = TimestampDeserializer.Deserialize("'T 2024'");
            Assert.AreEqual(DateTimeOffset.Parse("2024-01-01Z"), result.Value);

            result = TimestampDeserializer.Deserialize("'TS 2024-02'");
            Assert.AreEqual(DateTimeOffset.Parse("2024-02-01Z"), result.Value);

            result = TimestampDeserializer.Deserialize("'ts 2024-02-03'");
            Assert.AreEqual(DateTimeOffset.Parse("2024-02-03Z"), result.Value);

            result = TimestampDeserializer.Deserialize("'timestamp 2024-02-03 15'");
            Assert.AreEqual(DateTimeOffset.Parse("2024-02-03 15:00Z"), result.Value);

            result = TimestampDeserializer.Deserialize("'TS 2024-02-03 15:29'");
            Assert.AreEqual(DateTimeOffset.Parse("2024-02-03 15:29Z"), result.Value);

            result = TimestampDeserializer.Deserialize("'TS 2024-02-03 15:29:51'");
            Assert.AreEqual(DateTimeOffset.Parse("2024-02-03 15:29:51Z"), result.Value);

            result = TimestampDeserializer.Deserialize("'TS 2024-02-03 15:29:51.12345'");
            Assert.AreEqual(
                DateTimeOffset.Parse("2024-02-03 15:29:51.12345Z").ToString(format),
                result.Value!.Value.ToString(format));

            result = TimestampDeserializer.Deserialize("'TS 2024-02-03 15:29:51.001 +01:00'");
            Assert.AreEqual(
                DateTimeOffset.Parse("2024-02-03 15:29:51.001 +01:00").ToString(format),
                result.Value!.Value.ToString(format));

            result = TimestampDeserializer.Deserialize("@flag; 'TS 2024-02'");
            Assert.AreEqual(DateTimeOffset.Parse("2024-02-01Z"), result.Value);
            Assert.IsTrue(result.Attributes.Contains("flag"));

            result = TimestampDeserializer.Deserialize("@attribute;null.timestamp");
            Assert.IsTrue(result.IsNull);
            Assert.IsTrue(result.Attributes.Contains("attribute"));
        }
    }
}
