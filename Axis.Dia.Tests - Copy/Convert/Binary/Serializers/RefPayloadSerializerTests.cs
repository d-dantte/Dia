using Axis.Dia.Convert.Binary;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Binary.Serializers
{
    [TestClass]
    public class RefPayloadSerializerTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            IntValue value = IntValue.Of(43);
            ReferenceValue linkedRef = ReferenceValue.Of(value);
            ReferenceValue unlinkedRef = ReferenceValue.Of(Guid.NewGuid());

            var context = new SerializerContext();

            var result = RefPayloadSerializer.Serialize(linkedRef, context);
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(2, data.Length);
            Assert.AreEqual(140, data[0]);
            Assert.AreEqual(1, data[1]);


            result = RefPayloadSerializer.Serialize(unlinkedRef, context);
            Assert.IsTrue(result.IsErrorResult());
        }

        [TestMethod]
        public void Deserialize_Tests()
        {

            var context = new SerializerContext();
            var dcontext = new DeserializerContext();
            var tmeta = TypeMetadata.Of(Contracts.DiaType.Int, TypeMetadata.MetadataFlags.None);

            Assert.ThrowsException<ArgumentNullException>(() => RefPayloadSerializer.Deserialize(null, default, dcontext));
            Assert.ThrowsException<ArgumentNullException>(() => RefPayloadSerializer.Deserialize(
                new MemoryStream(Array.Empty<byte>()), default, null));
            Assert.ThrowsException<ArgumentException>(() => RefPayloadSerializer.Deserialize(
                new MemoryStream(Array.Empty<byte>()), tmeta, dcontext));

            var address = dcontext.AllocateAddress();
            IntValue value = IntValue.Of(43).RelocateValue(address);
            ReferenceValue linkedRef = ReferenceValue.Of(value);

            var bytes = RefPayloadSerializer.Serialize(linkedRef, context).Resolve();
            var result = RefPayloadSerializer.Deserialize(
                new MemoryStream(Array.Empty<byte>()),
                TypeMetadata.Of(bytes),
                dcontext);
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
        }
    }
}
