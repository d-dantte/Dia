using Axis.Dia.IO.Binary.Metadata;

namespace Axis.Dia.Tests.IO.Binary.Metadata
{
    [TestClass]
    public class CustomMetadataTests
    {
        [TestMethod]
        public void ByteValue_Tests()
        {
            CustomMetadata cm;
            for (int cnt = 0; cnt <= byte.MaxValue; cnt++)
            {
                cm = (byte)cnt;
                if (cnt < 128)
                    Assert.AreEqual(cnt, cm.DataByteValue);

                else Assert.AreEqual((cnt & 127), cm.DataByteValue);
            }

        }

        [TestMethod]
        public void FlagValue_Tests()
        {
            CustomMetadata cm;
            for (int cnt = 0; cnt <= byte.MaxValue; cnt++)
            {
                cm = (byte)cnt;
                Assert.AreEqual(cnt, cnt & (int)cm.FlagValue);
            }

        }

        [TestMethod]
        public void HasOverflow_Tests()
        {
            CustomMetadata cm;
            for (int cnt = 0; cnt <= byte.MaxValue; cnt++)
            {
                cm = (byte)cnt;

                if (cnt < 128)
                    Assert.IsFalse(cm.HasOverflow);

                else Assert.IsTrue(cm.HasOverflow);
            }

        }

        [TestMethod]
        public void IsSet_Tests()
        {
            CustomMetadata cm;
            for (int cnt = 0; cnt <= byte.MaxValue; cnt++)
            {
                cm = (byte)cnt;
                var flags = (CustomMetadata.MetadataFlags)cnt;
                Assert.IsTrue(cm.IsSet(flags));
            }

        }
    }
}
