using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Matchers;
using Axis.Dia.PathQuery.Predicates;

namespace Axis.Dia.PathQuery.Tests.Matchers
{
    [TestClass]
    public class AttributeMatcherTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new AttributeMatcher(null!));

            Assert.ThrowsException<ArgumentException>(
                () => new AttributeMatcher([]));

            Assert.ThrowsException<InvalidOperationException>(
                () => new AttributeMatcher([(null!, null)]));

            var predicate = (
                new RegexPredicate("abc"),
                default(ITokenPredicate));
            var matcher = new AttributeMatcher([predicate]);

            Assert.AreEqual(1, matcher.PredicateCount);
            Assert.AreEqual(predicate, matcher.Predicates[0]);
        }

        [TestMethod]
        public void IsMatch_Tests()
        {
            var value = Blob.Null("abc", ("att", "value"));

            var matcher = new AttributeMatcher([
                (new RegexPredicate("abc"), default(ITokenPredicate))]);
            Assert.IsTrue(matcher.IsMatch(value, out var atts));
            Assert.IsTrue(atts.Contains("abc"));

            matcher = new AttributeMatcher([
                (new RegexPredicate("abcd"), default(ITokenPredicate))]);
            Assert.IsFalse(matcher.IsMatch(value, out atts));

            matcher = new AttributeMatcher([
                (new RegexPredicate("abc"), new RegexPredicate("123"))]);
            Assert.IsFalse(matcher.IsMatch(value, out atts));

            matcher = new AttributeMatcher([
                (new RegexPredicate("abc"), default(ITokenPredicate)),
                (new RegexPredicate("^..t$"), new RegexPredicate("value"))]);
            Assert.IsTrue(matcher.IsMatch(value, out atts));
            Assert.AreEqual(2, atts.Count);
        }

        [TestMethod]
        public void IsMatch_WithNegation_Tests()
        {
            var value = Blob.Null("abc", ("att", "value"));

            var matcher = new AttributeMatcher(true, [
                (new RegexPredicate("abc"), default(ITokenPredicate))]);
            Assert.IsTrue(matcher.IsMatch(value, out var atts));
            Assert.IsTrue(atts.Contains(("att", "value")));

            matcher = new AttributeMatcher(true, [
                (new RegexPredicate("abcd"), default(ITokenPredicate))]);
            Assert.IsTrue(matcher.IsMatch(value, out atts));
            Assert.AreEqual(2, atts.Count);

            matcher = new AttributeMatcher(true, [
                (new RegexPredicate("abc"), new RegexPredicate("123"))]);
            Assert.IsTrue(matcher.IsMatch(value, out atts));
            Assert.AreEqual(2, atts.Count);

            matcher = new AttributeMatcher(true, [
                (new RegexPredicate("abc"), default(ITokenPredicate)),
                (new RegexPredicate("^..t$"), new RegexPredicate("value"))]);
            Assert.IsFalse(matcher.IsMatch(value, out atts));
        }

        [TestMethod]
        public void Parse_Test()
        {
            var result = AttributeMatcher.Parse("@abc; @`bleh`:`more bleh`;");
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result!.PredicateCount);
        }
    }
}
