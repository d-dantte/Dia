using Axis.Dia.PathQuery.Predicates.Wildcard;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.PathQuery.Tests.Predicates.Wildcard
{
    [TestClass]
    public class TokenTests
    {
        [TestMethod]
        public void Constructor_Tests()
        {
            var token = new Token(new TokenCardinality(1, 1), null);
            Assert.AreEqual(new TokenCardinality(1, 1), token.Cardinality);
            Assert.IsNull(token.Char);
            Assert.IsTrue(token.IsWildcard);
        }

        [TestMethod]
        public void IsMatch_Tests()
        {
            var token = new Token(new TokenCardinality(1, 1), null);

            var isMatch = token.IsMatch("a", out var tokenCount);
            Assert.IsTrue(isMatch);
            Assert.AreEqual(1, tokenCount);

            token = new Token(new TokenCardinality(2, 2), null);
            isMatch = token.IsMatch("ab", out tokenCount);
            Assert.IsTrue(isMatch);
            Assert.AreEqual(2, tokenCount);

            token = new Token(new TokenCardinality(2, 2), 'a');
            isMatch = token.IsMatch("aa", out tokenCount);
            Assert.IsTrue(isMatch);
            Assert.AreEqual(2, tokenCount);

            token = new Token(new TokenCardinality(2, 2), 'a');
            isMatch = token.IsMatch("ab", out tokenCount);
            Assert.IsFalse(isMatch);
            Assert.AreEqual(1, tokenCount);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => Token.Parse(ISymbolNode.Of("bleh", "helb")));
            Assert.ThrowsException<FormatException>(
                () => Token.Parse("("));
            Assert.ThrowsException<ArgumentNullException>(
                () => Token.Parse(default(ISymbolNode)!));

            var result = Token.Parse("a{1}");
            Assert.AreEqual(Token.Of("{1}", 'a'), result);

            result = Token.Parse("b{+}");
            Assert.AreEqual(Token.Of("{+}", 'b'), result);

            result = Token.Parse("_");
            Assert.AreEqual(Token.Of("{1}", null), result);

            result = Token.Parse("\\_");
            Assert.AreEqual(Token.Of("{1}", '_'), result);
        }
    }
}
