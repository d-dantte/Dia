using Axis.Dia.Bion.Deserializers;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;

namespace Axis.Dia.Bion.Tests.Deserializers
{
    [TestClass]
    public class DiaTimestampDeserializerTests
    {
        private DiaTimestampSerializer serializer = DiaTimestampSerializer.DefaultInstance;
        private DiaTimestampDeserializer deserializer = DiaTimestampDeserializer.DefaultInstance;
        private DeserializerContext.CompoundDeserializer tdeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

        [TestMethod]
        public void Deserialize_WhenValueIsNull()
        {
            // null
            var value = Core.Types.Timestamp.Null();
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
            var value = Core.Types.Timestamp.Null("att");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsDefault()
        {
            // empty
            var value = Core.Types.Timestamp.Of(DateTimeOffset.MinValue);
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNonDefault()
        {
            // DateTimeOffset.Now
            var value = Core.Types.Timestamp.Of(DateTimeOffset.Now);
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
            var value = Core.Types.Timestamp.Of(DateTimeOffset.Now, "abc", "def", "ghi", "jkl", "mno");
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


