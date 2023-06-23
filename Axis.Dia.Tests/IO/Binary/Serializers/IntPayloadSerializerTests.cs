using Axis.Dia.IO.Binary.Serializers;
using Axis.Dia.Types;
using System.Numerics;

namespace Axis.Dia.Tests.IO.Binary.Serializers
{
    [TestClass]
    public class IntPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = IntValue.Null();
            var payload = IntPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);


            IntValue zeroValue = BigInteger.Zero;
            payload = IntPayloadSerializer.CreatePayload(zeroValue);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);


            IntValue oneValue = IntValue.Of(BigInteger.One, "annotated");
            payload = IntPayloadSerializer.CreatePayload(oneValue);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            IntValue value = 2;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 7;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 12;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 45;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 111;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 256;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(2, payload.TypeMetadata.CustomMetadata[0].DataByteValue);

            var bigInt = BigInteger.Parse(
                "999999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999"
                + "9999999999999999999999999999999999999999999999999999999999999999999999999999999999");

            value = bigInt;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(2, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(66, payload.TypeMetadata.CustomMetadata[0].DataByteValue);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = IntValue.Null();
            var bytes = IntPayloadSerializer.Serialize(nullValue, new Dia.IO.Binary.BinarySerializerContext());
        }
    }
}
