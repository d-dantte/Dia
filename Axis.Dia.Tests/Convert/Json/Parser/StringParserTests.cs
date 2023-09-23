using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class StringParserTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentException>(() => StringParser.Serialize(
                StringValue.Of(""),
                default));

            var context = new SerializerContext();
            var result = StringParser.Serialize(StringValue.Null(), context);
            Assert.IsTrue(result.IsErrorResult());

            result = StringParser.Serialize(StringValue.Of("", "stuff"), context);
            Assert.IsTrue(result.IsErrorResult());

            // values
            result = StringParser.Serialize(StringValue.Of("the quick brown bla, bla, bla"), context);
            Assert.AreEqual("\"the quick brown bla, bla, bla\"", result.Resolve());

            result = StringParser.Serialize(StringValue.Of("the quick brown bla, bla, bla \b \f \n \r \t \\ / \" \ua12c "), context);
            Assert.AreEqual("\"the quick brown bla, bla, bla \\b \\f \\n \\r \\t \\\\ \\/ \\\" \\ua12c \"", result.Resolve());
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => StringParser.Parse(
                null,
                new ParserContext()));

            Assert.ThrowsException<ArgumentException>(() => StringParser.Parse(
                CSTNode.Of("dummy", "stuff"),
                default));

            Assert.ThrowsException<ArgumentException>(() => StringParser.Parse(
                CSTNode.Of("dummy", "stuff"),
                new ParserContext()));

            var context = new ParserContext();


            // values
            var node = ParserUtil.ParseTokens("string-value", "\"the quick brown bla, bla, bla\"");
            var result = StringParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(StringValue.Of("the quick brown bla, bla, bla"), result.Resolve());

            node = ParserUtil.ParseTokens("string-value", "\"the quick brown bla, bla, bla \\b \\f \\n \\r \\t \\\\ \\/ \\\" \\ua12c \"");
            result = StringParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(StringValue.Of("the quick brown bla, bla, bla \b \f \n \r \t \\ / \" ꄬ "), result.Resolve());
        }


        [TestMethod]
        public void GrammarTests()
        {
            var result = ParserUtil.ParseTokens("string-value", "\"all sorts of stuff\"");
            Assert.IsInstanceOfType<CSTNode>(result);
            var valueContent = result.FindNodes("value-content").FirstOrDefault();
            Assert.IsNotNull(valueContent);
            Assert.AreEqual("all sorts of stuff", valueContent.TokenValue());

            result = ParserUtil.ParseTokens("string-value", "\"all sorts of escaped stuff \\b \\f \\n \\r \\t \\\\ \\/ \\\" \\ua12c \"");
            Assert.IsInstanceOfType<CSTNode>(result);
            valueContent = result.FindNodes("value-content").FirstOrDefault();
            Assert.IsNotNull(valueContent);
            Assert.AreEqual("all sorts of escaped stuff \\b \\f \\n \\r \\t \\\\ \\/ \\\" \\ua12c ", valueContent.TokenValue());
        }
    }
}
