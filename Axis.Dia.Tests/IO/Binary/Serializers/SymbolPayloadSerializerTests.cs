using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.IO.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.IO.Binary.Serializers
{
    [TestClass]
    public class SymbolPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            var nullValue = SymbolValue.Null();
            var payload = SymbolPayloadSerializer.CreatePayload(nullValue);

            Assert.IsTrue(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);


            SymbolValue someValue = SymbolValue.Of("some-symbol", "annotated");
            payload = SymbolPayloadSerializer.CreatePayload(someValue);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsTrue(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsTrue(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(1, payload.TypeMetadata.CustomMetadataCount);
            Assert.AreEqual(11, payload.TypeMetadata.CustomMetadata[0].DataByteValue);
        }

        [TestMethod]
        public void Serialize_Tests()
        {
            var nullValue = SymbolValue.Null();
            var result = SymbolPayloadSerializer.Serialize(nullValue, new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(1, data.Length);
            Assert.AreEqual(39, data[0]);

            SymbolValue someValue = SymbolValue.Of("the-value", "the-annotation", "the-other-annotation");
            result = SymbolPayloadSerializer.Serialize(someValue, new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(82, data.Length);
            Assert.AreEqual(151, data[0]); // type metadata byte
            Assert.AreEqual(9, data[1]);   // symbol char count (varbyte)
            Assert.AreEqual(2, data[2]);   // annotation count (varbyte)
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var nullValue = SymbolValue.Null();
            var bytes = SymbolPayloadSerializer
                .Serialize(nullValue, new Dia.IO.Binary.BinarySerializerContext())
                .Resolve();
            var result = SymbolPayloadSerializer.Deserialize(
                new MemoryStream(bytes[1..]),
                TypeMetadata.Of(bytes[0]),
                new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            var resultValue = result.Resolve();
            Assert.AreEqual(nullValue, resultValue);


            var emptyValue = SymbolValue.Of("");
            bytes = SymbolPayloadSerializer
                .Serialize(emptyValue, new Dia.IO.Binary.BinarySerializerContext())
                .Resolve();
            result = SymbolPayloadSerializer.Deserialize(
                new MemoryStream(bytes[1..]),
                TypeMetadata.Of(bytes[0]),
                new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(emptyValue, resultValue);


            var someValue = SymbolValue.Of("the-value", "the-annotation", "the-other-annotation");
            bytes = SymbolPayloadSerializer
                .Serialize(someValue, new Dia.IO.Binary.BinarySerializerContext())
                .Resolve();
            result = SymbolPayloadSerializer.Deserialize(
                new MemoryStream(bytes[2..]),
                TypeMetadata.Of(bytes[0], CustomMetadata.Of(bytes[1])),
                new Dia.IO.Binary.BinarySerializerContext());
            Assert.IsTrue(result.IsDataResult());
            resultValue = result.Resolve();
            Assert.AreEqual(someValue, resultValue);

        }
    }
}
