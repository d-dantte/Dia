using Axis.Dia.IO.Binary;
using Axis.Dia.IO.Binary.Metadata;

namespace Axis.Dia.Tests.IO.Binary.Metadata
{
    [TestClass]
    public class TypeMetadataTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new TypeMetadata(5, null));

            var x = new TypeMetadata(5, CustomMetadata.Of(65));
            x = new TypeMetadata(
                5,
                CustomMetadata.Of(65),
                CustomMetadata.Of(92));

            x = new TypeMetadata(VarBytes.Of(new byte[] { 1, 2, 3, 4 }));

            x = VarBytes.Of(new byte[] { 1, 2, 3, 4 });

            x = TypeMetadata.Of(VarBytes.Of(new byte[] { 1, 2, 3, 4 }));

            x = TypeMetadata.Of(
                5,
                CustomMetadata.Of(65),
                CustomMetadata.Of(92));
        }
    }
}
