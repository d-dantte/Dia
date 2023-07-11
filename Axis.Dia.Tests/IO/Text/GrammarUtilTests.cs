using Axis.Dia.IO.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.Tests.IO.Text
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
