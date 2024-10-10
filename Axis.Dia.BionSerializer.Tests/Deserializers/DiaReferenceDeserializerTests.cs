using Axis.Dia.Bion.Deserializers;
using Axis.Dia.Bion.Serializers;
using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Bion.Types;

namespace Axis.Dia.Bion.Tests.Deserializers
{
    [TestClass]
    public class DiaReferenceDeserializerTests
    {
        [TestMethod]
        public void DeserializeType_Tests()
        {
            var serializer = DiaReferenceSerializer.DefaultInstance;
            var deserializer = DiaReferenceDeserializer.DefaultInstance;
            var typeDeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;

            // single byte
            var scontext = new SerializerContext();
            var dcontext = new DeserializerContext();

            var @ref = Reference.Of(123);
            serializer.SerializeType(@ref, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            var tmeta = typeDeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            var result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(@ref, result);

            //multiple bytes

            // single byte
            scontext = new SerializerContext();
            dcontext = new DeserializerContext();

            @ref = Reference.Of(123456);
            serializer.SerializeType(@ref, scontext);
            scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);

            tmeta = typeDeserializer.DeserializeTypeMetadata(scontext.Buffer.Stream, dcontext);
            result = deserializer.DeserializeType(scontext.Buffer.Stream, tmeta, dcontext);

            Assert.AreEqual(@ref, result);
        }
    }
}
