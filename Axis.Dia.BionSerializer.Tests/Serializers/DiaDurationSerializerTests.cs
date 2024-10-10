using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;

namespace Axis.Dia.Bion.Tests.Serializers
{
    [TestClass]
    public class DiaDurationSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaDurationSerializer.DefaultInstance;

            var duration = Core.Types.Duration.Null();
            var tmeta = serializer.ExtractMetadata(duration);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Duration, tmeta.Type);

            duration = Core.Types.Duration.Null("att");
            tmeta = serializer.ExtractMetadata(duration);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Duration, tmeta.Type);

            duration = Core.Types.Duration.Of(0);
            tmeta = serializer.ExtractMetadata(duration);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Duration, tmeta.Type);

            duration = Core.Types.Duration.Of(1234567890);
            tmeta = serializer.ExtractMetadata(duration);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Duration, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaDurationSerializer.DefaultInstance;

            var value = Core.Types.Duration.Of(0);
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 5 },
                context.Buffer.StreamData);

            value = Core.Types.Duration.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 37 },
                context.Buffer.StreamData);

            value = Core.Types.Duration.Of(2554000000);
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(11, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 69, 8, 0, 128, 242, 58, 152, 0, 0, 0, 0 },
                context.Buffer.StreamData);

            value = Core.Types.Duration.Of(1239876000000, "bleh");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(23, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 85, 1, 1, 8, 0, 98, 0, 108, 0, 101, 0, 104, 0, 8, 0, 0, 89, 94, 174, 32, 1, 0, 0 },
                context.Buffer.StreamData);
        }
    }
}
