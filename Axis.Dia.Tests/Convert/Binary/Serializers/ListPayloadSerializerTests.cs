using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Axis.Dia.Tests.Convert.Binary.Serializers
{
    [TestClass]
    public class ListPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = ListValue.Null();
            var payload = ListPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.List, payload.TypeMetadata.Type);

            var value = ListValue.Empty();
            payload = ListPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.List, payload.TypeMetadata.Type);

            value = ListValue.Of(
                ArrayUtil.Of<Annotation>("a", "b"),
                BoolValue.Of(true),
                IntValue.Null(),
                InstantValue.Of(DateTimeOffset.Now, "xyz", "abc"),
                ListValue.Empty());
            payload = ListPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(CmetaCount(value.Count), payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.List, payload.TypeMetadata.Type);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = ListValue.Null();
            var result = ListPayloadSerializer.Serialize(nullValue, new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(42, data[0]);


            var value = ListValue.Empty();
            result = ListPayloadSerializer.Serialize(value, new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(10, data[0]); // tmeta

            value = ListValue.Of(
                ArrayUtil.Of<Annotation>("a", "b"),
                BoolValue.Of(true),
                IntValue.Null(),
                InstantValue.Of(DateTimeOffset.Parse("2023/07/05 09:52:37"), "xyz", "abc"),
                ListValue.Empty());
            result = ListPayloadSerializer.Serialize(value, new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(38, data.Length);
            Assert.AreEqual(154, data[0]); // tmeta
        }

        [TestMethod]
        public void Desrialize_Tests()
        {
            var nullValue = ListValue.Null();
            var bytes = ListPayloadSerializer
                .Serialize(nullValue, new BinarySerializerContext())
                .Resolve();
            var result = ListPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            ListValue value = ListValue.Empty();
            bytes = ListPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            result = ListPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = ListValue.Of(
                ArrayUtil.Of<Annotation>("a", "b"),
                BoolValue.Of(true),
                IntValue.Null(),
                InstantValue.Of(DateTimeOffset.Parse("2023/07/05 09:52:37"), "xyz", "abc"),
                ListValue.Empty());
            bytes = ListPayloadSerializer
                .Serialize(value, new BinarySerializerContext())
                .Resolve();
            var cmetaCount = CmetaCount(value.Count);
            result = ListPayloadSerializer.Deserialize(
                new MemoryStream(bytes[(cmetaCount + 1)..]),
                TypeMetadata.Of(bytes[0], ToCmeta(bytes, cmetaCount)),
                new BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);
        }

        private static int CmetaCount(int charCount)
        {
            var bitCount = System.Convert.ToString(charCount, 2).Length;
            return Math.DivRem(bitCount, 7, out var rem) + (rem > 0 ? 1 : 0);
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
