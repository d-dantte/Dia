using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Dia.PathQuery.Matchers;
using Axis.Dia.PathQuery.Predicates;

namespace Axis.Dia.PathQuery.Tests
{
    [TestClass]
    public class PathTests
    {
        [TestMethod]
        public void Construction_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Path(null!));
            Assert.ThrowsException<InvalidOperationException>(
                () => new Path([null!]));

            var path = new Path(
                new Segment(new IndexRangeMatcher(..)));

            Assert.AreEqual(1, path.Segments.Length);
        }

        [TestMethod]
        public void FilterValue_Tests()
        {
            var path = new Path(
                new Segment(new IndexRangeMatcher(..^1)));

            var results = path
                .FilterValues(new Sequence
                {
                    Core.Types.Decimal.Null(),
                    false,
                    DateTimeOffset.Now
                })
                .ToArray();

            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(Core.DiaType.Decimal, results[0].Type);
            Assert.AreEqual(Core.DiaType.Bool, results[1].Type);
        }

        [TestMethod]
        public void Parse_Tests()
        {
            var result = Path.Parse("/:abcd/:`123`/!#..^3");
            Assert.AreEqual(3, result.Segments.Length);
        }

        #region Filter
        [TestMethod]
        public void FilterValues_WithIndexRange_Tests()
        {
            var path = new Path(new Segment(new IndexRangeMatcher(^1..)));

            Assert.ThrowsException<ArgumentNullException>(
                () => path.FilterValues(null!));

            var result = path
                .FilterValues(new Sequence() { 123, "abc", false })
                .ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(Core.DiaType.Bool, result[0].Type);
        }

        [TestMethod]
        public void FilterValues_WithTypeMatcher_Tests()
        {
            var path = new Path(
                new Segment(
                    new TypeMatcher(
                        Core.DiaType.Int,
                        Core.DiaType.Timestamp)));

            var result = path.FilterValues(Core.Types.Integer.Null()).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(Core.DiaType.Int, result[0].Type);
        }

        [TestMethod]
        public void FilterValues_WithAttributeMatcher_Tests()
        {
            var path = new Path(
                new Segment(
                    new AttributeMatcher(
                        (RegexPredicate.Of("flag1"), default),
                        (RegexPredicate.Of(".*"), RegexPredicate.Of("value\\d+")))));

            var result = path
                .FilterValues(
                    Duration.Null("flag1", ("abc", "value0")))
                .ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(Core.DiaType.Duration, result[0].Type);
        }

        [TestMethod]
        public void FilterValues_WithPropertyNameeMatcher_Tests()
        {
            var path = new Path(
                new Segment(
                    new PropertyNameMatcher(
                        RegexPredicate.Of("xyz-prop.*"),
                        new AttributeMatcher(
                            (RegexPredicate.Of(".*"), RegexPredicate.Of("value\\d*"))))));

            var result = path
                .FilterValues(
                    new Record
                    {
                        ["xyz-prop0"] = Timestamp.Null(),
                        [Record.PropertyName.Of("xyz-prop1", ("ab", "value"))] = 23,
                        [Record.PropertyName.Of("xyz-prop", ("melti", "value0"))] = DateTimeOffset.Now,
                    })
                .ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Core.DiaType.Int, result[0].Type);
            Assert.AreEqual(Core.DiaType.Timestamp, result[1].Type);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeleteValues_WithNullValue()
        {
            var path = Path.Parse("/:abcd");

            Assert.ThrowsException<ArgumentNullException>(
                () => path.DeleteValues(null!));
        }

        [TestMethod]
        public void DeleteValues_WithIndexRange_Tests()
        {
            var path = Path.Parse("/#..^1");
            var seq = new Sequence() { 123, "abc", false };

            var result = path
                .DeleteValues(seq)
                .ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(1, seq.Count);
        }

        [TestMethod]
        public void Delete_WithPropertyNameeMatcher_Tests()
        {
            var path = new Path(
                new Segment(
                    new PropertyNameMatcher(
                        RegexPredicate.Of("xyz-prop.*"),
                        new AttributeMatcher(
                            (RegexPredicate.Of(".*"), RegexPredicate.Of("value\\d*"))))));

            var rec = new Record
            {
                ["xyz-prop0"] = Timestamp.Null(),
                [Record.PropertyName.Of("xyz-prop1", ("ab", "value"))] = 23,
                [Record.PropertyName.Of("xyz-prop", ("melti", "value0"))] = DateTimeOffset.Now,
            };

            var result = path
                .DeleteValues(rec)
                .ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Core.DiaType.Int, result[0].Type);
            Assert.AreEqual(Core.DiaType.Timestamp, result[1].Type);
            Assert.AreEqual(1, rec.Count);
            Assert.IsTrue(rec.ContainsProperty("xyz-prop0"));
        }

        [TestMethod]
        public void DeleteValues_WithTypeMatcher_Tests()
        {
            var path = new Path(
                new Segment(
                    new TypeMatcher(
                        Core.DiaType.Int,
                        Core.DiaType.Timestamp)));

            var result = path.DeleteValues(Core.Types.Integer.Null()).ToArray();
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void DeleteValues_WithAttributeMatcher_Tests()
        {
            var path = new Path(
                new Segment(
                    new AttributeMatcher(
                        (RegexPredicate.Of("flag1"), default),
                        (RegexPredicate.Of(".*"), RegexPredicate.Of("value\\d+")))));

            var result = path
                .DeleteValues(
                    Duration.Null("flag1", ("abc", "value0")))
                .ToArray();
            Assert.AreEqual(0, result.Length);
        }
        #endregion

        #region Modify
        [TestMethod]
        public void ReplaceValues_WithNullValue()
        {
            var path = Path.Parse("/:abcd");

            Assert.ThrowsException<ArgumentNullException>(
                () => path.ReplaceValues(null!, FalseDummy));

            Assert.ThrowsException<ArgumentNullException>(
                () => path.ReplaceValues(Duration.Null(), null!));
        }

        [TestMethod]
        public void ReplaceValues_WithIndexRange_Tests()
        {
            var path = Path.Parse("/:stuff/#..^1");
            var seq = new Sequence() { 123, "abc", false };
            var rec = new Record { ["stuff"] = seq };

            var values = new Dictionary<object, IDiaValue>
            {
                [0] = Integer.Of(321, "me-me"),
                [1] = Core.Types.String.Of("cba"),
                [2] = Symbol.Of("sudoku")
            };
            var result = path
                .ReplaceValues(rec, ProducerFor(values))
                .ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(3, seq.Count);
            Assert.AreEqual(values[0], seq[0].Payload);
            Assert.AreEqual(values[1], seq[1].Payload);


            path = Path.Parse("/:stuff/!#..^1");
            seq = new Sequence() { 123, "abc", false };
            rec = new Record { ["stuff"] = seq };
            result = path
                .ReplaceValues(rec, ProducerFor(values))
                .ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(3, seq.Count);
            Assert.AreEqual(values[2], seq[2].Payload);
        }

        [TestMethod]
        public void Replace_WithPropertyNameeMatcher_Tests()
        {
            var path = new Path(
                new Segment(
                    new PropertyNameMatcher(
                        RegexPredicate.Of("xyz-prop.*"),
                        new AttributeMatcher(
                            (RegexPredicate.Of(".*"), RegexPredicate.Of("value\\d*"))))));

            var rec = new Record
            {
                ["xyz-prop0"] = Timestamp.Null(),
                [Record.PropertyName.Of("xyz-prop1", ("ab", "value"))] = 23,
                [Record.PropertyName.Of("xyz-prop", ("melti", "value0"))] = DateTimeOffset.Now,
            };

            var replacements = new Dictionary<object, IDiaValue>
            {
                [Record.PropertyName.Of("xyz-prop", ("melti", "value0"))] = Duration.Null(),
                [Record.PropertyName.Of("xyz-prop1", ("ab", "value"))] = Blob.Null()
            };

            var result = path
                .ReplaceValues(rec, ProducerFor(replacements))
                .ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(DiaType.Int, result[0].Type);
            Assert.AreEqual(DiaType.Timestamp, result[1].Type);
            Assert.AreEqual(3, rec.Count);
            Assert.AreEqual(DiaType.Duration, rec["xyz-prop"].Type);
            Assert.AreEqual(DiaType.Blob, rec["xyz-prop1"].Type);
        }
        #endregion

        #region Seek
        #endregion

        private static IModify.ConditionalValueProvider ProducerFor(
            Dictionary<object, IDiaValue> valueMap)
        {
            return ((object Key, IDiaValue Value) keyValuePair, out IDiaValue? value) =>
            {
                return valueMap.TryGetValue(keyValuePair.Key, out value);
            };
        }

        private bool FalseDummy(
            (object Key, IDiaValue Value) keyValuePair,
            out IDiaValue? value)
        {
            value = null;
            return false;
        }
    }
}
