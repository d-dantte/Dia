using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.BionSerializer.Serializers.Contracts;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class DiaTimestampSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaTimestampSerializer.DefaultInstance;

            var symbol = Core.Types.Timestamp.Null();
            var tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Timestamp, tmeta.Type);

            symbol = Core.Types.Timestamp.Null("att");
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Timestamp, tmeta.Type);

            symbol = Core.Types.Timestamp.Of(DateTimeOffset.MinValue);
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Timestamp, tmeta.Type);

            symbol = Core.Types.Timestamp.Of(DateTimeOffset.Now);
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Timestamp, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaTimestampSerializer.DefaultInstance;
            var now = new DateTimeOffset(2021, 10, 5, 1, 23, 15, TimeSpan.Zero);

            var value = Core.Types.Timestamp.Of(now);
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(15, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 70, 0, 0, 240, 154, 233, 87, 36, 20, 5, 229, 143, 128, 128, 0 },
                context.Buffer.StreamData);

            value = Core.Types.Timestamp.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 38 },
                context.Buffer.StreamData);

            value = Core.Types.Timestamp.Of(now, "att");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(25, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 86, 1, 1, 6, 0, 97, 0, 116, 0, 116, 0, 0, 0, 240, 154, 233, 87, 36, 20, 5, 229, 143, 128, 128, 0 },
                context.Buffer.StreamData);

            value = Core.Types.Timestamp.Of(DateTimeOffset.MinValue);
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 6 },
                context.Buffer.StreamData);
        }
    }
}
