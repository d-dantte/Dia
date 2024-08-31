using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Matchers;

namespace Axis.Dia.PathQuery.Tests.Matchers
{
    [TestClass]
    public class IndexRangeTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            var ir = new IndexRangeMatcher(..);
            Assert.IsNotNull(ir);
            Assert.AreEqual(.., ir.Range);
        }

        [TestMethod]
        public void GetIndex_Tests()
        {
            Assert.AreEqual(0, IndexRangeMatcher.GetIndex(0, 10));
            Assert.AreEqual(-1, IndexRangeMatcher.GetIndex(^1, 0));
        }

        [TestMethod]
        public void IsMatch_Tests()
        {
            var ir = new IndexRangeMatcher(..);
            var seq = new Sequence { 1, true, DateTimeOffset.Now, TimeSpan.FromSeconds(2.344) };
            Assert.IsTrue(ir.IsMatch(seq, out var ranges));
            Assert.AreEqual(1, ranges.Length);

            ir = new IndexRangeMatcher(2..);
            Assert.IsTrue(ir.IsMatch(seq, out ranges));
            Assert.AreEqual(1, ranges.Length);
            Assert.IsTrue(ranges.Contains(2..4));
        }

        [TestMethod]
        public void IsMatch_WithNegation_Tests()
        {
            var ir = new IndexRangeMatcher(true, ..);
            var seq = new Sequence { 1, true, DateTimeOffset.Now, TimeSpan.FromSeconds(2.344) };
            Assert.IsTrue(ir.IsMatch(seq, out var ranges));
            Assert.IsTrue(ranges.Contains(0..0));
            Assert.IsTrue(ranges.Contains(4..4));

            ir = new IndexRangeMatcher(true, 2..);
            Assert.IsTrue(ir.IsMatch(seq, out ranges));
            Assert.IsTrue(ranges.Contains(0..2));
            Assert.IsTrue(ranges.Contains(4..4));
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var ir = IndexRangeMatcher.Parse("#..");
            Assert.AreEqual(.., ir.Range);

            ir = IndexRangeMatcher.Parse("#1..", true);
            Assert.AreEqual(1.., ir.Range);
            Assert.IsTrue(ir.IsNegated);

            ir = IndexRangeMatcher.Parse("#^1..", true);
            Assert.AreEqual(^1.., ir.Range);
            Assert.IsTrue(ir.IsNegated);

            ir = IndexRangeMatcher.Parse("#1..5");
            Assert.AreEqual(1..5, ir.Range);

            ir = IndexRangeMatcher.Parse("#1..^5");
            Assert.AreEqual(1..^5, ir.Range);

            ir = IndexRangeMatcher.Parse("#..5");
            Assert.AreEqual(..5, ir.Range);

            ir = IndexRangeMatcher.Parse("#..^5");
            Assert.AreEqual(..^5, ir.Range);
        }
    }
}
