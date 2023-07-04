using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.IO.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Axis.Dia.Tests.IO.Binary.Serializers
{
    [TestClass]
    public class ClobPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = ClobValue.Null();
            var payload = ClobPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Clob, payload.TypeMetadata.Type);

            var text = RandomText();
            var value = ClobValue.Of(text);
            payload = ClobPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(CmetaCount(text.Length), payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Clob, payload.TypeMetadata.Type);

            text = RandomText();
            value = ClobValue.Of(text, "a", "b");
            payload = ClobPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(CmetaCount(text.Length), payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Clob, payload.TypeMetadata.Type);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = ClobValue.Null();
            var result = ClobPayloadSerializer.Serialize(nullValue, new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(40, data[0]);


            var text = "";
            var value = ClobValue.Of(text);
            result = ClobPayloadSerializer.Serialize(value, new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(8, data[0]); // tmeta


            value = ClobValue.Of(text, "annotation1", "annotation2");
            result = ClobPayloadSerializer.Serialize(value, new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(48, data.Length);
            Assert.AreEqual(24, data[0]); // tmeta
            Assert.AreEqual(2, data[1]);   // annotation count


            text = RandomText();
            value = ClobValue.Of(text);
            result = ClobPayloadSerializer.Serialize(value, new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(CmetaCount(text.Length) + 1 + (text.Length * 2), data.Length);
            Assert.AreEqual(136, data[0]); // tmeta
            var charCount = VarBytes
                .Of(data[1..(CmetaCount(text.Length) + 1)], false)
                .ToByteArray()
                .ApplyTo(bytes => new BigInteger(bytes, true));
            Assert.AreEqual(text.Length, charCount);
        }

        [TestMethod]
        public void Deserialize_Test()
        {
            var nullValue = ClobValue.Null();
            var bytes = ClobPayloadSerializer
                .Serialize(nullValue, new Dia.IO.Binary.BinarySerializerContext())
                .Resolve();
            var result = ClobPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            ClobValue value = "";
            bytes = ClobPayloadSerializer
                .Serialize(value, new Dia.IO.Binary.BinarySerializerContext())
                .Resolve();
            result = ClobPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            var text = RandomText();
            value = ClobValue.Of(text, "annotation1", "annotation2");
            bytes = ClobPayloadSerializer
                .Serialize(value, new Dia.IO.Binary.BinarySerializerContext())
                .Resolve();
            result = ClobPayloadSerializer.Deserialize(
                new MemoryStream(bytes[(CmetaCount(text.Length) + 1)..]),
                TypeMetadata.Of(bytes[0], ToCmeta(bytes, CmetaCount(text.Length))),
                new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);
        }

        private static int CmetaCount(int charCount)
        {
            return charCount switch
            {
                < 127 => 1,
                >= 127 => 2
            };
        }

        private static string RandomText()
        {
            var random = new Random(DateTimeOffset.Now.Millisecond);
            var bytes = new byte[random.Next(25, 500) * 2];
            random.NextBytes(bytes);
            return bytes
                .Batch(2)
                .Select(charBytes => BitConverter.ToChar(charBytes.ToArray()))
                .ApplyTo(chars => new string(chars.ToArray()));
        }

        private static CustomMetadata[] ToCmeta(byte[] bytes, int cmetaCount)
        {
            var l = new List<CustomMetadata>();
            for (int cnt = 1; cnt <= cmetaCount; cnt++)
            {
                l.Add(CustomMetadata.Of(bytes[cnt]));
            }

            return l.ToArray();
        }
    }
}
