using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Bion.Types;

namespace Axis.Dia.Bion.Tests.Serializers
{
    [TestClass]
    public class DiaReferenceSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaReferenceSerializer.DefaultInstance;

            var @ref = Reference.Of(34);
            var tmeta = serializer.ExtractMetadata(@ref);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaReferenceSerializer.DefaultInstance;
            var @ref = Reference.Of(34);
            var context = new SerializerContext();

            serializer.SerializeType(@ref, context);
            Assert.AreEqual(4, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 15, 1, 0, 34 },
                context.Buffer.StreamData);
        }
    }
}
