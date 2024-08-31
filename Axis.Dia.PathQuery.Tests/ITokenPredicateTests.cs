using Axis.Dia.PathQuery.Predicates;

namespace Axis.Dia.PathQuery.Tests
{
    [TestClass]
    public class ITokenPredicateTests
    {
        [TestMethod]
        public void Parse_Tests()
        {
            var result = ITokenPredicate.Parse("abc(def){+}");
            Assert.IsInstanceOfType<IWildcardToken>(result);

            result = ITokenPredicate.Parse("`bleh`");
            Assert.IsInstanceOfType<RegexPredicate>(result);
        }
    }
}
