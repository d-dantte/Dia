using Axis.Dia.Convert.Axon;
using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class IntParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var context = new SerializerContext();

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
            context.Options.Ints.NumberFormat = SerializerOptions.IntFormat.Decimal;
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
            context.Options.Ints.NumberFormat = SerializerOptions.IntFormat.BigHex;
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
            Assert.AreEqual("-0X499602D2", ntext.Resolve());
            Assert.IsTrue(nannotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::-0X499602D2", nannotatedText.Resolve());

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
            Assert.AreEqual("-0X4996_02D2", ntext.Resolve());
            Assert.IsTrue(nannotatedText.IsDataResult());
            Assert.AreEqual("stuff::other::-0X4996_02D2", nannotatedText.Resolve());
            #endregion

            #region SmallHex
            @int = new IntValue(1234567890);
            nint = new IntValue(-1234567890);
            annotatedInt = new IntValue(1234567890, "stuff", "other");
            nannotatedInt = new IntValue(-1234567890, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = SerializerOptions.IntFormat.SmallHex;
            context.Options.Ints.UseDigitSeparator = false;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0x499602d2", text.Resolve());
            Assert.AreEqual("stuff::other::0x499602d2", annotatedText.Resolve());
            Assert.AreEqual("-0x499602d2", ntext.Resolve());
            Assert.AreEqual("stuff::other::-0x499602d2", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0x4996_02d2", text.Resolve());
            Assert.AreEqual("stuff::other::0x4996_02d2", annotatedText.Resolve());
            Assert.AreEqual("-0x4996_02d2", ntext.Resolve());
            Assert.AreEqual("stuff::other::-0x4996_02d2", nannotatedText.Resolve());
            #endregion

            #region BigBinary
            @int = new IntValue(100);
            nint = new IntValue(-100);
            annotatedInt = new IntValue(100, "stuff", "other");
            nannotatedInt = new IntValue(-100, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = SerializerOptions.IntFormat.BigBinary;
            context.Options.Ints.UseDigitSeparator = false;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0B1100100", text.Resolve());
            Assert.AreEqual("stuff::other::0B1100100", annotatedText.Resolve());
            Assert.AreEqual("-0B1100100", ntext.Resolve());
            Assert.AreEqual("stuff::other::-0B1100100", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0B110_0100", text.Resolve());
            Assert.AreEqual("stuff::other::0B110_0100", annotatedText.Resolve());
            Assert.AreEqual("-0B110_0100", ntext.Resolve());
            Assert.AreEqual("stuff::other::-0B110_0100", nannotatedText.Resolve());
            #endregion

            #region SmallBinary
            @int = new IntValue(100);
            nint = new IntValue(-100);
            annotatedInt = new IntValue(100, "stuff", "other");
            nannotatedInt = new IntValue(-100, "stuff", "other");

            // no separator
            context.Options.Ints.NumberFormat = SerializerOptions.IntFormat.SmallBinary;
            context.Options.Ints.UseDigitSeparator = false;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0b1100100", text.Resolve());
            Assert.AreEqual("stuff::other::0b1100100", annotatedText.Resolve());
            Assert.AreEqual("-0b1100100", ntext.Resolve());
            Assert.AreEqual("stuff::other::-0b1100100", nannotatedText.Resolve());

            // separator
            context.Options.Ints.UseDigitSeparator = true;
            text = IntParser.Serialize(@int, context);
            ntext = IntParser.Serialize(nint, context);
            annotatedText = IntParser.Serialize(annotatedInt, context);
            nannotatedText = IntParser.Serialize(nannotatedInt, context);

            Assert.AreEqual("0b110_0100", text.Resolve());
            Assert.AreEqual("stuff::other::0b110_0100", annotatedText.Resolve());
            Assert.AreEqual("-0b110_0100", ntext.Resolve());
            Assert.AreEqual("stuff::other::-0b110_0100", nannotatedText.Resolve());
            #endregion
        }

        [TestMethod]
        public void DeserializeTests()
        {
            var nvalue = IntValue.Null();
            var value1 = new IntValue(1000);
            var value2 = new IntValue(-1000, "stuff", "$other_stuff");
            var scontext = new SerializerContext();

            #region decimal
            scontext.Options.Ints.UseDigitSeparator = false;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.Decimal;
            var ntext = IntParser.Serialize(nvalue, scontext);
            var text1 = IntParser.Serialize(value1, scontext);
            var text2 = IntParser.Serialize(value2, scontext);
            var nresult = AxonSerializer.ParseValue(ntext.Resolve());
            var result1 = AxonSerializer.ParseValue(text1.Resolve());
            var result2 = AxonSerializer.ParseValue(text2.Resolve());

            scontext.Options.Ints.UseDigitSeparator = true;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.Decimal;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            var result3 = AxonSerializer.ParseValue(text1.Resolve());
            var result4 = AxonSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(nvalue, nresult.Resolve());
            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region big hex
            scontext.Options.Ints.UseDigitSeparator = false;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.BigHex;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());

            scontext.Options.Ints.UseDigitSeparator = true;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.BigHex;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result3 = AxonSerializer.ParseValue(text1.Resolve());
            result4 = AxonSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region small hex
            scontext.Options.Ints.UseDigitSeparator = false;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.SmallHex;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());

            scontext.Options.Ints.UseDigitSeparator = true;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.SmallHex;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result3 = AxonSerializer.ParseValue(text1.Resolve());
            result4 = AxonSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region big binary
            scontext.Options.Ints.UseDigitSeparator = false;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.BigBinary;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());

            scontext.Options.Ints.UseDigitSeparator = true;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.BigBinary;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result3 = AxonSerializer.ParseValue(text1.Resolve());
            result4 = AxonSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion

            #region small binary
            scontext.Options.Ints.UseDigitSeparator = false;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.SmallBinary;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());

            scontext.Options.Ints.UseDigitSeparator = true;
            scontext.Options.Ints.NumberFormat = SerializerOptions.IntFormat.SmallBinary;
            text1 = IntParser.Serialize(value1, scontext);
            text2 = IntParser.Serialize(value2, scontext);
            result3 = AxonSerializer.ParseValue(text1.Resolve());
            result4 = AxonSerializer.ParseValue(text2.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(value2, result2.Resolve());
            Assert.AreEqual(value1, result3.Resolve());
            Assert.AreEqual(value2, result4.Resolve());
            #endregion
        }
    }
}
