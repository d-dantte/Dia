using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.BitSequence;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class DiaTimestampSerializer :
        ITypeSerializer<Core.Types.Timestamp>,
        IMetadataProvider<Core.Types.Timestamp>,
        IDefaultInstance<DiaTimestampSerializer>
    {
        private const ushort MaxTimeZoneMinutes = 60 * 24;

        public static DiaTimestampSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Timestamp value)
        {
            return TypeMetadata
                .Of(DiaType.Timestamp)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull);
        }

        public void SerializeType(Core.Types.Timestamp value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write attributes
            context.TypeSerializer.SerializeAttributeSet(value.Attributes, context);

            // Write data
            if (value.IsNull)
                return;

            var timestamp = value.Value!.Value;

            // Timezone component (11 bits)
            var tzbits = timestamp.Offset.TotalMinutes
                .ThrowIf(
                    minutes => minutes > MaxTimeZoneMinutes,
                    minutes => throw new InvalidOperationException($"Invalid time-zone minutes: {minutes}"))
                .ApplyTo(minutes => (ushort)minutes)
                .ApplyTo(BitConverter.GetBytes)
                .ApplyTo(BitSequence.Of)
                .ApplyTo(seq => seq[..11]);

            // Time component (47 bits)
            var timebits = timestamp
                .ApplyTo(ts => ts - new DateTimeOffset(ts.Year, ts.Month, ts.Day, 0, 0, 0, 0, 0, ts.Offset))
                .ApplyTo(ts => (long)ts.TotalNanoseconds)
                .ApplyTo(BitConverter.GetBytes)
                .ApplyTo(BitSequence.Of)
                .ApplyTo(seq => seq[..47]);

            // Day component (5 bits)
            var daybits = timestamp.Day
                .ApplyTo(BitConverter.GetBytes)
                .ApplyTo(BitSequence.Of)
                .ApplyTo(seq => seq[..5]);

            // Month component (4 bits)
            var monthbits = timestamp.Month
                .ApplyTo(BitConverter.GetBytes)
                .ApplyTo(BitSequence.Of)
                .ApplyTo(seq => seq[..4]);

            tzbits
                .Concat(timebits)
                .Concat(daybits)
                .Concat(monthbits)
                .ApplyTo(bitseq => bitseq.ToByteArray())
                .Consume(data => context.Buffer.Write(data));

            // Year component (varbytes)
            timestamp.Year
                .ApplyTo(BitConverter.GetBytes)
                .ApplyTo(VarBytes.Of)
                .Consume(data => context.Buffer.Write(data));
        }
    }
}
