using Axis.Dia.Bion.Deserializers;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Bion.Tests.Deserializers
{
    [TestClass]
    public class DiaRecordDeserializerTests
    {
        private DiaRecordSerializer serializer = DiaRecordSerializer.DefaultInstance;
        private DiaRecordDeserializer deserializer = DiaRecordDeserializer.DefaultInstance;
        private DeserializerContext.CompoundDeserializer tdeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

        [TestMethod]
        public void Deserialize_WhenValueIsNull()
        {
            // null
            var value = Core.Types.Record.Null();
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
            var value = Core.Types.Record.Null("att");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeInstance(scontext.Buffer.Stream, tmeta, dcontext);
            deserializer.DeserializeItems(scontext.Buffer.Stream, result, dcontext);

            Assert.IsTrue(value.ValueEquals(result));
        }

        [TestMethod]
        public void Deserialize_WhenValueIsEmpty()
        {
            // empty
            var value = Core.Types.Record.Empty();
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Record>()));
        }

        [TestMethod]
        public void Deserialize_WhenValueIsNonEmpty()
        {
            // the quick brown fox, etc, etc...
            Record value = Core.Types.Record.Of(
                ("first", 1),
                ("second", 2.0m),
                ("third", "3"),
                ("fourth", DateTimeOffset.Now),
                ("fifth", TimeSpan.FromHours(5)));

            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = tdeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Record>()));
        }

        [TestMethod]
        public void Deserialize_WhenValueIsValidAndHasAttributes()
        {
            // non-empty with attributes
            var value = Core.Types.Record.Of(
                [("first","abc 123")],
                "abc", "def", "ghi", "jkl", "mno");
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeInstance(scontext.Buffer.Stream, tmeta, dcontext);
            deserializer.DeserializeItems(scontext.Buffer.Stream, result, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Record>()));
        }

        [TestMethod]
        public void Deserialize_WhenRecordEncapsulatesSelf()
        {
            // non-empty with attributes
            var value = new Record
            {
                ["first"] = "abc 123",
            };
            value["self"] = value;
            var scontext = new SerializerContext();
            serializer.SerializeType(value, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var result = dcontext.ValueDeserializer.DeserializeValue(scontext.Buffer.Stream, dcontext);

            Assert.IsTrue(value.ValueEquals(result.As<Record>()));
        }

        [TestMethod]
        public void DeserializeInstance_WithInvalidParameters()
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
                    Record.Empty(),
                    new DeserializerContext()));

            // invalid type
            Assert.ThrowsException<ArgumentNullException>(
                () => deserializer.DeserializeItems(
                    new MemoryStream(),
                    Record.Empty(),
                    null!));

            // invalid type
            Assert.ThrowsException<ArgumentException>(
                () => deserializer.DeserializeItems(
                    new MemoryStream(),
                    Record.Default,
                    new DeserializerContext()));
        }
    }
}
