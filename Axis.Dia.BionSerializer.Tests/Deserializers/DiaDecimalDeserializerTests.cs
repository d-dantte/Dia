using Axis.Dia.Bion.Deserializers;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;

namespace Axis.Dia.Bion.Tests.Deserializers
{
    [TestClass]
    public class DiaDecimalDeserializerTests
    {
        private DiaDecimalSerializer serializer = DiaDecimalSerializer.DefaultInstance;
        private DiaDecimalDeserializer deserializer = DiaDecimalDeserializer.DefaultInstance;
        private DeserializerContext.CompoundDeserializer tdeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

        [TestMethod]
        public void Deserialize_WhenValueIsNull()
        {
            // null
            var value = Core.Types.Decimal.Null();
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNullAndHasAttributes()
        {
            // null with attributes
            var value = Core.Types.Decimal.Null("att");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsValid()
        {
            // 0.0
            var value = Core.Types.Decimal.Of(0.0m);
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsInteger()
        {
            // 12,345,678,901,234L
            var value = Core.Types.Decimal.Of(12_345_678_901_234L);
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsDecimal()
        {
            // 0.12,345,678,901,234m
            var value = Core.Types.Decimal.Of(0.12_345_678_901_234m);
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsIntegerWithDecimal()
        {
            // 123,456.12,345,678,901,234m
            var value = Core.Types.Decimal.Of(123_456.12_345_678_901_234m);
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNegative()
        {
            // -123,456.12,345,678,901,234m
            var value = Core.Types.Decimal.Of(-123_456.12_345_678_901_234m);
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsValidAndHasAttributes()
        {
            // non-empty with attributes
            var value = Core.Types.Decimal.Of(123.001m, "abc", "def", "ghi", "jkl", "mno");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WithInvalidParameters()
        {
            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeType(
                    null!,
                    TypeMetadata.Of(Core.DiaType.Int),
                    new DeserializerContext()));

            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeType(
                    new MemoryStream(),
                    TypeMetadata.Of(Core.DiaType.Int),
                    null!));

            // invalid type
            Assert.ThrowsException<InvalidOperationException>(
                () => deserializer.DeserializeType(
                    new MemoryStream(),
                    TypeMetadata.Of(Core.DiaType.Int),
                    new DeserializerContext()));
        }
    }
}


