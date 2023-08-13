using Axis.Dia.Convert.Text;
using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Text.Parsers
{
    [TestClass]
    public class IntParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var context = new TextSerializerContext();

            #region Null
            var @int = IntValue.Null();
            var annotatedInt = IntValue.Null("stuff", "other");
            var text = IntParser.Serialize(@int, context);
            var annotatedText = IntParser.Serialize(annotatedInt, context);

            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("null.int", text.Resolve());

            Assert.IsTrue(annotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::null.int", annotatedText.Resolve());
            #endregion

            #region Decimal
            @int = new IntValue(1234567890);
            var nint = new IntValue(-1234567890);
            annotatedInt = IntValue.Of(1234567890, "stuff", "other");
            var nannotatedInt = IntValue.Of(-1234567890, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.Decimal;
            text = IntParser.Serialize(@int, context);
            var ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            var nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("1234567890", text.Resolve());
            Assert.IsTrue(annotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::1234567890", annotatedText.Resolve());
            Assert.IsTrue(ntext.IsDataResult());
            Assert.AreEqual("-1234567890", ntext.Resolve());
            Assert.IsTrue(nannotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::-1234567890", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("1_234_567_890", text.Resolve());
            Assert.IsTrue(annotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::1_234_567_890", annotatedText.Resolve());
            Assert.IsTrue(ntext.IsDataResult());
            Assert.AreEqual("-1_234_567_890", ntext.Resolve());
            Assert.IsTrue(nannotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::-1_234_567_890", nannotatedText.Resolve());
            #endregion

            #region BigHex
            @int = new IntValue(1234567890);
            nint = new IntValue(-1234567890);
            annotatedInt = new IntValue(1234567890, "stuff", "other");
            nannotatedInt = new IntValue(-1234567890, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.BigHex;
            context.Options.Ints.UseDigitSeparator = false;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("0X499602D2", text.Resolve());
            Assert.IsTrue(annotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::0X499602D2", annotatedText.Resolve());
            Assert.IsTrue(ntext.IsDataResult());
            Assert.AreEqual("0XB669FD2E", ntext.Resolve());
            Assert.IsTrue(nannotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::0XB669FD2E", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("0X4996_02D2", text.Resolve());
            Assert.IsTrue(annotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::0X4996_02D2", annotatedText.Resolve());
            Assert.IsTrue(ntext.IsDataResult());
            Assert.AreEqual("0XB669_FD2E", ntext.Resolve());
            Assert.IsTrue(nannotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::0XB669_FD2E", nannotatedText.Resolve());
            #endregion

            #region SmallHex
            @int = new IntValue(1234567890);
            nint = new IntValue(-1234567890);
            annotatedInt = new IntValue(1234567890, "stuff", "other");
            nannotatedInt = new IntValue(-1234567890, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.SmallHex;
            context.Options.Ints.UseDigitSeparator = false;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0x499602d2", text.Resolve());
            Assert.AreEqual("stuff::other::0x499602d2", annotatedText.Resolve());
            Assert.AreEqual("0xb669fd2e", ntext.Resolve());
            Assert.AreEqual("stuff::other::0xb669fd2e", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0x4996_02d2", text.Resolve());
            Assert.AreEqual("stuff::other::0x4996_02d2", annotatedText.Resolve());
            Assert.AreEqual("0xb669_fd2e", ntext.Resolve());
            Assert.AreEqual("stuff::other::0xb669_fd2e", nannotatedText.Resolve());
            #endregion

            #region BigBinary
            @int = new IntValue(100);
            nint = new IntValue(-100);
            annotatedInt = new IntValue(100, "stuff", "other");
            nannotatedInt = new IntValue(-100, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.BigBinary;
            context.Options.Ints.UseDigitSeparator = false;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0B1100100", text.Resolve());
            Assert.AreEqual("stuff::other::0B1100100", annotatedText.Resolve());
            Assert.AreEqual("0B10011100", ntext.Resolve());
            Assert.AreEqual("stuff::other::0B10011100", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0B110_0100", text.Resolve());
            Assert.AreEqual("stuff::other::0B110_0100", annotatedText.Resolve());
            Assert.AreEqual("0B1001_1100", ntext.Resolve());
            Assert.AreEqual("stuff::other::0B1001_1100", nannotatedText.Resolve());
            #endregion

            #region SmallBinary
            @int = new IntValue(100);
            nint = new IntValue(-100);
            annotatedInt = new IntValue(100, "stuff", "other");
            nannotatedInt = new IntValue(-100, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.SmallBinary;
            context.Options.Ints.UseDigitSeparator = false;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0b1100100", text.Resolve());
            Assert.AreEqual("stuff::other::0b1100100", annotatedText.Resolve());
            Assert.AreEqual("0b10011100", ntext.Resolve());
            Assert.AreEqual("stuff::other::0b10011100", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0b110_0100", text.Resolve());
            Assert.AreEqual("stuff::other::0b110_0100", annotatedText.Resolve());
            Assert.AreEqual("0b1001_1100", ntext.Resolve());
            Assert.AreEqual("stuff::other::0b1001_1100", nannotatedText.Resolve());
            #endregion
        }

        [TestMethod]
        public void DeserializeTests()
        {
            var nvalue = IntValue.Null();
            var value1 = new IntValue(1000);
            var value2 = new IntValue(-1000, "stuff", "$other_stuff");
            var context = new TextSerializerContext();

            #region decimal
            context.Options.Ints.UseDigitSeparator = false;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.Decimal;
            var ntext = IntParser.Serialize(nvalue, context);
            var text1 = IntParser.Serialize(value1, context);
            var text2 = IntParser.Serialize(value2, context);
            var nresult = TextSerializer.ParseValue(ntext.Resolve());
            var result1 = TextSerializer.ParseValue(text1.Resolve());
            var result2 = TextSerializer.ParseValue(text2.Resolve());

            context.Options.Ints.UseDigitSeparator = true;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.Decimal;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            var result3 = TextSerializer.ParseValue(text1.Resolve());
            var result4 = TextSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(nvalue, nresult.Resolve());
            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region big hex
            context.Options.Ints.UseDigitSeparator = false;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.BigHex;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result1 = TextSerializer.ParseValue(text1.Resolve());
            result2 = TextSerializer.ParseValue(text2.Resolve());

            context.Options.Ints.UseDigitSeparator = true;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.BigHex;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result3 = TextSerializer.ParseValue(text1.Resolve());
            result4 = TextSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region small hex
            context.Options.Ints.UseDigitSeparator = false;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.SmallHex;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result1 = TextSerializer.ParseValue(text1.Resolve());
            result2 = TextSerializer.ParseValue(text2.Resolve());

            context.Options.Ints.UseDigitSeparator = true;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.SmallHex;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result3 = TextSerializer.ParseValue(text1.Resolve());
            result4 = TextSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region big binary
            context.Options.Ints.UseDigitSeparator = false;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.BigBinary;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result1 = TextSerializer.ParseValue(text1.Resolve());
            result2 = TextSerializer.ParseValue(text2.Resolve());

            context.Options.Ints.UseDigitSeparator = true;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.BigBinary;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result3 = TextSerializer.ParseValue(text1.Resolve());
            result4 = TextSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region small binary
            context.Options.Ints.UseDigitSeparator = false;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.SmallBinary;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result1 = TextSerializer.ParseValue(text1.Resolve());
            result2 = TextSerializer.ParseValue(text2.Resolve());

            context.Options.Ints.UseDigitSeparator = true;
            context.Options.Ints.NumberFormat = TextSerializerOptions.IntFormat.SmallBinary;
            text1 = IntParser.Serialize(value1, context);
            text2 = IntParser.Serialize(value2, context);
            result3 = TextSerializer.ParseValue(text1.Resolve());
            result4 = TextSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion
        }
    }
}
