using Axis.Dia.PathQuery.Predicates;
using Axis.Pulsar.Core.CST;
using System.Text.RegularExpressions;

namespace Axis.Dia.PathQuery.Tests.Predicates
{
    [TestClass]
    public class RegexPredicateTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new RegexPredicate(default(Regex)!));

            var rpred = RegexPredicate.Of("abcd");
            Assert.AreEqual("abcd", rpred.Regex.ToString());

            rpred = RegexPredicate.Of(new Regex("abcd"));
            Assert.AreEqual("abcd", rpred.Regex.ToString());
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var regex = RegexPredicate.Parse("`abcd`");
            Assert.AreEqual("abcd", regex.Regex.ToString());

            Assert.ThrowsException<FormatException>(
                () => RegexPredicate.Parse("bleh"));

            var symbol = ISymbolNode.Of("abc", "abc");
            Assert.ThrowsException<InvalidOperationException>(
                () => RegexPredicate.Parse(symbol));
        }
    }
}
