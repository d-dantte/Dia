using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class TimestampSerializerTest
    {
        private static readonly string YearPrecision = "yyyy";
        private static readonly string MonthPrecision = "yyyy-MM";
        private static readonly string DayPrecision = "yyyy-MM-dd";
        private static readonly string HoursPrecision = "yyyy-MM-dd HH";
        private static readonly string MinutesPrecision = "yyyy-MM-dd HH:mm";
        private static readonly string SecondsPrecision = "yyyy-MM-dd HH:mm:ss";
        private static readonly string TicksPrecision = "yyyy-MM-dd HH:mm:ss.fffffff";
        private static readonly string TimeZone = "zzz";

        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => TimestampSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_Timestamp()
        {
            var datetime = DateTimeOffset.Now;
            var timestamp = Core.Types.Timestamp.Of(datetime);
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // Null
            var text = TimestampSerializer.Serialize(default, context);
            Assert.AreEqual("*.timestamp", text);
            text = TimestampSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.timestamp", text);

            // timezone precision
            canonicalContext = Options
                .Builder()
                .WithTimestampTimezone(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual(Options.TimestampPrecision.Ticks, context.Options.Timestamps.Precision);
            var tzPrecision = $"{TicksPrecision} {TimeZone}";
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(tzPrecision)}'", text);

            // tick precision
            canonicalContext = Options
                .Builder()
                .WithTimestampTimezone(false)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual(Options.TimestampPrecision.Ticks, context.Options.Timestamps.Precision);
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(TicksPrecision)}'", text);

            // seconds precision
            canonicalContext = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Seconds)
                .WithTimestampTimezone(false)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(SecondsPrecision)}'", text);

            // minutes precision
            canonicalContext = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Minutes)
                .WithTimestampTimezone(false)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(MinutesPrecision)}'", text);

            // hours precision
            tzPrecision = $"{HoursPrecision} {TimeZone}";
            canonicalContext = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Hours)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(tzPrecision)}'", text);

            // days precision
            canonicalContext = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Day)
                .WithTimestampTimezone(false)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(DayPrecision)}'", text);

            // month precision
            canonicalContext = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Month)
                .WithTimestampTimezone(false)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(MonthPrecision)}'", text);

            // year precision
            canonicalContext = Options
                .Builder()
                .WithTimestampPrecision(Options.TimestampPrecision.Year)
                .WithTimestampTimezone(false)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            text = TimestampSerializer.Serialize(timestamp, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Timestamp} {datetime.ToString(YearPrecision)}'", text);
        }
    }
}
