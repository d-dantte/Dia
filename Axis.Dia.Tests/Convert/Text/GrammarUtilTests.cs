using Axis.Dia.Convert.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.Tests.Convert.Text
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
