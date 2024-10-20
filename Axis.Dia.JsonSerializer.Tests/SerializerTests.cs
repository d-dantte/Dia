using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Json.Tests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void Serialize_WhenValueIsRecord()
        {
            var serializer = new Serializer();
            var record = new Record
            {
                ["first"] = 34,
                ["second"] = 3.45m,
                ["third"] = Blob.Null("att", ("k", "v")),
                ["foruth"] = DateTimeOffset.Now,
                ["fifth"] = new Sequence
                {
                    "bleh", TimeSpan.FromSeconds(5454.345),
                    Symbol.Of("xyz", "att1", "att2")
                }
            };
            record["self"] = record;

            var jobj = serializer.Serialize(record);
            Console.WriteLine(jobj.ToString());
        }

        [TestMethod]
        public void Deserialize_WhenValueIsRecord()
        {
            var serializer = new Serializer();
            var record = new Record
            {
                ["first"] = 34,
                ["second"] = 3.45m,
                ["third"] = Blob.Null("att", ("k", "v")),
                ["foruth"] = DateTimeOffset.Now,
                ["fifth"] = new Sequence
                {
                    "bleh", TimeSpan.FromSeconds(5454.345),
                    Symbol.Of("xyz", "att1", "att2")
                }
            };
            record["self"] = record;

            var jobj = serializer.Serialize(record);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Record resultRecord));
            Assert.IsTrue(record.ValueEquals(resultRecord));
            Assert.IsTrue(record.IsStructurallyEquivalent(resultRecord));
        }

        [TestMethod]
        public void Deserialize_WhenIsSequence()
        {
            var serializer = new Serializer();
            var seq = new Sequence
            {
                345, false, "bleh", DateTimeOffset.Now,
                Symbol.Of("symbolistic", "att", ("x", "y")),
                Core.Types.Decimal.Of(0.00003564m, "some-att")
            };
            seq.Add(seq);
            seq.Add(seq);

            var jobj = serializer.Serialize(seq);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Sequence resultSeq));
            Assert.IsTrue(seq.IsStructurallyEquivalent(resultSeq));
            Assert.IsTrue(seq.ValueEquals(resultSeq));
        }

        [TestMethod]
        public void Deserialize_WhenIsBlob()
        {
            var serializer = new Serializer();

            #region Null
            var value = Blob.Null("abcd");
            var jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Blob resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion

            #region Non-Null
            value = Blob.Of([1, 2, 3, 4, 5, 6, 7, 8, 9, 0]);
            jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion
        }

        [TestMethod]
        public void Deserialize_WhenIsBool()
        {
            var serializer = new Serializer();

            #region Null
            var value = Core.Types.Boolean.Null("abcd");
            var jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Core.Types.Boolean resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion

            #region Non-Null
            value = Core.Types.Boolean.Of(true);
            jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion
        }

        [TestMethod]
        public void Deserialize_WhenIsDuration()
        {
            var serializer = new Serializer();

            #region Null
            var value = Core.Types.Duration.Null("abcd");
            var jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Core.Types.Duration resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion

            #region Non-Null
            value = Core.Types.Duration.Of(TimeSpan.FromSeconds(434564.55654));
            jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion
        }

        [TestMethod]
        public void Deserialize_WhenIsInteger()
        {
            var serializer = new Serializer();

            #region Null
            var value = Core.Types.Integer.Null("abcd");
            var jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Core.Types.Integer resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion

            #region Non-Null
            value = Core.Types.Integer.Of(543546543500000L);
            jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion
        }

        [TestMethod]
        public void Deserialize_WhenIsString()
        {
            var serializer = new Serializer();

            #region Null
            var value = Core.Types.String.Null("abcd");
            var jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Core.Types.String resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion

            #region Non-Null
            value = Core.Types.String.Of("the quick brown fox, blah, blah, blah");
            jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion
        }

        [TestMethod]
        public void Deserialize_WhenIsSymbol()
        {
            var serializer = new Serializer();

            #region Null
            var value = Core.Types.Symbol.Null("abcd");
            var jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Core.Types.Symbol resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion

            #region Non-Null
            value = Core.Types.Symbol.Of("the quick brown fox, blah, blah, blah");
            jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion
        }

        [TestMethod]
        public void Deserialize_WhenIsTimestamp()
        {
            var serializer = new Serializer();

            #region Null
            var value = Core.Types.Timestamp.Null("abcd");
            var jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            var result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out Core.Types.Timestamp resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion

            #region Non-Null
            value = Core.Types.Timestamp.Of(DateTimeOffset.Now);
            jobj = serializer.Serialize(value);
            Console.WriteLine(jobj.ToString());

            result = serializer.Deserialize(jobj);
            Assert.IsTrue(result.Is(out resultValue));
            Assert.IsTrue(value.ValueEquals(resultValue));
            #endregion
        }
    }
}
