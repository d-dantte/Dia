using Axis.Dia.PathQuery.Predicates;
using Axis.Dia.PathQuery.Predicates.Wildcard;

namespace Axis.Dia.PathQuery.Tests.Predicates
{
    [TestClass]
    public class IWildcardTokenTetss
    {
        [TestMethod]
        public void Parse_Tests()
        {
            var result = IWildcardToken.Parse("abcde");
            Assert.IsInstanceOfType<TokenSequence>(result);
        }
    }
}
