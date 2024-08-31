using Axis.Dia.Convert.Json;

namespace Axis.Dia.Tests.Convert.Json
{
    [TestClass]
    public class GrammarUtilTests
    {
        [TestMethod]
        public void GenerateGrammarTests()
        {
            var grammar = GrammarUtil.Grammar;
            Assert.IsNotNull(grammar);
        }
    }
}
