using Axis.Dia.BionSerializer.Serializers;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class DiaSymbolSerializerTests
    {
        [TestMethod]
        public void ExtractMetadata_Tests()
        {
            var serializer = DiaSymbolSerializer.DefaultInstance;

            var symbol = Core.Types.Symbol.Null();
            var tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Symbol, tmeta.Type);

            symbol = Core.Types.Symbol.Null("att");
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsTrue(tmeta.IsNull);
            Assert.IsTrue(tmeta.IsAnnotated);
            Assert.IsFalse(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(Core.DiaType.Symbol, tmeta.Type);

            symbol = Core.Types.Symbol.Of("");
            tmeta = serializer.ExtractMetadata(symbol);
            Assert.IsFalse(tmeta.IsNull);
            Assert.IsFalse(tmeta.IsAnnotated);
            Assert.IsTrue(tmeta.IsCustomFlagSet);
            Assert.IsFalse(tmeta.IsOverflowFlagSet);
            Assert.AreEqual(0, tmeta.CustomMetadata.Length);
            Assert.AreEqual(Core.DiaType.Symbol, tmeta.Type);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {
            var serializer = DiaSymbolSerializer.DefaultInstance;

            var value = Core.Types.Symbol.Of("");
            var context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 72 },
                context.Buffer.StreamData);

            value = Core.Types.Symbol.Null();
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(1, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 40 },
                context.Buffer.StreamData);

            value = Core.Types.Symbol.Of("-2554");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(13, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 8, 10, 0, 45, 0, 50, 0, 53, 0, 53, 0, 52, 0 },
                context.Buffer.StreamData);

            value = Core.Types.Symbol.Of("1234567", "att");
            context = new SerializerContext();
            serializer.SerializeType(value, context);
            Assert.AreEqual(27, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 24, 1, 1, 6, 0, 97, 0, 116, 0, 116, 0, 14, 0, 49, 0, 50, 0, 51, 0, 52, 0, 53, 0, 54, 0, 55, 0 },
                context.Buffer.StreamData);
        }
    }
}
