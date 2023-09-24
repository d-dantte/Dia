using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary;
using Axis.Dia.Convert.Binary.Metadata;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Common.Utils;

namespace Axis.Dia.Tests.Convert.Binary.Serializers
{
    [TestClass]
    public class RecordPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = RecordValue.Null();
            var payload = RecordPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Record, payload.TypeMetadata.Type);


            var emptyValue = RecordValue.Empty();
            payload = RecordPayloadSerializer.CreatePayload(emptyValue);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Record, payload.TypeMetadata.Type);


            var value = RecordValue.Of(
                ArrayUtil.Of<Annotation>("a", "b"),
                KeyValuePair.Create(SymbolValue.Of("first"), (IDiaValue)BoolValue.Of(true)),
                KeyValuePair.Create(SymbolValue.Of("second"), (IDiaValue)IntValue.Null()),
                KeyValuePair.Create(SymbolValue.Of("third"), (IDiaValue)InstantValue.Of(DateTimeOffset.Now, "xyz", "abc")),
                KeyValuePair.Create(SymbolValue.Of("fourth"), (IDiaValue)RecordValue.Empty()));
            payload = RecordPayloadSerializer.CreatePayload(value);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(DiaType.Record, payload.TypeMetadata.Type);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = RecordValue.Null();
            var result = RecordPayloadSerializer.Serialize(nullValue, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(43, data[0]);

            var value = RecordValue.Empty();
            result = RecordPayloadSerializer.Serialize(value, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(11, data[0]); // tmeta

            value = RecordValue.Of(
                ArrayUtil.Of<Annotation>("a", "b"),
                KeyValuePair.Create(SymbolValue.Of("first"), (IDiaValue)BoolValue.Of(true)),
                KeyValuePair.Create(SymbolValue.Of("second"), (IDiaValue)IntValue.Null()),
                KeyValuePair.Create(SymbolValue.Of("third"), (IDiaValue)InstantValue.Of(DateTimeOffset.Parse("2023/07/05 09:52:37"), "xyz", "abc")),
                KeyValuePair.Create(SymbolValue.Of("fourth"), (IDiaValue)RecordValue.Empty()));

            result = RecordPayloadSerializer.Serialize(value, new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(68, data.Length);
            Assert.AreEqual(155, data[0]); // tmeta
        }

        [TestMethod]
        public void Desrialize_Tests()
        {
            var nullValue = RecordValue.Null();
            var bytes = RecordPayloadSerializer
                .Serialize(nullValue, new SerializerContext())
                .Resolve();
            var result = RecordPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            RecordValue value = RecordValue.Empty();
            bytes = RecordPayloadSerializer
                .Serialize(value, new SerializerContext())
                .Resolve();
            result = RecordPayloadSerializer.Deserialize(
                new MemoryStream(),
                TypeMetadata.Of(bytes[0]),
                new DeserializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(value, resultValue);


            value = RecordValue.Of(
                ArrayUtil.Of<Annotation>("a", "b"),
                KeyValuePair.Create(SymbolValue.Of("first"), (IDiaValue)BoolValue.Of(true)),
                KeyValuePair.Create(SymbolValue.Of("second"), (IDiaValue)IntValue.Null()),
                KeyValuePair.Create(SymbolValue.Of("third"), (IDiaValue)InstantValue.Of(DateTimeOffset.Parse("2023/07/05 09:52:37"), "xyz", "abc")),
                KeyValuePair.Create(SymbolValue.Of("fourth"), (IDiaValue)RecordValue.Empty()));
            bytes = RecordPayloadSerializer
                .Serialize(value, new SerializerContext())
                .Resolve();
            var cmetaCount = CmetaCount(value.Count);
            result = RecordPayloadSerializer.Deserialize(
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
