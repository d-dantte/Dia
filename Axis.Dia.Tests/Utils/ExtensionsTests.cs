using Axis.Dia.Utils;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;

namespace Axis.Dia.Tests.Utils
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void SkipEveryNth_Tests()
        {
            var seq = "0000100001000010000100001";
            var skipped = seq
                .SkipEveryNth(5)
                .ApplyTo(s => new string(s.ToArray()));

            Assert.IsFalse(skipped.Contains('1'));
            Assert.AreEqual(20, skipped.Length);
        }

        [TestMethod]
        public void JoinUsing_Tests()
        {
            var arrr = ArrayUtil.Of(
                ArrayUtil.Of(0, 0, 0, 0),
                ArrayUtil.Of(0, 0, 0),
                ArrayUtil.Of(0, 0),
                ArrayUtil.Of(0));

            var joined = arrr
                .JoinUsing(ArrayUtil.Of(1, 1))
                .ToArray();

            Assert.AreEqual(7, joined.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(joined[0], ArrayUtil.Of(0, 0, 0, 0)));
            Assert.IsTrue(Enumerable.SequenceEqual(joined[1], ArrayUtil.Of(1, 1)));
            Assert.IsTrue(Enumerable.SequenceEqual(joined[2], ArrayUtil.Of(0, 0, 0)));
            Assert.IsTrue(Enumerable.SequenceEqual(joined[3], ArrayUtil.Of(1, 1)));
            Assert.IsTrue(Enumerable.SequenceEqual(joined[4], ArrayUtil.Of(0, 0)));
            Assert.IsTrue(Enumerable.SequenceEqual(joined[5], ArrayUtil.Of(1, 1)));
            Assert.IsTrue(Enumerable.SequenceEqual(joined[6], ArrayUtil.Of(0)));

        }

        [TestMethod]
        public void TryNormalizeDiaValue_Tests()
        {
            throw new NotImplementedException();
        }
    }
}
