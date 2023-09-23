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
        [TestMethod]
        public void ParseTests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => BoolParser.Parse(null, new ParserContext()));
            Assert.ThrowsException<ArgumentException>(() => BoolParser.Parse(CSTNode.Of("stuff", "token"), default));

            var cstnode = ParserUtil.ParseTokens("bool-value", "true");
            var result = BoolParser.Parse(cstnode, new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            var value = result.Resolve();
            Assert.AreEqual(DiaType.Bool, value.Type);
            Assert.IsFalse(value.IsNull);
            Assert.AreEqual(true, value.As<BoolValue>().Value!);

            cstnode = ParserUtil.ParseTokens("bool-value", "false");
            result = BoolParser.Parse(cstnode, new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            value = result.Resolve();
            Assert.AreEqual(DiaType.Bool, value.Type);
            Assert.IsFalse(value.IsNull);
            Assert.AreEqual(false, value.As<BoolValue>().Value!);
        }

        [TestMethod]
        public void SerializeTests()
        {
            var @true = BoolValue.Of(true);
            var @false = BoolValue.Of(false);
            Assert.ThrowsException<ArgumentException>(() => BoolParser.Serialize(@true, default));

            var result = BoolParser.Serialize(BoolValue.Null(), new SerializerContext());
            Assert.IsTrue(result.IsErrorResult());

            result = BoolParser.Serialize(BoolValue.Of(true, "annotation"), new SerializerContext());
            Assert.IsTrue(result.IsErrorResult());

            result = BoolParser.Serialize(BoolValue.Null("annotation"), new SerializerContext());
            Assert.IsTrue(result.IsErrorResult());

            result = BoolParser.Serialize(BoolValue.Of(true), new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("true", result.Resolve());

            result = BoolParser.Serialize(BoolValue.Of(false), new SerializerContext());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("false", result.Resolve());
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
    }
}
