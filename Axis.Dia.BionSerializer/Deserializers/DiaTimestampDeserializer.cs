using Axis.Dia.Bion.Deserializers.Contracts;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Utils;
using Axis.Dia.Core;
using Axis.Luna.BitSequence;
using Axis.Luna.Extensions;

namespace Axis.Dia.Bion.Deserializers
{
    using DiaTimestamp = Core.Types.Timestamp;

    public class DiaTimestampDeserializer :
        ITypeDeserializer<DiaTimestamp>,
        IDefaultInstance<DiaTimestampDeserializer>
    {
        public static DiaTimestampDeserializer DefaultInstance { get; } = new();

        public DiaTimestamp DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Timestamp.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Timestamp}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return DiaTimestamp.Null(attributes!);

            else if (!typeMetadata.IsCustomFlagSet)
                return DiaTimestamp.Of(DateTimeOffset.MinValue, attributes);

            var ztdmComponents = stream
                .ReadExactly(9)
                .ApplyTo(BitSequence.Of)
                .ApplyTo(bitSeq => (
                    TimeZoneMinutes: bitSeq[..11].ToBigInteger(),
                    TimeTicks: bitSeq[11..58].ToBigInteger() / TimeSpan.NanosecondsPerTick,
                    DayOfMonth: bitSeq[58..63].ToBigInteger(),
                    MonthOfYear: bitSeq[63..].ToBigInteger()));

            var yearComponent = stream
                .ReadVarBytes()
                .ApplyTo(varBytes => varBytes.ToBigInteger());

            var dateOnly = new DateOnly(
                (int)yearComponent,
                (int)ztdmComponents.MonthOfYear,
                (int)ztdmComponents.DayOfMonth);
            var timeOnly = new TimeOnly((long)ztdmComponents.TimeTicks);
            var timeZone = TimeSpan.FromMinutes((int)ztdmComponents.TimeZoneMinutes);

            return DiaTimestamp.Of(
                new DateTimeOffset(dateOnly.ToDateTime(timeOnly), timeZone),
                attributes);
        }
    }
}
