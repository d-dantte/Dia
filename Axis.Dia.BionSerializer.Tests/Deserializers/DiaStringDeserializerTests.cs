using Axis.Dia.BionSerializer.Deserializers;
using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.BionSerializer.Serializers.Contracts;

namespace Axis.Dia.BionSerializer.Tests.Deserializers
{
    [TestClass]
    public class DiaStringDeserializerTests
    {
        private DiaStringSerializer serializer = DiaStringSerializer.DefaultInstance;
        private DiaStringDeserializer deserializer = DiaStringDeserializer.DefaultInstance;
        private DeserializerContext.CompoundDeserializer tdeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

        [TestMethod]
        public void Deserialize_WhenValueIsNull()
        {
            // null
            var value = Core.Types.String.Null();
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
            var value = Core.Types.String.Null("att");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsEmpty()
        {
            // empty
            var value = Core.Types.String.Of(string.Empty);
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNonEmpty()
        {
            // non-empty
            var value = Core.Types.String.Of("the quick brown fox, etc, etc...");
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
            var value = Core.Types.String.Of("abc 123", "abc", "def", "ghi", "jkl", "mno");
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
                    TypeMetadata.Of(Core.DiaType.Decimal),
                    new DeserializerContext()));

            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeType(
                    new MemoryStream(),
                    TypeMetadata.Of(Core.DiaType.Decimal),
                    null!));

            // invalid type
            Assert.ThrowsException<InvalidOperationException>(
                () => deserializer.DeserializeType(
                    new MemoryStream(),
                    TypeMetadata.Of(Core.DiaType.Decimal),
                    new DeserializerContext()));
        }
    }
}


