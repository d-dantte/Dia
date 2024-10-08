using Axis.Dia.BionSerializer.Serializers;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class DiaBlobSerializerTests
    {
        [TestMethod]
        public void SerializeMetadata_Tests()
        {
            var serializer = DiaBlobSerializer.DefaultInstance;

            var nullBlob = Core.Types.Blob.Null();
            var blob = Core.Types.Blob.Of([1, 2, 3, 4, 5]);
            var emptyBlob = Core.Types.Blob.Of([]);
            var nullBlobWithAttributes = Core.Types.Blob.Null("att");

            var tmeta = serializer.ExtractMetadata(nullBlob);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Blob, tmeta.Type);

            tmeta = serializer.ExtractMetadata(nullBlobWithAttributes);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Blob, tmeta.Type);

            tmeta = serializer.ExtractMetadata(blob);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Blob, tmeta.Type);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);

            tmeta = serializer.ExtractMetadata(emptyBlob);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.IsTrue(tmeta.CustomMetadata.IsEmpty());
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var serializer = DiaBlobSerializer.DefaultInstance;

            var blob = Core.Types.Blob.Of([1, 2, 3, 4, 5]);
            var emptyBlob = Core.Types.Blob.Of([]);
            var nullBlob = Core.Types.Blob.Null();
            var blobWithAttributes = Core.Types.Blob.Of([1, 2, 3, 4, 5], "att");

            var context = new SerializerContext();
            serializer.SerializeType(blob, context);
            Assert.AreEqual(7, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 73, 5, 1, 2, 3, 4, 5 },
                context.Buffer.StreamData);

            context = new SerializerContext();
            serializer.SerializeType(emptyBlob, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 9 },
                context.Buffer.StreamData);

            context = new SerializerContext();
            serializer.SerializeType(nullBlob, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 41 },
                context.Buffer.StreamData);

            context = new SerializerContext();
            serializer.SerializeType(blobWithAttributes, context);
            Assert.AreEqual(17, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 89, 1, 1, 6, 0, 97, 0, 116, 0, 116, 0, 5, 1, 2, 3, 4, 5 },
                context.Buffer.StreamData);
        }

    }
}
