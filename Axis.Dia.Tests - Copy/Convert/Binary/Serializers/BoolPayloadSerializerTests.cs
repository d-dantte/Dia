using Axis.Dia.Convert.Binary;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Binary.Serializers
{
    [TestClass]
    public class BoolPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            BoolValue falseValue = false;
            BoolValue trueValue = true;
            BoolValue nullValue = null;
            BoolValue annotatedValue = BoolValue.Of(true, "annotation1", "annotation2");

            var payload = BoolPayloadSerializer.CreatePayload(falseValue);
            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);

            payload = BoolPayloadSerializer.CreatePayload(trueValue);
            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsTrue(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);

            payload = BoolPayloadSerializer.CreatePayload(nullValue);
            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);

            payload = BoolPayloadSerializer.CreatePayload(annotatedValue);
            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsTrue(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = BoolValue.Null();
            var result = BoolPayloadSerializer.Serialize(nullValue, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(34, data[0]);


            BoolValue falseValue = false;
            result = BoolPayloadSerializer.Serialize(falseValue, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(2, data[0]);


            BoolValue trueValue = BoolValue.Of(true, "the-annotation", "the-other-annotation");
            result = BoolPayloadSerializer.Serialize(trueValue, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(72, data.Length);
            Assert.AreEqual(82, data[0]); // type metadata byte
            Assert.AreEqual(2, data[1]);   // annotation count (varbyte)
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var nullValue = BoolValue.Null();
            var bytes = BoolPayloadSerializer
                .Serialize(nullValue, new SerializerContext())
                .Resolve();
            var result = BoolPayloadSerializer.Deserialize(
                new MemoryStream(bytes[1..]),
                TypeMetadata.Of(bytes[0]),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            var falseValue = BoolValue.Of(false);
            bytes = BoolPayloadSerializer
                .Serialize(falseValue, new SerializerContext())
                .Resolve();
            result = BoolPayloadSerializer.Deserialize(
                new MemoryStream(bytes[1..]),
                TypeMetadata.Of(bytes[0]),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(falseValue, resultValue);


            var someValue = BoolValue.Of(true, "the-annotation", "the-other-annotation");
            bytes = BoolPayloadSerializer
                .Serialize(someValue, new SerializerContext())
                .Resolve();
            result = BoolPayloadSerializer.Deserialize(
                new MemoryStream(bytes[1..]),
                TypeMetadata.Of(bytes[0]),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(someValue, resultValue);

        }
    }
}
