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
    public class StringPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = StringValue.Null();
            var payload = StringPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.String, payload.TypeMetadata.Type);

            var text = RandomText();
            var value = StringValue.Of(text);
            payload = StringPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(CmetaCount(text.Length), payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.String, payload.TypeMetadata.Type);

            text = RandomText();
            value = StringValue.Of(text, "a", "b");
            payload = StringPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(CmetaCount(text.Length), payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.String, payload.TypeMetadata.Type);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = StringValue.Null();
            var result = StringPayloadSerializer.Serialize(nullValue, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(38, data[0]);


            var text = "";
            var value = StringValue.Of(text);
            result = StringPayloadSerializer.Serialize(value, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(6, data[0]); // tmeta


            value = StringValue.Of(text, "annotation1", "annotation2");
            result = StringPayloadSerializer.Serialize(value, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(48, data.Length);
            Assert.AreEqual(22, data[0]); // tmeta
            Assert.AreEqual(2, data[1]);   // annotation count


            text = RandomText();
            value = StringValue.Of(text);
            result = StringPayloadSerializer.Serialize(value, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(CmetaCount(text.Length) + 1 + (text.Length * 2), data.Length);
            Assert.AreEqual(134, data[0]); // tmeta
            var charCount = VarBytes
                .Of(data[1..(CmetaCount(text.Length) + 1)], false)
                .ToByteArray()
                .ApplyTo(bytes => new BigInteger(bytes, true));
            Assert.AreEqual(text.Length, charCount);
        }

        [TestMethod]
        public void Desrialize_Tests()
        {
            var nullValue = StringValue.Null();
            var bytes = StringPayloadSerializer
                .Serialize(nullValue, new SerializerContext())
                .Resolve();
            var result = StringPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            StringValue value = "";
            bytes = StringPayloadSerializer
                .Serialize(value, new SerializerContext())
                .Resolve();
            result = StringPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            var text = AllChars();
            value = StringValue.Of(text, "annotation1", "annotation2");
            bytes = StringPayloadSerializer
                .Serialize(value, new SerializerContext())
                .Resolve();
            var cmetaCount = CmetaCount(text.Length);
            result = StringPayloadSerializer.Deserialize(
                new MemoryStream(bytes[(cmetaCount + 1)..]),
                TypeMetadata.Of(bytes[0], ToCmeta(bytes, cmetaCount)),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);
        }

        private static int CmetaCount(int charCount)
        {
            var bitCount = System.Convert.ToString(charCount, 2).Length;
            return Math.DivRem(bitCount, 7, out var rem) + (rem > 0 ? 1 : 0);
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

        private static string AllChars()
        {
            var chars = new char[ushort.MaxValue];
            for (ushort cnt = 0; cnt < ushort.MaxValue; cnt++)
            {
                chars[cnt] = (char)cnt;
            }

            return new string(chars);
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
