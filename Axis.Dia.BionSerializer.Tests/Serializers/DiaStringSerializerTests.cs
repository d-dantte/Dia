using Axis.Dia.BionSerializer.Serializers;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class DiaStringSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaStringSerializer.DefaultInstance;

            var @string = Core.Types.String.Null();
            var tmeta = serializer.ExtractMetadata(@string);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.String, tmeta.Type);

            @string = Core.Types.String.Null("att");
            tmeta = serializer.ExtractMetadata(@string);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.String, tmeta.Type);

            @string = Core.Types.String.Of("");
            tmeta = serializer.ExtractMetadata(@string);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.String, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaStringSerializer.DefaultInstance;

            var value = Core.Types.String.Of("");
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 71 },
                context.Buffer.StreamData);

            value = Core.Types.String.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 39 },
                context.Buffer.StreamData);

            value = Core.Types.String.Of("-2554");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(13, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 7, 10, 0, 45, 0, 50, 0, 53, 0, 53, 0, 52, 0 },
                context.Buffer.StreamData);

            value = Core.Types.String.Of("1234567", "att");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(27, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 23, 1, 1, 6, 0, 97, 0, 116, 0, 116, 0, 14, 0, 49, 0, 50, 0, 51, 0, 52, 0, 53, 0, 54, 0, 55, 0 },
                context.Buffer.StreamData);
        }
    }
}
