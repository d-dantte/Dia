using Axis.Dia.BionSerializer.Deserializers;
using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.Core.Types;

namespace Axis.Dia.BionSerializer.Tests.Deserializers
{
    [TestClass]
    public class DiaBlobDeserializerTests
    {
        private DiaBlobSerializer serializer = DiaBlobSerializer.DefaultInstance;
        private DiaBlobDeserializer deserializer = DiaBlobDeserializer.DefaultInstance;
        private DeserializerContext.CompoundDeserializer tdeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

        [TestMethod]
        public void Deserialize_WhenValueIsNull()
        {
            // null
            var blob = Blob.Null();
            var scontext = new SerializerContext();
            serializer.SerializeType(blob, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(blob, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNullAndHasAttributes()
        {
            // null with attributes
            var blob = Blob.Null("att");
            var scontext = new SerializerContext();
            serializer.SerializeType(blob, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(blob, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsEmpty()
        {
            // empty
            var blob = Blob.Of();
            var scontext = new SerializerContext();
            serializer.SerializeType(blob, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(blob, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNonEmpty()
        {
            // non-empty
            var bytes = new byte[100];
            Random.Shared.NextBytes(bytes);
            var blob = Blob.Of(bytes);
            var scontext = new SerializerContext();
            serializer.SerializeType(blob, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(blob, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNonEmptyAndHasAttributes()
        {
            // non-empty with attributes
            var bytes = new byte[100000];
            Random.Shared.NextBytes(bytes);
            var blob = Blob.Of(bytes, "abc", "def", "ghi", "jkl", "mno");
            var scontext = new SerializerContext();
            serializer.SerializeType(blob, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(blob, result);
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
