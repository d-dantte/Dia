using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using System.Numerics;

namespace Axis.Dia.Tests.Convert.Binary.Serializers
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
            Assert.AreEqual(3, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            IntValue value = 2;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(3, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 7;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(3, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 12;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(3, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 45;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(3, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 111;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(3, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = 256;
            payload = IntPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(5, payload.TypeMetadata.CustomMetadata[0].DataByteValue);

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
            Assert.AreEqual(5, payload.TypeMetadata.CustomMetadata[0].DataByteValue);
            Assert.AreEqual(25, payload.TypeMetadata.CustomMetadata[1].DataByteValue);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = IntValue.Null();
            var result = IntPayloadSerializer.Serialize(nullValue, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(35, data[0]);


            IntValue zeroValue = 0;
            result = IntPayloadSerializer.Serialize(zeroValue, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(3, data[0]);


            IntValue oneValue = 1;
            result = IntPayloadSerializer.Serialize(oneValue, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(3, data.Length);
            Assert.AreEqual(131, data[0]);
            Assert.AreEqual(3, data[1]);
            Assert.AreEqual(1, data[2]);


            oneValue = IntValue.Of(1, "annotation1", "annotation2");
            result = IntPayloadSerializer.Serialize(oneValue, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(50, data.Length);
            Assert.AreEqual(147, data[0]); // tmeta
            Assert.AreEqual(3, data[1]);   // int byte count
            Assert.AreEqual(2, data[2]);   // annotation count


            IntValue otherInt = -1222;
            result = IntPayloadSerializer.Serialize(otherInt, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(4, data.Length);
            Assert.AreEqual(131, data[0]);
            Assert.AreEqual(4, data[1]);
            Assert.AreEqual(58, data[2]);
            Assert.AreEqual(251, data[3]);


            otherInt = 65536;
            result = IntPayloadSerializer.Serialize(otherInt, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(5, data.Length);
            Assert.AreEqual(131, data[0]);
            Assert.AreEqual(7, data[1]);
            Assert.AreEqual(0, data[2]);
            Assert.AreEqual(0, data[3]);
            Assert.AreEqual(1, data[4]);

            otherInt = BigInteger.Parse("18446744073709551616");
            result = IntPayloadSerializer.Serialize(otherInt, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(11, data.Length);
            Assert.AreEqual(131, data[0]);
            Assert.AreEqual(19, data[1]);


            otherInt = BigInteger.Parse(
                "18446744073709551616000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            result = IntPayloadSerializer.Serialize(otherInt, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(95, data.Length);
            Assert.AreEqual(131, data[0]);
            Assert.AreEqual(185, data[1]);


            otherInt = BigInteger.Parse(
                "18446744073709551616000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            result = IntPayloadSerializer.Serialize(otherInt, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1363, data.Length);
            Assert.AreEqual(131, data[0]); // tmeta
            Assert.AreEqual(161, data[1]); // cmeta[0]
            Assert.AreEqual(21, data[2]);  // cmeta[1]
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var nullValue = IntValue.Null();
            var bytes = IntPayloadSerializer
                .Serialize(nullValue, new Dia.Convert.Binary.BinarySerializerContext())
                .Resolve();
            var result = IntPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            IntValue value = 0;
            bytes = IntPayloadSerializer
                .Serialize(value, new Dia.Convert.Binary.BinarySerializerContext())
                .Resolve();
            result = IntPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = 1;
            bytes = IntPayloadSerializer
                .Serialize(value, new Dia.Convert.Binary.BinarySerializerContext())
                .Resolve();
            result = IntPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = -1222;
            bytes = IntPayloadSerializer
                .Serialize(value, new Dia.Convert.Binary.BinarySerializerContext())
                .Resolve();
            result = IntPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = IntValue.Of(65536, "annotation of the gods");
            bytes = IntPayloadSerializer
                .Serialize(value, new Dia.Convert.Binary.BinarySerializerContext())
                .Resolve();
            result = IntPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = BigInteger.Parse(
                "18446744073709551616000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
                + "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            bytes = IntPayloadSerializer
                .Serialize(value, new Dia.Convert.Binary.BinarySerializerContext())
                .Resolve();
            result = IntPayloadSerializer.Deserialize(
                new MemoryStream(bytes[3..]),
                TypeMetadata.Of(bytes[0], bytes[1], bytes[2]),
                new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);
        }
    }
}
