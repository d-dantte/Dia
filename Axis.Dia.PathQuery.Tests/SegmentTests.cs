using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Matchers;
using Axis.Dia.PathQuery.Predicates;

namespace Axis.Dia.PathQuery.Tests
{
    [TestClass]
    public class SegmentTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Segment(null!));

            var segment = new Segment(new IndexRangeMatcher(..));
            Assert.IsInstanceOfType<IndexRangeMatcher>(segment.Matcher);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var result = Segment.Parse("/:abcd");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<PropertyNameMatcher>(result.Matcher);

            result = Segment.Parse("/!:abcd");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<PropertyNameMatcher>(result.Matcher);
            Assert.IsTrue(result.Matcher.IsNegated);
        }
    }
}
