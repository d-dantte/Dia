namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class TimestampSerializer : ISerializer<Types.Timestamp>
    {
        private static readonly string YearPricision = "yyyy";
        private static readonly string MonthPricision = "yyyy-MM";
        private static readonly string DayPricision = "yyyy-MM-dd";
        private static readonly string HoursPricision = "yyyy-MM-dd HH";
        private static readonly string MinutesPricision = "yyyy-MM-dd HH:mm";
        private static readonly string SecondsPricision = "yyyy-MM-dd HH:mm:ss";
        private static readonly string TicksPricision = "yyyy-MM-dd HH:mm:ss.fffffff";
        private static readonly string TimeZone = "zzz";

        private static readonly string NullTypeText = $"*.{DiaType.Timestamp.ToString().ToLower()}";

        public static string Serialize(Types.Timestamp value, SerializerContext context)
        {
            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var precision = context.Options.Timestamps.Precision switch
            {
                Options.TimestampPrecision.Year => YearPricision,
                Options.TimestampPrecision.Month => MonthPricision,
                Options.TimestampPrecision.Day => DayPricision,
                Options.TimestampPrecision.Hours => HoursPricision,
                Options.TimestampPrecision.Minutes => MinutesPricision,
                Options.TimestampPrecision.Seconds => SecondsPricision,
                Options.TimestampPrecision.Ticks => TicksPricision,
                _ => throw new InvalidOperationException($"Invalid timestamp precision: {context.Options.Timestamps.Precision}")
            };

            precision = context.Options.Timestamps.IncludeTimezone
                ? $"{precision} {TimeZone}"
                : precision;

            return $"{attributeText}'#{DiaType.Timestamp} {value.Value!.Value.ToString(precision)}'";
        }
    }
}
