using Axis.Dia.BionSerializer.Deserializers;
using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.BionSerializer.Serializers.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Tests.Deserializers
{
    [TestClass]
    public class DiaSequenceDeserializerTests
    {
        private DiaSequenceSerializer serializer = DiaSequenceSerializer.DefaultInstance;
        private DiaSequenceDeserializer deserializer = DiaSequenceDeserializer.DefaultInstance;
        private DeserializerContext.CompoundDeserializer tdeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

        [TestMethod]
        public void Deserialize_WhenValueIsNull()
        {
            // null
            var value = Core.Types.Sequence.Null();
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeInstance(scontext.Buffer.Stream, tmeta, dcontext);
            deserializer.DeserializeItems(scontext.Buffer.Stream, result, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNullAndHasAttributes()
        {
            // null with attributes
            var value = Core.Types.Sequence.Null("att");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeInstance(scontext.Buffer.Stream, tmeta, dcontext);
            deserializer.DeserializeItems(scontext.Buffer.Stream, result, dcontext);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Deserialize_WhenValueIsEmpty()
        {
            // empty
            var value = Core.Types.Sequence.Empty();
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Sequence>()));
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNonEmpty()
        {
            // he quick brown fox, etc, etc...
            var value = Core.Types.Sequence.Of(1, 2.0m, "3", DateTimeOffset.Now, TimeSpan.FromHours(5));
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Sequence>()));
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNonEmptyAndContainsSelf()
        {
            // he quick brown fox, etc, etc...
            var value = Sequence.Of(
                DateTimeOffset.Now,
                TimeSpan.FromHours(5),
                Sequence.Of(45.65543m));
            value[2].AsSequence().Add(value);

            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Sequence>()));
        }

        [TestMethod]
        public void Deserialize_WhenValueIsValidAndHasAttributes()
        {
            // non-empty with attributes
            var value = Core.Types.Sequence.Of("abc 123", "abc", "def", "ghi", "jkl", "mno");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeInstance(scontext.Buffer.Stream, tmeta, dcontext);
            deserializer.DeserializeItems(scontext.Buffer.Stream, result, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Sequence>()));
        }

        [TestMethod]
        public void Deserialize_WithInvalidParameters()
        {
            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeInstance(
                    null!,
                    TypeMetadata.Of(Core.DiaType.Decimal),
                    new DeserializerContext()));

            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeInstance(
                    new MemoryStream(),
                    TypeMetadata.Of(Core.DiaType.Decimal),
                    null!));

            // invalid type
            Assert.ThrowsException<InvalidOperationException>(
                () => deserializer.DeserializeInstance(
                    new MemoryStream(),
                    TypeMetadata.Of(Core.DiaType.Decimal),
                    new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeItems_WithInvalidParameters()
        {
            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeItems(
                    null!,
                    Sequence.Empty(),
                    new DeserializerContext()));

            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeItems(
                    new MemoryStream(),
                    Sequence.Empty(),
                    null!));

            // invalid type
            Assert.ThrowsException<ArgumentException>(
                () => deserializer.DeserializeItems(
                    new MemoryStream(),
                    Sequence.Default,
                    new DeserializerContext()));
        }
    }
}
