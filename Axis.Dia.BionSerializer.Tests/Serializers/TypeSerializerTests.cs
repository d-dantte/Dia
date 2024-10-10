using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.BionSerializer.Serializers.Contracts;

namespace Axis.Dia.BionSerializer.Tests.Serializers
{
    [TestClass]
    public class TypeSerializerTests
    {
        [TestMethod]
        public void SerializeAttributes_Tests()
        {
            var serializer = TypeSerializer.DefaultInstance;
            var context = new SerializerContext();

            Core.Types.AttributeSet attributes = new Core.Types.Attribute[] { ("abcd", "efgh"), "abcd" };
            serializer.SerializeAttributeSet(attributes, context);
            Assert.AreEqual(33, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(
                new byte[] { 2, 1, 8, 0, 97, 0, 98, 0, 99, 0, 100, 0, 65, 8, 0, 97, 0, 98, 0, 99, 0, 100, 0, 8, 0, 101, 0, 102, 0, 103, 0, 104, 0 },
                context.Buffer.StreamData);


            attributes = Array.Empty<Core.Types.Attribute>();
            context = new();
            serializer.SerializeAttributeSet(attributes, context);
            Assert.AreEqual(0, context.Buffer.Stream.Length);
            CollectionAssert.AreEqual(Array.Empty<byte>(), context.Buffer.StreamData);
        }

        [TestMethod]
        public void SerializeType_Tests()
        {

        }
    }
}
