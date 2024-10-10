using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Core.Types;

namespace Axis.Dia.Bion.Tests.Serializers
{
    [TestClass]
    public class DiaRecordSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaRecordSerializer.DefaultInstance;

            var symbol = Core.Types.Record.Null();
            var tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Record, tmeta.Type);

            symbol = Core.Types.Record.Null("att");
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Record, tmeta.Type);

            symbol = Core.Types.Record.Empty();
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Record, tmeta.Type);

            symbol = new Core.Types.Record
            {
                ["first"] = "string",
                ["second"] = 45.6m,
                [Record.PropertyName.Of("third", ("att", "key"))] = DateTimeOffset.Now
            };
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Record, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaRecordSerializer.DefaultInstance;

            var value = Core.Types.Record.Empty();
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 11 },
                context.Buffer.StreamData);

            value = Core.Types.Record.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 43 },
                context.Buffer.StreamData);

            value = new Core.Types.Record
            {
                ["first"] = "string",
                ["second"] = 45.6m,
                [Record.PropertyName.Of("third", ("att", "key"))] = TimeSpan.FromSeconds(34442)
            };
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(96, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] {
                    75, 131, 0, 71, 10, 0, 102, 0, 105, 0, 114, 0, 115, 0, 116, 0, 71, 12, 0, 115, 0, 116, 0, 114, 0,
                    105, 0, 110, 0, 103, 0, 71, 12, 0, 115, 0, 101, 0, 99, 0, 111, 0, 110, 0, 100, 0, 196, 1, 129, 0,
                    2, 0, 200, 1, 87, 1, 65, 6, 0, 97, 0, 116, 0, 116, 0, 6, 0, 107, 0, 101, 0, 121, 0, 10, 0, 116, 0,
                    104, 0, 105, 0, 114, 0, 100, 0, 69, 8, 0, 0, 228, 44, 39, 83, 31, 0, 0
                },
                context.Buffer.StreamData);

            value = new Core.Types.Record(new Core.Types.Attribute[] { ("abc", "xyz") }, [])
            {
                ["first"] = "string",
                ["second"] = 45.6m,
                [Record.PropertyName.Of("third", ("att", "key"))] = TimeSpan.FromSeconds(34442)
            };
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(114, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] {
                    91, 1, 65, 6, 0, 97, 0, 98, 0, 99, 0, 6, 0, 120, 0, 121, 0, 122, 0, 131, 0, 71, 10, 0, 102, 0, 105,
                    0, 114, 0, 115, 0, 116, 0, 71, 12, 0, 115, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 71, 12, 0, 115,
                    0, 101, 0, 99, 0, 111, 0, 110, 0, 100, 0, 196, 1, 129, 0, 2, 0, 200, 1, 87, 1, 65, 6, 0, 97, 0, 116,
                    0, 116, 0, 6, 0, 107, 0, 101, 0, 121, 0, 10, 0, 116, 0, 104, 0, 105, 0, 114, 0, 100, 0, 69, 8, 0, 0,
                    228, 44, 39, 83, 31, 0, 0
                },
                context.Buffer.StreamData);

            value["self"] = value;
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(129, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] {
                    91, 1, 65, 6, 0, 97, 0, 98, 0, 99, 0, 6, 0, 120, 0, 121, 0, 122, 0, 132, 0, 71, 10, 0, 102, 0, 105, 0,
                    114, 0, 115, 0, 116, 0, 71, 12, 0, 115, 0, 116, 0, 114, 0, 105, 0, 110, 0, 103, 0, 71, 12, 0, 115, 0, 101,
                    0, 99, 0, 111, 0, 110, 0, 100, 0, 196, 1, 129, 0, 2, 0, 200, 1, 87, 1, 65, 6, 0, 97, 0, 116, 0, 116, 0, 6,
                    0, 107, 0, 101, 0, 121, 0, 10, 0, 116, 0, 104, 0, 105, 0, 114, 0, 100, 0, 69, 8, 0, 0, 228, 44, 39, 83, 31,
                    0, 0, 71, 8, 0, 115, 0, 101, 0, 108, 0, 102, 0, 15, 1, 0, 0
                },
                context.Buffer.StreamData);
        }
    }
}
