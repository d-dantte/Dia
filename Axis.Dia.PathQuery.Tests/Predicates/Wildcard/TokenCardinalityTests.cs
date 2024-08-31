using Axis.Dia.PathQuery.Predicates.Wildcard;
using Axis.Luna.Common;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.PathQuery.Tests.Predicates.Wildcard
{
    [TestClass]
    public class TokenCardinalityTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => TokenCardinality.Of(0, 0));
            Assert.ThrowsException<InvalidOperationException>(
                () => TokenCardinality.Of(1, 0));

            var cardinality = TokenCardinality.Of(0, 1);
            Assert.AreEqual(0, cardinality.MinOccurence);
            Assert.AreEqual(1, cardinality.MaxOccurence!.Value);
            Assert.IsTrue(cardinality.IsOptionalMinOccurence);
            Assert.IsFalse(cardinality.IsInfiniteMaxOccurence);
        }

        [TestMethod]
        public void IsValidRepetition_Tests()
        {
            var cardinality = TokenCardinality.Of(1, 1);
            Assert.IsTrue(cardinality.IsValidRepetition(1));
            Assert.IsFalse(cardinality.IsValidRepetition(0));
            Assert.IsFalse(cardinality.IsValidRepetition(2));

            cardinality = TokenCardinality.Of(0, null);
            Assert.IsTrue(cardinality.IsValidRepetition(0));
            Assert.IsTrue(cardinality.IsValidRepetition(2));
            Assert.IsTrue(cardinality.IsValidRepetition(3));
        }

        [TestMethod]
        public void CanRepeat_Tests()
        {
            var cardinality = TokenCardinality.Of(1, 1);
            Assert.IsTrue(cardinality.CanRepeat(0));
            Assert.IsFalse(cardinality.CanRepeat(1));

            cardinality = TokenCardinality.Of(0, null);
            Assert.IsTrue(cardinality.CanRepeat(0));
            Assert.IsTrue(cardinality.CanRepeat(2));
            Assert.IsTrue(cardinality.CanRepeat(3));
        }

        [TestMethod]
        public void IsMatch_Tests()
        {
            var cardinality = TokenCardinality.Of(1, 1);
            Assert.IsTrue(cardinality.IsMatch("abc", PredicateOf(true, 2), out var count));
            Assert.AreEqual(2, count);

            cardinality = TokenCardinality.Of(1, 1);
            Assert.IsFalse(cardinality.IsMatch("abc", PredicateOf(false, 0), out count));
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => TokenCardinality.Parse(ISymbolNode.Of("bleh", "helb")));
            Assert.ThrowsException<FormatException>(
                () => TokenCardinality.Parse("dflka"));
            Assert.ThrowsException<ArgumentNullException>(
                () => TokenCardinality.Parse(default(ISymbolNode)!));

            var result = TokenCardinality.Parse("{*}");
            Assert.AreEqual(TokenCardinality.Of(0, null), result);

            result = TokenCardinality.Parse("{+}");
            Assert.AreEqual(TokenCardinality.Of(1, null), result);

            result = TokenCardinality.Parse("{?}");
            Assert.AreEqual(TokenCardinality.Of(0, 1), result);

            result = TokenCardinality.Parse("{1,1}");
            Assert.AreEqual(TokenCardinality.Of(1, 1), result);

            result = TokenCardinality.Parse("{2,+}");
            Assert.AreEqual(TokenCardinality.Of(2, null), result);
        }

        internal CardinalPredicate PredicateOf(bool result, int count)
        {
            return (CharSequence x, out int y) =>
            {
                y = count;
                return result;
            };
        }
    }
}
