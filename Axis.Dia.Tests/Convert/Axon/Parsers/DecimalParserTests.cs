using Axis.Dia.Convert.Axon;
using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class DecimalParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var context = new SerializerContext();

            #region Null
            var decimalValue = DecimalValue.Null();
            var decimalValueAnnotated = DecimalValue.Null("stuff", "other");
            var text = DecimalParser.Serialize(decimalValue, context);
            var textAnnotated = DecimalParser.Serialize(decimalValueAnnotated, context);

            Assert.AreEqual("null.decimal", text.Resolve());
            Assert.AreEqual("stuff::other::null.decimal", textAnnotated.Resolve());
            #endregion

            #region value
            decimalValue = new DecimalValue(123456789.0009m);
            var ldecimalValue = new DecimalValue(0.000000098765m);
            var ndecimalValue = new DecimalValue(-123456789.0009m);
            decimalValueAnnotated = new DecimalValue(123456789.0009m, "stuff", "other");
            var ndecimalValueAnnotated = new DecimalValue(-123456789.0009m, "stuff", "other");

            // no exponent
            context.Options.Decimals.UseExponentNotation = false;
            text = DecimalParser.Serialize(decimalValue, context);
            var ltext = DecimalParser.Serialize(ldecimalValue, context);
            var ntext = DecimalParser.Serialize(ndecimalValue, context);
            textAnnotated = DecimalParser.Serialize(decimalValueAnnotated, context);
            var ntextAnnotated = DecimalParser.Serialize(ndecimalValueAnnotated, context);

            Assert.AreEqual("123456789.0009", text.Resolve());
            Assert.AreEqual("0.000000098765", ltext.Resolve());
            Assert.AreEqual("stuff::other::123456789.0009", textAnnotated.Resolve());
            Assert.AreEqual("-123456789.0009", ntext.Resolve());
            Assert.AreEqual("stuff::other::-123456789.0009", ntextAnnotated.Resolve());

            // exponent
            context.Options.Decimals.UseExponentNotation = true;
            ltext = DecimalParser.Serialize(ldecimalValue, context);
            text = DecimalParser.Serialize(decimalValue, context);
            ntext = DecimalParser.Serialize(ndecimalValue, context);
            textAnnotated = DecimalParser.Serialize(decimalValueAnnotated, context);
            ntextAnnotated = DecimalParser.Serialize(ndecimalValueAnnotated, context);

            Assert.AreEqual("1.234567890009E8", text.Resolve());
            Assert.AreEqual("9.8765E-8", ltext.Resolve());
            Assert.AreEqual("stuff::other::1.234567890009E8", textAnnotated.Resolve());
            Assert.AreEqual("-1.234567890009E8", ntext.Resolve());
            Assert.AreEqual("stuff::other::-1.234567890009E8", ntextAnnotated.Resolve());
            #endregion
        }

        [TestMethod]
        public void DeserializeTetss()
        {
            var nvalue = DecimalValue.Null();
            var value1 = new DecimalValue(1000);
            var value2 = new DecimalValue(-1000, "stuff", "$other_stuff");
            var value3 = new DecimalValue(0.000006557m, "stuff", "$other_stuff");
            var context = new ParserContext();
            var scontext = new SerializerContext();

            // no exponent
            scontext.Options.Decimals.UseExponentNotation = false;
            var ntext = DecimalParser.Serialize(nvalue, scontext);
            var text1 = DecimalParser.Serialize(value1, scontext);
            var text2 = DecimalParser.Serialize(value2, scontext);
            var text3 = DecimalParser.Serialize(value3, scontext);

            var nresult = AxonSerializer.ParseValue(ntext.Resolve());
            var result1 = AxonSerializer.ParseValue(text1.Resolve());
            var result2 = AxonSerializer.ParseValue(text2.Resolve());
            var result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(nvalue, nresult.Resolve());
            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value3, result3.Resolve());

            // exponent
            scontext.Options.Decimals.UseExponentNotation = true;
            ntext = DecimalParser.Serialize(nvalue, scontext);
            text1 = DecimalParser.Serialize(value1, scontext);
            text2 = DecimalParser.Serialize(value2, scontext);
            text3 = DecimalParser.Serialize(value3, scontext);

            nresult = AxonSerializer.ParseValue(ntext.Resolve());
            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());
            result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(nvalue, nresult.Resolve());
            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value3, result3.Resolve());
        }
    }
}
