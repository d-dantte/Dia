using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.BionSerializer.Serializers.Contracts;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class DiaAttributeSerializerTests
    {
        [TestMethod]
        public void SerializeMetadata_Tests()
        {
            var serializer = DiaAttributeSerializer.DefaultInstance;

            Core.Types.Attribute attribute = ("abcd", "efgh");
            var tmeta = serializer.ExtractMetadata(attribute);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Attribute, tmeta.Type);

            attribute = "abcd";
            tmeta = serializer.ExtractMetadata(attribute);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var serializer = DiaAttributeSerializer.DefaultInstance;
            var context = new SerializerContext();

            Core.Types.Attribute attribute = ("abcd", "efgh");
            serializer.SerializeType(attribute, context);
            Assert.AreEqual(21, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 65, 8, 0, 97, 0, 98, 0, 99, 0, 100, 0, 8, 0, 101, 0, 102, 0, 103, 0, 104, 0 },
                context.Buffer.StreamData);

            attribute = "abcd";
            context = new SerializerContext();
            serializer.SerializeType(attribute, context);
            Assert.AreEqual(11, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 1, 8, 0, 97, 0, 98, 0, 99, 0, 100, 0 },
                context.Buffer.StreamData);

        }
    }
}
