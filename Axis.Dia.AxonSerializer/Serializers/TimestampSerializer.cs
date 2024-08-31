using Axis.Dia.Core;

namespace Axis.Dia.Axon.Serializers
{
    public class TimestampSerializer : IValueSerializer<Core.Types.Timestamp>
    {
        private static readonly string YearPrecision = "yyyy";
        private static readonly string MonthPrecision = "yyyy-MM";
        private static readonly string DayPrecision = "yyyy-MM-dd";
        private static readonly string HoursPrecision = "yyyy-MM-dd HH";
        private static readonly string MinutesPrecision = "yyyy-MM-dd HH:mm";
        private static readonly string SecondsPrecision = "yyyy-MM-dd HH:mm:ss";
        private static readonly string TicksPrecision = "yyyy-MM-dd HH:mm:ss.fffffff";
        private static readonly string TimeZone = "zzz";

        private static readonly string NullTypeText = $"null.{DiaType.Timestamp.ToString().ToLower()}";

        public static string Serialize(Core.Types.Timestamp value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var precision = context.Options.Timestamps.Precision switch
            {
                Options.TimestampPrecision.Year => YearPrecision,
                Options.TimestampPrecision.Month => MonthPrecision,
                Options.TimestampPrecision.Day => DayPrecision,
                Options.TimestampPrecision.Hours => HoursPrecision,
                Options.TimestampPrecision.Minutes => MinutesPrecision,
                Options.TimestampPrecision.Seconds => SecondsPrecision,
                _ => TicksPrecision
            };

            precision = context.Options.Timestamps.IncludeTimezone
                ? $"{precision} {TimeZone}"
                : precision;

            return $"{attributeText}'{DiaType.Timestamp} {value.Value!.Value.ToString(precision)}'";
        }
    }
}
