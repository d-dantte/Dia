using Axis.Dia.PathQuery.Predicates.Wildcard;

namespace Axis.Dia.PathQuery.Tests.Predicates.Wildcard
{
    [TestClass]
    public class TokenSequenceTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new TokenSequence(default, null!));
            Assert.ThrowsException<ArgumentException>(
                () => new TokenSequence(default, []));
            Assert.ThrowsException<InvalidOperationException>(
                () => new TokenSequence(default, [null!]));

            var tseq = TokenSequence.Of(
                TokenCardinality.Of(1, 1),
                Token.Of(TokenCardinality.Of(1, 1), 'a'),
                Token.Of(TokenCardinality.Of(1, 1), 'b'),
                Token.Of(TokenCardinality.Of(1, 1), '_'));

            Assert.AreEqual(TokenCardinality.Of(1, 1), tseq.Cardinality);
            Assert.AreEqual(3, tseq.Tokens.Length);
        }

        [TestMethod]
        public void IsMatch_Tests()
        {
            var tseq = TokenSequence.Of(
                TokenCardinality.Of(1, 1),
                Token.Of(TokenCardinality.Of(1, 1), 'a'),
                Token.Of(TokenCardinality.Of(1, 1), 'b'),
                Token.Of(TokenCardinality.Of(1, 1), null));

            var isMatch = tseq.IsMatch("abcdef", out var count);
            Assert.IsTrue(isMatch);
            Assert.AreEqual(3, count);

            tseq = TokenSequence.Of(
                TokenCardinality.Of(1, 1),
                Token.Of(TokenCardinality.Of(1, 1), 'a'),
                Token.Of(TokenCardinality.Of(1, 1), 'b'),
                Token.Of(TokenCardinality.Of(1, 1), null),
                Token.Of(TokenCardinality.Of(0, 1), 'x'));

            isMatch = tseq.IsMatch("abcdef", out count);
            Assert.IsTrue(isMatch);
            Assert.AreEqual(3, count);

            tseq = TokenSequence.Of(
                TokenCardinality.Of(1, 1),
                Token.Of(TokenCardinality.Of(1, 1), 'a'),
                Token.Of(TokenCardinality.Of(1, 1), 'b'),
                Token.Of(TokenCardinality.Of(1, 1), null),
                Token.Of(TokenCardinality.Of(0, 1), 'x'));

            isMatch = tseq.IsMatch("ab", out count);
            Assert.IsFalse(isMatch);
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var result = TokenSequence.Parse("abcd");
            Assert.AreEqual(4, result.Tokens.Length);
            Assert.AreEqual("abcd", result.ToString());

            result = TokenSequence.Parse("(abcd)");
            Assert.AreEqual(1, result.Tokens.Length);
            Assert.AreEqual("abcd", result.ToString());

            result = TokenSequence.Parse("(abcd){*}");
            Assert.AreEqual(1, result.Tokens.Length);
            Assert.AreEqual("(abcd){*}", result.ToString());

            result = TokenSequence.Parse("a(bc){+}d)");
            Assert.AreEqual(3, result.Tokens.Length);
            Assert.AreEqual("a(bc){+}d", result.ToString());

            result = TokenSequence.Parse("a(_c){+}d)");
            Assert.AreEqual(3, result.Tokens.Length);
            Assert.AreEqual("a(_c){+}d", result.ToString());

            result = TokenSequence.Parse("a(b(c){*}){+}d)");
            Assert.AreEqual("a(b(c){*}){+}d", result.ToString());

            result = TokenSequence.Parse("a(bc{*}){+}d)");
            Assert.AreEqual(3, result.Tokens.Length);
            Assert.AreEqual("a(bc{*}){+}d", result.ToString());
        }
    }
}
