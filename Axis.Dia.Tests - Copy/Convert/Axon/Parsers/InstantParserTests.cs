using Axis.Dia.Convert.Axon;
using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class InstantParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var context = new SerializerContext();

            #region Null
            var ionTimestamp = InstantValue.Null();
            var ionTimestampAnnotated = InstantValue.Null("stuff", "other");
            var text = InstantParser.Serialize(ionTimestamp, context);
            var textAnnotated = InstantParser.Serialize(ionTimestampAnnotated, context);

            Assert.AreEqual("null.instant", text.Resolve());
            Assert.AreEqual("stuff::other::null.instant", textAnnotated.Resolve());
            #endregion

            var timestamp = DateTimeOffset.Parse("2/2/2023 9:37:54 AM+05:00") + TimeSpan.FromMilliseconds(0.123456);
            ionTimestamp = new InstantValue(timestamp);
            ionTimestampAnnotated = new InstantValue(timestamp, "stuff", "eurt");

            #region Year precision
            context.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Year;
            text = InstantParser.Serialize(ionTimestamp, context);
            textAnnotated = InstantParser.Serialize(ionTimestampAnnotated, context);
            Assert.AreEqual("2023T", text.Resolve());
            Assert.AreEqual("stuff::eurt::2023T", textAnnotated.Resolve());
            #endregion

            #region Month precision
            context.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Month;
            text = InstantParser.Serialize(ionTimestamp, context);
            textAnnotated = InstantParser.Serialize(ionTimestampAnnotated, context);
            Assert.AreEqual("2023-02T", text.Resolve());
            Assert.AreEqual("stuff::eurt::2023-02T", textAnnotated.Resolve());
            #endregion

            #region Day precision
            context.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Day;
            text = InstantParser.Serialize(ionTimestamp, context);
            textAnnotated = InstantParser.Serialize(ionTimestampAnnotated, context);
            Assert.AreEqual("2023-02-02", text.Resolve());
            Assert.AreEqual("stuff::eurt::2023-02-02", textAnnotated.Resolve());
            #endregion

            #region Minute precision
            context.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Minute;
            text = InstantParser.Serialize(ionTimestamp, context);
            textAnnotated = InstantParser.Serialize(ionTimestampAnnotated, context);
            Assert.AreEqual("2023-02-02T09:37+05:00", text.Resolve());
            Assert.AreEqual("stuff::eurt::2023-02-02T09:37+05:00", textAnnotated.Resolve());
            #endregion

            #region Second precision
            context.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Second;
            text = InstantParser.Serialize(ionTimestamp, context);
            textAnnotated = InstantParser.Serialize(ionTimestampAnnotated, context);
            Assert.AreEqual("2023-02-02T09:37:54+05:00", text.Resolve());
            Assert.AreEqual("stuff::eurt::2023-02-02T09:37:54+05:00", textAnnotated.Resolve());
            #endregion

            #region Millisecond precision
            context.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.MilliSecond;
            text = InstantParser.Serialize(ionTimestamp, context);
            textAnnotated = InstantParser.Serialize(ionTimestampAnnotated, context);
            Assert.AreEqual("2023-02-02T09:37:54.0001234+05:00", text.Resolve());
            Assert.AreEqual("stuff::eurt::2023-02-02T09:37:54.0001234+05:00", textAnnotated.Resolve());
            #endregion
        }

        [TestMethod]
        public void DeserializerTests()
        {
            var timestamp = DateTimeOffset.Parse("2/2/2023 9:37:54 AM+05:00") + TimeSpan.FromMilliseconds(0.123456);
            var value1 = InstantValue.Null();
            var value2 = new InstantValue(timestamp);
            var value3 = new InstantValue(timestamp, "stuff", "$other_stuff");
            var scontext = new SerializerContext();
            var pcontext = new ParserContext();
            var localOffset = TimeZoneInfo.Local.BaseUtcOffset;

            #region Year precision
            scontext.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Year;
            var text1 = InstantParser.Serialize(value1, scontext);
            var text2 = InstantParser.Serialize(value2, scontext);
            var text3 = InstantParser.Serialize(value3, scontext);

            var result1 = AxonSerializer.ParseValue(text1.Resolve());
            var result2 = AxonSerializer.ParseValue(text2.Resolve());
            var result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(
                value2
                    .Copy(InstantValue.Precision.Year)
                    .SwitchOffset(localOffset),
                result2.Resolve());
            Assert.AreEqual(
                value3
                .Copy(InstantValue.Precision.Year)
                .SwitchOffset(localOffset),
                result3.Resolve());
            #endregion

            #region Month precision
            scontext.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Month;
            text1 = InstantParser.Serialize(value1, scontext);
            text2 = InstantParser.Serialize(value2, scontext);
            text3 = InstantParser.Serialize(value3, scontext);

            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());
            result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(
                value2
                    .Copy(InstantValue.Precision.Month)
                    .SwitchOffset(localOffset),
                result2.Resolve());
            Assert.AreEqual(
                value3
                .Copy(InstantValue.Precision.Month)
                .SwitchOffset(localOffset),
                result3.Resolve());
            #endregion

            #region Day precision
            scontext.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Day;
            text1 = InstantParser.Serialize(value1, scontext);
            text2 = InstantParser.Serialize(value2, scontext);
            text3 = InstantParser.Serialize(value3, scontext);

            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());
            result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(
                value2
                    .Copy(InstantValue.Precision.Day)
                    .SwitchOffset(localOffset),
                result2.Resolve());
            Assert.AreEqual(
                value3
                .Copy(InstantValue.Precision.Day)
                .SwitchOffset(localOffset),
                result3.Resolve());
            #endregion

            #region Minute precision
            scontext.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Minute;
            text1 = InstantParser.Serialize(value1, scontext);
            text2 = InstantParser.Serialize(value2, scontext);
            text3 = InstantParser.Serialize(value3, scontext);

            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());
            result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(
                value2
                    .Copy(InstantValue.Precision.Minute),
                result2.Resolve());
            Assert.AreEqual(
                value3
                .Copy(InstantValue.Precision.Minute),
                result3.Resolve());
            #endregion

            #region Second precision
            scontext.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.Second;
            text1 = InstantParser.Serialize(value1, scontext);
            text2 = InstantParser.Serialize(value2, scontext);
            text3 = InstantParser.Serialize(value3, scontext);

            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());
            result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(
                value2
                    .Copy(InstantValue.Precision.Second),
                result2.Resolve());
            Assert.AreEqual(
                value3
                .Copy(InstantValue.Precision.Second),
                result3.Resolve());
            #endregion

            #region Millisecond precision
            scontext.Options.Timestamps.TimestampPrecision = SerializerOptions.TimestampPrecision.MilliSecond;
            text1 = InstantParser.Serialize(value1, scontext);
            text2 = InstantParser.Serialize(value2, scontext);
            text3 = InstantParser.Serialize(value3, scontext);

            result1 = AxonSerializer.ParseValue(text1.Resolve());
            result2 = AxonSerializer.ParseValue(text2.Resolve());
            result3 = AxonSerializer.ParseValue(text3.Resolve());

            Assert.AreEqual(value1, result1.Resolve());
            Assert.AreEqual(
                value2.Copy(InstantValue.Precision.Millisecond),
                result2.Resolve());
            Assert.AreEqual(
                value3
                .Copy(InstantValue.Precision.Millisecond),
                result3.Resolve());
            #endregion

            #region Now
            var valueNow = InstantValue.Of(DateTimeOffset.Now);
            var textNow = InstantParser.Serialize(valueNow, scontext);
            var resultNow = AxonSerializer.ParseValue(textNow.Resolve());
            var rvalueNow = resultNow.Resolve();
            Assert.AreEqual(valueNow, rvalueNow);
            #endregion
        }
    }
}
