using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.BionSerializer.Serializers.Contracts;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class DiaBooleanSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaBooleanSerializer.DefaultInstance;

            var nullBoolean = Core.Types.Boolean.Null();
            var @true = Core.Types.Boolean.Of(true);
            var @false = Core.Types.Boolean.Of(false);
            var nullBooleanWithAttributes = Core.Types.Boolean.Null("att");

            var tmeta = serializer.ExtractMetadata(nullBoolean);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Bool, tmeta.Type);

            tmeta = serializer.ExtractMetadata(@true);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Bool, tmeta.Type);

            tmeta = serializer.ExtractMetadata(@false);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Bool, tmeta.Type);

            tmeta = serializer.ExtractMetadata(nullBooleanWithAttributes);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Bool, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaBooleanSerializer.DefaultInstance;

            var nullBoolean = Core.Types.Boolean.Null();
            var @true = Core.Types.Boolean.Of(true);
            var @false = Core.Types.Boolean.Of(false);
            var nullBooleanWithAttributes = Core.Types.Boolean.Null("att");

            var context = new SerializerContext();
            serializer.SerializeType(nullBoolean, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 34 },
                context.Buffer.StreamData);

            context = new SerializerContext();
            serializer.SerializeType(@true, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 66 },
                context.Buffer.StreamData);

            context = new SerializerContext();
            serializer.SerializeType(@false, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 2 },
                context.Buffer.StreamData);

            context = new SerializerContext();
            serializer.SerializeType(nullBooleanWithAttributes, context);
            Assert.AreEqual(11, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 50, 1, 1, 6, 0, 97, 0, 116, 0, 116, 0 },
                context.Buffer.StreamData);
        }
    }
}
