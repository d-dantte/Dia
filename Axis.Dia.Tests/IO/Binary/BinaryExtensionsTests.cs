using Axis.Dia.IO.Binary;
using Axis.Luna.Common;
using Axis.Luna.Common.Utils;

namespace Axis.Dia.Tests.IO.Binary
{
    [TestClass]
    public class BinaryExtensionsTests
    {
        [TestMethod]
        public void AppendBits_Tests()
        {
            BitSequence seq = ArrayUtil.Of(true, true, false, false);
            var appended = seq.AppendBits(true, false, true);
            Assert.AreEqual(7, appended.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(
                ArrayUtil.Of(true, false, true),
                appended[^3..]));
        }

        [TestMethod]
        public void AppendBits_FromBytes_Tests()
        {
            BitSequence seq = ArrayUtil.Of(true, true, false, false);
            var appended = seq.AppendBits(4);
            Assert.AreEqual(12, appended.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(
                ArrayUtil.Of(false, false, true, false),
                appended[4..8]));
        }
    }
}
