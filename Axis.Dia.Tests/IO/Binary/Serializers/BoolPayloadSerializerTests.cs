using Axis.Dia.IO.Binary.Serializers;
using Axis.Dia.Types;

namespace Axis.Dia.Tests.IO.Binary.Serializers
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
    }
}
