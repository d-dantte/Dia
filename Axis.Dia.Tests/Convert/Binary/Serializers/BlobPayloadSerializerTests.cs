using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Tests.Convert.Binary.Serializers
{
    [TestClass]
    public class BlobPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = BlobValue.Null();
            var payload = BlobPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Blob, payload.TypeMetadata.Type);

            var bytes = RandomBytes();
            var value = BlobValue.Of(bytes);
            payload = BlobPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(CmetaCount(bytes.Length), payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Blob, payload.TypeMetadata.Type);

            bytes = RandomBytes();
            value = BlobValue.Of(bytes, "a", "b");
            payload = BlobPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(CmetaCount(bytes.Length), payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Blob, payload.TypeMetadata.Type);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = BlobValue.Null();
            var result = BlobPayloadSerializer.Serialize(nullValue, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(41, data[0]);


            var bytes = new byte[0];
            var value = BlobValue.Of(bytes);
            result = BlobPayloadSerializer.Serialize(value, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(9, data[0]); // tmeta


            value = BlobValue.Of(bytes, "annotation1", "annotation2");
            result = BlobPayloadSerializer.Serialize(value, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(48, data.Length);
            Assert.AreEqual(25, data[0]); // tmeta
            Assert.AreEqual(2, data[1]);   // annotation count


            bytes = RandomBytes();
            value = BlobValue.Of(bytes);
            result = BlobPayloadSerializer.Serialize(value, new Dia.Convert.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(CmetaCount(bytes.Length) + 1 + bytes.Length, data.Length);
            Assert.AreEqual(137, data[0]); // tmeta
            var byteCount = VarBytes
                .Of(data[1..(CmetaCount(bytes.Length) + 1)], false)
                .ToByteArray()
                .ApplyTo(bytes => new BigInteger(bytes, true));
            Assert.AreEqual(bytes.Length, byteCount);
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var nullValue = BlobValue.Null();
            var bytes = BlobPayloadSerializer
                .Serialize(nullValue, new BinarySerializerContext())
                .Resolve();
            var result = BlobPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            var initialBytes = new byte[0];
            BlobValue value = initialBytes;
            bytes = BlobPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = BlobPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            initialBytes = RandomBytes();
            var cmetaCount = CmetaCount(initialBytes.Length);
            value = initialBytes;
            bytes = BlobPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = BlobPayloadSerializer.Deserialize(
                new MemoryStream(bytes[(cmetaCount + 1)..]),
                TypeMetadata.Of(bytes[0], ToCmeta(bytes, cmetaCount)),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);
        }

        private static int CmetaCount(int byteCount)
        {
            return byteCount switch
            {
                < 127 => 1,
                >= 127 => 2
            };
        }

        private static byte[] RandomBytes()
        {
            var random = new Random(DateTimeOffset.Now.Millisecond);
            var bytes = new byte[random.Next(50, 1000)];
            random.NextBytes(bytes);
            return bytes;
        }

        private static CustomMetadata[] ToCmeta(byte[] bytes, int cmetaCount)
        {
            var l = new List<CustomMetadata>();
            for(int cnt=1; cnt <= cmetaCount; cnt++)
            {
                l.Add(CustomMetadata.Of(bytes[cnt]));
            }

            return l.ToArray();
        }
    }
}
