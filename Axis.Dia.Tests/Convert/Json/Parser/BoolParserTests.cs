using Axis.Dia.Contracts;
using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class BoolParserTests
    {
        private static BoolParser parser = new BoolParser();

        [TestMethod]
        public void ParseTests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => parser.Parse(null, new ParserContext()));
            Assert.ThrowsException<ArgumentNullException>(() => parser.Parse(CSTNode.Of("stuff", "token"), null));

            var cstnode = PulsarParse("bool-value", "true");
            var result = parser.Parse(cstnode, new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            var value = result.Resolve();
            Assert.AreEqual(DiaType.Bool, value.Type);
            Assert.IsFalse(value.IsNull);
            Assert.AreEqual(true, value.As<BoolValue>().Value!);

            cstnode = PulsarParse("bool-value", "false");
            result = parser.Parse(cstnode, new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            value = result.Resolve();
            Assert.AreEqual(DiaType.Bool, value.Type);
            Assert.IsFalse(value.IsNull);
            Assert.AreEqual(false, value.As<BoolValue>().Value!);

            //cstnode = PulsarParse("encoded-value", "\"[$Bool;]true\"");
            //result = parser.Parse(cstnode, new ParserContext());
            //Assert.IsTrue(result.IsDataResult());
            //value = result.Resolve();
            //Assert.AreEqual(DiaType.Bool, value.Type);
            //Assert.IsFalse(value.IsNull);
            //Assert.AreEqual(false, value.As<BoolValue>().Value!);
        }


        [TestMethod]
        public void GrammarTest()
        {
            var result = GrammarUtil.Grammar.GetRecognizer("bool-value").Recognize("true");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("bool-value").Recognize("false");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("bool-value").Recognize("FALsE");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("bool-value").Recognize("TrUe");
            Assert.IsTrue(result is SuccessResult);
        }

        private static CSTNode PulsarParse(string recognizer, string tokens)
        {
            var result = GrammarUtil.Grammar
                .GetRecognizer(recognizer)
                .Recognize(tokens);

            if (result is SuccessResult success)
                return success.Symbol;

            else throw new Pulsar.Grammar.Exceptions.RecognitionException(result);
        }
    }
}
