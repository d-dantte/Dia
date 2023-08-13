using Axis.Dia.Convert.Text;
using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Text.Parsers
{
    [TestClass]
    public class DecimalParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var context = new TextSerializerContext();

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
            var context = new TextSerializerContext();

            // no exponent
            context.Options.Decimals.UseExponentNotation = false;
            var ntext = DecimalParser.Serialize(nvalue, context);
            var text1 = DecimalParser.Serialize(value1, context);
            var text2 = DecimalParser.Serialize(value2, context);
            var text3 = DecimalParser.Serialize(value3, context);

            var nresult = TextSerializer.ParseValue(ntext.Resolve(), context);
            var result1 = TextSerializer.ParseValue(text1.Resolve(), context);
            var result2 = TextSerializer.ParseValue(text2.Resolve(), context);
            var result3 = TextSerializer.ParseValue(text3.Resolve(), context);

            Assert.AreEqual(nvalue, nresult.Resolve());
            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value3, result3.Resolve());

            // exponent
            context.Options.Decimals.UseExponentNotation = true;
            ntext = DecimalParser.Serialize(nvalue, context);
            text1 = DecimalParser.Serialize(value1, context);
            text2 = DecimalParser.Serialize(value2, context);
            text3 = DecimalParser.Serialize(value3, context);

            nresult = TextSerializer.ParseValue(ntext.Resolve(), context);
            result1 = TextSerializer.ParseValue(text1.Resolve(), context);
            result2 = TextSerializer.ParseValue(text2.Resolve(), context);
            result3 = TextSerializer.ParseValue(text3.Resolve(), context);

            Assert.AreEqual(nvalue, nresult.Resolve());
            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value3, result3.Resolve());
        }
    }
}
