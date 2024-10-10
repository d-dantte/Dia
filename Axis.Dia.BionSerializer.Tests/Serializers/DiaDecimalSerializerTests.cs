using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;

namespace Axis.Dia.Bion.Tests.Serializers
{
    [TestClass]
    public class DiaDecimalSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaDecimalSerializer.DefaultInstance;

            var @decimal = Core.Types.Decimal.Null();
            var tmeta = serializer.ExtractMetadata(@decimal);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Decimal, tmeta.Type);

            @decimal = Core.Types.Decimal.Null("att");
            tmeta = serializer.ExtractMetadata(@decimal);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Decimal, tmeta.Type);

            @decimal = Core.Types.Decimal.Of(0);
            tmeta = serializer.ExtractMetadata(@decimal);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Decimal, tmeta.Type);

            @decimal = Core.Types.Decimal.Of(21m);
            tmeta = serializer.ExtractMetadata(@decimal);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsTrue(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(1, tmeta.CustomMetadata.Length);
            Assert.AreEqual(3, (byte)tmeta.CustomMetadata[0]);
            Assert.AreEqual(Core.DiaType.Decimal, tmeta.Type);

            @decimal = Core.Types.Decimal.Of(-21m);
            tmeta = serializer.ExtractMetadata(@decimal);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsTrue(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(1, tmeta.CustomMetadata.Length);
            Assert.AreEqual(2, (byte)tmeta.CustomMetadata[0]);
            Assert.AreEqual(Core.DiaType.Decimal, tmeta.Type);

            @decimal = Core.Types.Decimal.Of(0.21m);
            tmeta = serializer.ExtractMetadata(@decimal);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsTrue(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(1, tmeta.CustomMetadata.Length);
            Assert.AreEqual(1, (byte)tmeta.CustomMetadata[0]);
            Assert.AreEqual(Core.DiaType.Decimal, tmeta.Type);

            @decimal = Core.Types.Decimal.Of(-0.21m);
            tmeta = serializer.ExtractMetadata(@decimal);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsTrue(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(1, tmeta.CustomMetadata.Length);
            Assert.AreEqual(0, (byte)tmeta.CustomMetadata[0]);
            Assert.AreEqual(Core.DiaType.Decimal, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaDecimalSerializer.DefaultInstance;

            var value = Core.Types.Decimal.Of(0);
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 4 },
                context.Buffer.StreamData);

            value = Core.Types.Decimal.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 36 },
                context.Buffer.StreamData);

            value = Core.Types.Decimal.Of(0.0m);
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 4 },
                context.Buffer.StreamData);

            value = Core.Types.Decimal.Of(-2554);
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(8, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 196, 2, 128, 0, 2, 0, 250, 9 },
                context.Buffer.StreamData);

            value = Core.Types.Decimal.Of(-0.2554, "att");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(24, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 212, 0, 1, 1, 6, 0, 97, 0, 116, 0, 116, 0, 146, 0, 8, 0, 16, 128, 162, 99, 242, 92, 139, 3 },
                context.Buffer.StreamData);
        }
    }
}
