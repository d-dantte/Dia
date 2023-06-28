using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.IO.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using System.Numerics;

namespace Axis.Dia.Tests.IO.Binary.Serializers
{
    [TestClass]
    public class InstantPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = InstantValue.Null();
            var payload = InstantPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Instant, payload.TypeMetadata.Type);


            InstantValue defaultValue = default(DateTimeOffset);
            payload = InstantPayloadSerializer.CreatePayload(defaultValue);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);


            InstantValue value = new DateTimeOffset(2012, 1, 1, 0, 0, 0, TimeSpan.Zero);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(4, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 1, 0, 0, 0, TimeSpan.Zero);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(4, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 0, 0, 0, TimeSpan.Zero);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(5, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 0, 0, TimeSpan.Zero);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(7, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 0, TimeSpan.Zero);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(7, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.Zero);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(7, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.Zero) + TimeSpan.FromTicks(1230000);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(7, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.Zero) + TimeSpan.FromTicks(1234560);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(7, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.Zero) + TimeSpan.FromTicks(1234567);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(7, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.FromHours(12)) + TimeSpan.FromTicks(1234567);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(31, payload.TypeMetadata.CustomMetadata[0].DataByteValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.FromHours(-12)) + TimeSpan.FromTicks(1234567);
            payload = InstantPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(11, payload.TypeMetadata.CustomMetadata[0].DataByteValue);
        }


        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = InstantValue.Null();
            var result = InstantPayloadSerializer.Serialize(nullValue, new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(37, data[0]);


            InstantValue defaultValue = default(DateTimeOffset);
            result = InstantPayloadSerializer.Serialize(defaultValue, new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(10, data.Length);
            Assert.AreEqual(133, data[0]);


            InstantValue value = new DateTimeOffset(2012, 1, 1, 0, 0, 0, TimeSpan.Zero);
            result = InstantPayloadSerializer.Serialize(value, new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(11, data.Length);
            Assert.AreEqual(133, data[0]);


            value =InstantValue.Of(new DateTimeOffset(2012, 1, 1, 0, 0, 0, TimeSpan.Zero), "annotation");
            result = InstantPayloadSerializer.Serialize(value, new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(33, data.Length);
            Assert.AreEqual(149, data[0]);
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var nullValue = InstantValue.Null();
            var bytes = InstantPayloadSerializer
                .Serialize(nullValue, new BinarySerializerContext())
                .Resolve();
            var result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            InstantValue value = default(DateTimeOffset);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = new DateTimeOffset(2012, 1, 1, 0, 0, 0, TimeSpan.Zero);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = new DateTimeOffset(2012, 12, 1, 0, 0, 0, TimeSpan.Zero);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = new DateTimeOffset(2012, 12, 31, 0, 0, 0, TimeSpan.Zero);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 0, 0, TimeSpan.Zero);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = InstantValue.Of(new DateTimeOffset(2012, 12, 31, 23, 59, 0, TimeSpan.Zero), "ann-1", "ann-2");
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.Zero);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.Zero) + TimeSpan.FromTicks(1234567);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = new DateTimeOffset(2012, 12, 31, 23, 59, 59, TimeSpan.FromHours(-12)) + TimeSpan.FromTicks(1234567);
            bytes = InstantPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = InstantPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);
        }

        [TestMethod]
        public void meh()
        {
            var bint = new BigInteger(2001);
            Console.WriteLine(bint.ToVarBytes(false));
            Console.WriteLine(bint.ToVarBytes(true));
            Console.WriteLine(BitSequence.Of(bint.ToVarBytes(true).ToByteArray()));
            Console.WriteLine(BitSequence.Of(bint.ToVarBytes(false).ToByteArray()));
        }
    }
}
