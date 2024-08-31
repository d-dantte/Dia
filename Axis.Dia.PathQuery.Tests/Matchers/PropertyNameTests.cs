using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Matchers;
using Axis.Dia.PathQuery.Predicates;
using Axis.Dia.PathQuery.Predicates.Wildcard;

namespace Axis.Dia.PathQuery.Tests.Matchers
{
    [TestClass]
    public class PropertyNameTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            var predicate = TokenSequence.Of(
                Token.Of('a'),
                Token.Of('b'),
                Token.Of('c'),
                Token.Of('d'));

            Assert.ThrowsException<ArgumentNullException>(
                () => new PropertyNameMatcher(false, null!, null));

            var pn = new PropertyNameMatcher(false, predicate, null);
            Assert.IsFalse(pn.IsNegated);
            Assert.IsNull(pn.AttributeMatcher);
            Assert.AreEqual(predicate, pn.NameMatcher);
        }

        [TestMethod]
        public void IsMatch_Tests()
        {
            var predicate = TokenSequence.Of(
                Token.Of('a'),
                Token.Of('b'),
                Token.Of('c'),
                Token.Of('d'));

            var attMatcher = new AttributeMatcher(
                (RegexPredicate.Of("flag"), null));

            var pn1 = new PropertyNameMatcher(false, predicate, null);
            var pn2 = new PropertyNameMatcher(false, predicate, attMatcher);
            var pn3 = new PropertyNameMatcher(true, predicate, attMatcher);

            Assert.IsTrue(pn1.IsMatch(Record.PropertyName.Of("abcd")));
            Assert.IsTrue(pn2.IsMatch(Record.PropertyName.Of("abcd", "flag")));
            Assert.IsFalse(pn2.IsMatch(Record.PropertyName.Of("abcd", "fleg")));
            Assert.IsTrue(pn3.IsMatch(Record.PropertyName.Of("xyz", "foo")));
            Assert.IsFalse(pn3.IsMatch(Record.PropertyName.Of("abcd", "flag")));
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var result = PropertyNameMatcher.Parse(":abcd");
            Assert.IsInstanceOfType<TokenSequence>(result.NameMatcher);
            Assert.IsNull(result.AttributeMatcher);

            result = PropertyNameMatcher.Parse(":`abcd` @flag;");
            Assert.IsInstanceOfType<RegexPredicate>(result.NameMatcher);
            Assert.AreEqual(1, result.AttributeMatcher!.PredicateCount);
        }
    }
}
