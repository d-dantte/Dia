using Axis.Dia.Core;
using Axis.Dia.PathQuery.Matchers;

namespace Axis.Dia.PathQuery.Tests.Matchers
{
    [TestClass]
    public class TypeMatcherTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new TypeMatcher(null!));

            Assert.ThrowsException<ArgumentException>(
                () => new TypeMatcher([]));

            var dt = (DiaType)int.MaxValue;
            Assert.ThrowsException<InvalidOperationException>(
                () => new TypeMatcher([dt]));

            var matcher = new TypeMatcher(
                DiaType.Symbol,
                DiaType.Blob,
                DiaType.Blob,
                DiaType.Record);

            Assert.AreEqual(3, matcher.Types.Count);
            Assert.IsTrue(matcher.Types.Contains(DiaType.Blob));
            Assert.IsTrue(matcher.Types.Contains(DiaType.Symbol));
            Assert.IsTrue(matcher.Types.Contains(DiaType.Record));
        }

        [TestMethod]
        public void IsMatch_Tests()
        {
            var matcher = new TypeMatcher(
                DiaType.Symbol,
                DiaType.Blob,
                DiaType.Record);

            Assert.AreEqual(3, matcher.Types.Count);
            Assert.IsTrue(matcher.IsMatch(Core.Types.Blob.Null()));
            Assert.IsTrue(matcher.IsMatch(Core.Types.Symbol.Null()));
            Assert.IsTrue(matcher.IsMatch(Core.Types.Record.Null()));
        }

        [TestMethod]
        public void IsMatch_WithNegation_Tests()
        {
            var matcher = new TypeMatcher(
                true,
                DiaType.Symbol,
                DiaType.Blob,
                DiaType.Record);

            Assert.AreEqual(3, matcher.Types.Count);
            Assert.IsFalse(matcher.IsMatch(Core.Types.Blob.Null()));
            Assert.IsFalse(matcher.IsMatch(Core.Types.Symbol.Null()));
            Assert.IsFalse(matcher.IsMatch(Core.Types.Record.Null()));
            Assert.IsTrue(matcher.IsMatch(Core.Types.Sequence.Null()));
            Assert.IsTrue(matcher.IsMatch(Core.Types.Duration.Null()));
            Assert.IsTrue(matcher.IsMatch(Core.Types.Decimal.Null()));
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var matcher = TypeMatcher.Parse(
                " $int $Decimal $timeStamp $duration $String $symbol $record $sequence $blob $bool");
            Assert.AreEqual(10, matcher.Types.Count);
            Assert.IsTrue(matcher.Types.Contains(DiaType.Int));
            Assert.IsTrue(matcher.Types.Contains(DiaType.Decimal));
            Assert.IsTrue(matcher.Types.Contains(DiaType.Timestamp));
        }
    }
}
