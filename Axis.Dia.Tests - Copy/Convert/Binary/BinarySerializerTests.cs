using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Tests.Convert.Binary
{
    [TestClass]
    public class BinarySerializerTests
    {
        [TestMethod]
        public void TestSerialization()
        {
            var packet = default(ValuePacket);
            var result = BinarySerializer.Serialize(packet);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDataResult());
            Assert.IsTrue(result.IsDataResult(out var bytes));
            Assert.IsNotNull(bytes);
            Assert.AreEqual(0, bytes.Length);

            packet = ValuePacket.Of(
                BoolValue.Of(true),
                InstantValue.Of(DateTimeOffset.Now),
                StringValue.Of("ready player two"),
                ListValue.Of(
                    ListValue.Empty(),
                    IntValue.Of(3443, "abc", "def"),
                    DecimalValue.Of(11.12212)),
                new RecordValue
                {
                    ["show"] = true,
                    ["me"] = SymbolValue.Of("plenty"),
                    ["the"] = ClobValue.Of("very plenty", "one", "two"),
                    ["things"] = BlobValue.Of(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 })
                });
            packet.Values[4].As<RecordValue>()["self"] = ReferenceValue.Of(packet.Values[4].As<RecordValue>());
            result = BinarySerializer.Serialize(packet);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDataResult());
            bytes = result.Resolve();
            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length > 0);
        }

        [TestMethod]
        public void TestDeserialization()
        {
            var packet = default(ValuePacket);
            var result = BinarySerializer.Serialize(packet);
            var stream = result
                .Map(bytes => new MemoryStream(bytes))
                .Resolve();
            var deserializedResult = BinarySerializer.Deserialize(stream);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDataResult());
            Assert.IsTrue(deserializedResult.IsDataResult(out var deserializedPacket));
            Assert.IsNotNull(deserializedPacket);
            Assert.IsTrue(Enumerable.SequenceEqual(
                packet.Values,
                deserializedPacket.Values));

            packet = ValuePacket.Of(
                BoolValue.Of(true),
                InstantValue.Of(DateTimeOffset.Now),
                StringValue.Of("ready player two"),
                ListValue.Of(
                    ListValue.Empty(),
                    IntValue.Of(3443, "abc", "def"),
                    DecimalValue.Of(11.12212m)),
                new RecordValue
                {
                    ["show"] = true,
                    ["me"] = SymbolValue.Of("plenty"),
                    ["the"] = ClobValue.Of("very plenty", "one", "two"),
                    ["things"] = BlobValue.Of(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 })
                });
            result = BinarySerializer.Serialize(packet);
            stream = result
                .Map(bytes => new MemoryStream(bytes))
                .Resolve();
            deserializedResult = BinarySerializer.Deserialize(stream);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDataResult());
            Assert.IsTrue(deserializedResult.IsDataResult(out deserializedPacket));
            Assert.IsNotNull(deserializedPacket);
            Assert.IsTrue(Enumerable.SequenceEqual(
                packet.Values,
                deserializedPacket.Values));

            var value = new RecordValue
            {
                ["show"] = true,
                ["me"] = SymbolValue.Of("plenty"),
                ["the"] = ClobValue.Of("very plenty", "one", "two"),
                ["things"] = BlobValue.Of(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 })
            };
            value["self"] = ReferenceValue.Of(value);
            result = PayloadSerializer.SerializeDiaValueResult(value, new SerializerContext());
            stream = result
                .Map(bytes => new MemoryStream(bytes))
                .Resolve();
            deserializedResult = BinarySerializer.Deserialize(stream);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDataResult());
            Assert.IsTrue(deserializedResult.IsDataResult(out deserializedPacket));
            Assert.IsNotNull(deserializedPacket);
            Assert.AreEqual(1, deserializedPacket.Values.Length);
            Assert.AreEqual(DiaType.Record, deserializedPacket.Values[0].Type);
        }
    }
}
