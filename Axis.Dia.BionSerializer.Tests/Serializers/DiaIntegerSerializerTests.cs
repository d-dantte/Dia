using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.BionSerializer.Serializers.Contracts;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class DiaIntegerSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaIntegerSerializer.DefaultInstance;

            var integer = Core.Types.Integer.Null();
            var tmeta = serializer.ExtractMetadata(integer);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Int, tmeta.Type);

            integer = Core.Types.Integer.Null("att");
            tmeta = serializer.ExtractMetadata(integer);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Int, tmeta.Type);

            integer = Core.Types.Integer.Of(0);
            tmeta = serializer.ExtractMetadata(integer);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Int, tmeta.Type);

            integer = Core.Types.Integer.Of(-243);
            tmeta = serializer.ExtractMetadata(integer);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Int, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaIntegerSerializer.DefaultInstance;

            var value = Core.Types.Integer.Of(0);
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 3 },
                context.Buffer.StreamData);

            value = Core.Types.Integer.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 35 },
                context.Buffer.StreamData);

            value = Core.Types.Integer.Of(-2554);
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(5, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 67, 2, 0, 6, 246 },
                context.Buffer.StreamData);

            value = Core.Types.Integer.Of(1234567, "att");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(16, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 83, 1, 1, 6, 0, 97, 0, 116, 0, 116, 0, 3, 0, 135, 214, 18 },
                context.Buffer.StreamData);
        }
    }
}
