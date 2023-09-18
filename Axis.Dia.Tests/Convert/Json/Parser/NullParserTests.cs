using Axis.Dia.Contracts;
using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class NullParserTests
    {
        private static NullParser parser = new NullParser();

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => parser.Parse(null, new ParserContext()));
            Assert.ThrowsException<ArgumentNullException>(() => parser.Parse(CSTNode.Of("stuff", "token"), null));

            var result = parser.Parse(CSTNode.Of("null", "Null"), new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            var value = result.Resolve();
            Assert.AreEqual(DiaType.Record, value.Type);
            Assert.IsTrue(value.IsNull);
        }

        [TestMethod]
        public void NullGrammarTest()
        {
            var result = GrammarUtil.Grammar.GetRecognizer("null").Recognize("null");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("null").Recognize("Null");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("null").Recognize("NULL");
            Assert.IsTrue(result is SuccessResult);
        }
    }
}
