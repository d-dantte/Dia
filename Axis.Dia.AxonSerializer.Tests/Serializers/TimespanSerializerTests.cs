using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core.Types;

namespace Axis.Dia.AxonSerializer.Tests.Serializers
{
    [TestClass]
    public class TimespanSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid context: default")]
        public void Serialize_InvalidContext_ThrowsException()
        {
            TimestampSerializer.Serialize(Timestamp.Null(), default);
        }

        [TestMethod]
        public void Serialize_NullValue_ReturnsNullTypeText()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Timestamp.Null();

            var result = TimestampSerializer.Serialize(value, context);

            Assert.AreEqual("null.timestamp", result);
        }

        [TestMethod]
        public void Serialize_TimestampWithAttributes_ReturnsSerializedStringWithAttributes()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp, "flag");

            var result = TimestampSerializer.Serialize(value, context);
            var expectedTimestampText = $"'Timestamp {expectedTimestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz")}'";

            Assert.AreEqual($"@flag; {expectedTimestampText}", result);
        }

        [TestMethod]
        public void Serialize_TimestampWithYearPrecision_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Year)
                .WithTimestampTimezone(false)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy}'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_TimestampWithMonthPrecision_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Month)
                .WithTimestampTimezone(false)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy-MM}'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_TimestampWithDayPrecision_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Day)
                .WithTimestampTimezone(false)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy-MM-dd}'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_TimestampWithHourPrecision_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Hours)
                .WithTimestampTimezone(false)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy-MM-dd HH}'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_TimestampWithMinutesPrecision_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Minutes)
                .WithTimestampTimezone(false)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy-MM-dd HH:mm}'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_TimestampWithSecondsPrecision_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Seconds)
                .WithTimestampTimezone(false)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy-MM-dd HH:mm:ss}'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_TimestampWithTicksPrecision_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Ticks)
                .WithTimestampTimezone(false)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy-MM-dd HH:mm:ss.fffffff}'";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_TimestampWithHourPrecisionAndTimeZone_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Hours)
                .WithTimestampTimezone(true)
                .Build();
            var context = SerializerContext.Of(options);
            var expectedTimestamp = DateTimeOffset.Now;
            var value = Timestamp.Of(expectedTimestamp);

            var result = TimestampSerializer.Serialize(value, context);
            var expected = $"'Timestamp {expectedTimestamp:yyyy-MM-dd HH zzz}'";

            Assert.AreEqual(expected, result);
        }
    }
}
