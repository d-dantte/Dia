using Axis.Dia.IO.Binary.Serializers;
using Axis.Dia.Types;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Axis.Dia.Tests.IO.Binary.Serializers
{
    [TestClass]
    public class IntPayloadSerializerTests
    {
        [TestMethod]
        public void CreatePayload_Tests()
        {
            IntValue zeroValue = BigInteger.Zero;
            var payload = IntPayloadSerializer.CreatePayload(zeroValue);

            Assert.IsFalse(payload.TypeMetadata.IsNull);
            Assert.IsFalse(payload.TypeMetadata.IsAnnotated);
            Assert.IsFalse(payload.TypeMetadata.IsCustomFlagSet);
            Assert.IsFalse(payload.TypeMetadata.IsOverflowFlagSet);
            Assert.AreEqual(0, payload.TypeMetadata.CustomMetadataCount);
        }
    }
}
