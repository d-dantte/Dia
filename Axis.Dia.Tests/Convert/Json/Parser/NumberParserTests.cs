using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class NumberParserTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => NumberParser.Serialize(
                null,
                new SerializerContext()));

            Assert.ThrowsException<ArgumentException>(() => NumberParser.Serialize(
                IntValue.Of(8),
                default));

            var options = SerializerOptionsBuilder.NewBuilder().Build();
            var context = new SerializerContext(options);
            var result = NumberParser.Serialize(IntValue.Null(), context);
            Assert.IsTrue(result.IsErrorResult());

            result = NumberParser.Serialize(IntValue.Of(8, "stuff"), context);
            Assert.IsTrue(result.IsErrorResult());

            // values
            result = NumberParser.Serialize(IntValue.Of(123), context);
            Assert.AreEqual("123", result.Resolve());

            result = NumberParser.Serialize(IntValue.Of(-123), context);
            Assert.AreEqual("-123", result.Resolve());

            options = SerializerOptionsBuilder
                .FromOptions(options)
                .WithDecimalOptions(false, (ushort)options.Decimals.MaxPrecision)
                .Build();
            context = new SerializerContext(options);
            result = NumberParser.Serialize(DecimalValue.Of(123.0456m), context);
            Assert.AreEqual("123.0456", result.Resolve());

            result = NumberParser.Serialize(DecimalValue.Of(-123.0456m), context);
            Assert.AreEqual("-123.0456", result.Resolve());


            options = SerializerOptionsBuilder
                .FromOptions(options)
                .WithDecimalOptions(true, (ushort)options.Decimals.MaxPrecision)
                .Build();
            context = new SerializerContext(options);
            result = NumberParser.Serialize(DecimalValue.Of(0m), context);
            Assert.AreEqual("0.0E0", result.Resolve());

            result = NumberParser.Serialize(DecimalValue.Of(1200000m), context);
            Assert.AreEqual("1.2E6", result.Resolve());

            result = NumberParser.Serialize(DecimalValue.Of(-1200000m), context);
            Assert.AreEqual("-1.2E6", result.Resolve());

            result = NumberParser.Serialize(DecimalValue.Of(0.0000012m), context);
            Assert.AreEqual("1.2E-6", result.Resolve());

            result = NumberParser.Serialize(DecimalValue.Of(-0.0000012m), context);
            Assert.AreEqual("-1.2E-6", result.Resolve());
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => NumberParser.Parse(
                null,
                new ParserContext()));

            Assert.ThrowsException<ArgumentException>(() => NumberParser.Parse(
                CSTNode.Of("dummy", "stuff"),
                default));

            Assert.ThrowsException<ArgumentException>(() => NumberParser.Parse(
                CSTNode.Of("dummy", "stuff"),
                new ParserContext()));

            var context = new ParserContext();

            // values
            var node = ParserUtil.ParseTokens("number-value", "123");
            var result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(IntValue.Of(123), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "-123");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(IntValue.Of(-123), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "123.0456");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(123.0456m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "-123.0456");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(-123.0456m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "0.0");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(0m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "0E0");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(0m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "0.0E0");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(0m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "00.00E00");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(0m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "-12.0E005");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(-1200000m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "-12.0E+005");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(-1200000m), result.Resolve());

            node = ParserUtil.ParseTokens("number-value", "-12.0E-005");
            result = NumberParser.Parse(node, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DecimalValue.Of(-0.00012m), result.Resolve());
        }


        [TestMethod]
        public void GrammarTests()
        {
            var result = ParserUtil.ParseTokens("number-value", "123");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("int-number").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "-123");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("int-number").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "123.0456");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("regular-decimal").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "-123.0456");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("regular-decimal").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "0.0");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("regular-decimal").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "0E0");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("scientific-decimal").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "00.0E0");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("scientific-decimal").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "-12.0E005");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("scientific-decimal").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "-12.0E+005");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("scientific-decimal").FirstOrDefault());

            result = ParserUtil.ParseTokens("number-value", "-12.0E-005");
            Assert.IsInstanceOfType<CSTNode>(result);
            Assert.IsNotNull(result.FindNodes("scientific-decimal").FirstOrDefault());
        }
    }
}
