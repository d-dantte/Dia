using Axis.Dia.BionSerializer.Deserializers;
using Axis.Dia.BionSerializer.Serializers;

namespace Axis.Dia.BionSerializer.Tests.Deserializers
{
    [TestClass]
    public class DiaAttributeDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            var serializer = DiaAttributeSerializer.DefaultInstance;
            var deserializer = DiaAttributeDeserializer.DefaultInstance;
            var tdeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

            Core.Types.Attribute attribute = ("abcd", "efgh");
            var scontext = new SerializerContext();
            serializer.SerializeType(attribute, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var dcontext = new DeserializerContext();
            var tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(attribute, result);


            attribute = "abcd";
            scontext = new SerializerContext();
            serializer.SerializeType(attribute, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            dcontext = new DeserializerContext();
            tmeta = tdeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(attribute, result);
        }
    }
}
