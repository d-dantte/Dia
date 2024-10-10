using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Core;

namespace Axis.Dia.Bion.Tests.Serializers
{
    [TestClass]
    public class DiaSequenceSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaSequenceSerializer.DefaultInstance;

            var symbol = Core.Types.Sequence.Null();
            var tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Sequence, tmeta.Type);

            symbol = Core.Types.Sequence.Null("att");
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Sequence, tmeta.Type);

            symbol = Core.Types.Sequence.Of("string", 34);
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Sequence, tmeta.Type);

            symbol = Core.Types.Sequence.Of(Enumerable.Empty<DiaValue>(), []);
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Sequence, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaSequenceSerializer.DefaultInstance;

            var value = Core.Types.Sequence.Of();
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 10 },
                context.Buffer.StreamData);

            value = Core.Types.Sequence.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 42 },
                context.Buffer.StreamData);

            value = Core.Types.Sequence.Of(2, false);
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(8, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 74, 130, 0, 67, 1, 0, 2, 2 },
                context.Buffer.StreamData);

            value = Core.Types.Sequence.Of([("key", "value")], "1234567", TimeSpan.FromSeconds(343454));
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(53, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] {
                    90, 1, 65, 6, 0, 107, 0, 101, 0, 121, 0, 10, 0, 118, 0, 97, 0, 108, 0, 117, 0, 101, 0, 130, 0, 71,
                    14, 0, 49, 0, 50, 0, 51, 0, 52, 0, 53, 0, 54, 0, 55, 0, 69, 8, 0, 0, 172, 170, 157, 94, 56, 1, 0
                },
                context.Buffer.StreamData);

            value.Add(value);
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(57, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] {
                    90, 1, 65, 6, 0, 107, 0, 101, 0, 121, 0, 10, 0, 118, 0, 97, 0, 108, 0, 117, 0, 101, 0, 131, 0, 71, 14,
                    0, 49, 0, 50, 0, 51, 0, 52, 0, 53, 0, 54, 0, 55, 0, 69, 8, 0, 0, 172, 170, 157, 94, 56, 1, 0, 15, 1, 0, 0
                },
                context.Buffer.StreamData);
        }
    }
}
