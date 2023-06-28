using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class InstantPayloadSerializer :
        IPayloadSerializer<InstantValue>,
        IPayloadProvider<InstantValue>
    {
        // prohibits instantiation
        private InstantPayloadSerializer() { }

        public static ValuePayload<InstantValue> CreatePayload(InstantValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | (!value.IsNull ? TypeMetadata.MetadataFlags.Overflow : TypeMetadata.MetadataFlags.None);

            var cmetaBits = value.Value switch
            {
                null => BitSequence.Empty,
                DateTimeOffset instant => BitSequence
                    .Empty
                    .Concat(DayBits(instant)[^1..])
                    .Concat(HourBits(instant)[^1..])
                    .ApplyTo(bits =>
                    {
                        var tz = TimeZoneBits(instant);
                        return bits.Concat(tz.Sign).Concat(tz.HourBits[^2..]);
                    })
                    .ApplyTo(BitSequence.Of)
            };

            var cmeta = VarBytes
                .Of(cmetaBits)
                .ToCustomMetadata();

            var typeMetadata = TypeMetadata.Of(
                DiaType.Instant,
                metadataFlags,
                cmeta);

            return new ValuePayload<InstantValue>(value, typeMetadata);
        }

        public static IResult<InstantValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Instant.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Instant}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many bytes to read for the significand
                .Map(tmeta => (
                    IsNullValue: tmeta.IsNull,
                    MetadataBits: tmeta.CustomMetadata
                        .Select(cmeta => cmeta.DataByteValue)
                        .ToArray()
                        .ApplyTo(BitSequence.Of),
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the decimal from the scale, and the sig
                .Map(tuple => (
                    tuple.Annotations,
                    Instant: tuple.IsNullValue
                        ? (DateTimeOffset?)null
                        : ReadInstant(stream, tuple.MetadataBits)))

                // construct the InstantValue
                .Map(tuple => InstantValue.Of(
                    tuple.Instant,
                    tuple.Annotations));
        }

        public static IResult<byte[]> Serialize(InstantValue value, BinarySerializerContext context)
        {
            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var instantDataResult = value.Value switch
                {
                    null => Result.Of(Array.Empty<byte>),
                    DateTimeOffset instant => Result.Of(() =>
                    {
                        var yearBytes = instant.Year
                            .ApplyTo(year => new BigInteger(year))
                            .ApplyTo(year => year.ToVarBytes(true))
                            .ToArray();

                        var mdBytes = BitSequence.Empty
                            .Concat(MonthBits(instant))
                            .Concat(DayBits(instant)[..^1])
                            .ToByteArray();

                        var hmsBytes = BitSequence.Empty
                            .Concat(SecondsBits(instant)[..6])
                            .Concat(MinuteBits(instant)[..6])
                            .Concat(HourBits(instant)[..4])
                            .ToByteArray();

                        var tickBytes = TicksBits(instant).ToByteArray();

                        var tzBytes = TimeZoneBits(instant)
                            .ApplyTo(tzbits => tzbits.MinutesBits.Concat(tzbits.HourBits[..2]))
                            .ToByteArray();

                        return yearBytes
                            .ConcatWith(mdBytes)
                            .ConcatWith(hmsBytes)
                            .ConcatWith(tickBytes)
                            .ConcatWith(tzBytes);
                    })
                };

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the decimal bytes
                    .Combine(instantDataResult, (bytes, decimalBytes) => bytes.Concat(decimalBytes))
                    .Map(bytes => bytes.ToArray());
            }
            catch (Exception ex)
            {
                return Result.Of<byte[]>(ex);
            }
        }

        internal static BitSequence HourBits(DateTimeOffset instant)
        {
            return new BigInteger(instant.Hour)
                .ToByteArray()
                .ApplyTo(BitSequence.Of)
                [..5];
        }

        internal static BitSequence MinuteBits(DateTimeOffset instant)
        {
            return new BigInteger(instant.Minute)
                .ToByteArray()
                .ApplyTo(BitSequence.Of)
                [..6];
        }

        internal static BitSequence SecondsBits(DateTimeOffset instant)
        {
            return new BigInteger(instant.Second)
                .ToByteArray()
                .ApplyTo(BitSequence.Of)
                [..6];
        }

        internal static BitSequence TicksBits(DateTimeOffset instant)
        {
            var ticks =
                (instant.Millisecond * 10000)
                + (instant.Microsecond * 10)
                + (instant.Nanosecond / 100);

            return BitConverter
                .GetBytes(ticks)[..3]
                .ApplyTo(BitSequence.Of);
        }

        internal static (bool Sign, BitSequence HourBits, BitSequence MinutesBits) TimeZoneBits(DateTimeOffset instant)
        {
            var tz = instant.Offset;
            return (
                Sign: int.IsPositive(tz.Hours),
                HourBits: BitSequence.Of((byte)Math.Abs(tz.Hours))[..4],
                MinutesBits: BitSequence.Of((byte)tz.Minutes)[..6]);
        }

        internal static BitSequence DayBits(DateTimeOffset instant)
        {
            return BitSequence.Of((byte)instant.Day)[..5];
        }

        internal static BitSequence MonthBits(DateTimeOffset instant)
        {
            return BitSequence.Of((byte)instant.Month)[..4];
        }

        internal static DateTimeOffset ReadInstant(Stream stream, BitSequence metadataBits)
        {
            var year = stream
                .ReadVarBytesResult()
                .Map(vb => (int)vb.ToBigInteger(true))
                .Resolve();

            var md = stream
                .ReadExactBytesResult(1)
                .Map(BitSequence.Of)
                .Map(bits => (
                    Month: bits[..4].ToByteArray()[0],
                    Day: bits[4..]
                        .Concat(metadataBits[..1])
                        .ToByteArray()[0]))
                .Resolve();

            var hms = stream
                .ReadExactBytesResult(2)
                .Map(BitSequence.Of)
                .Map(bits => (
                    Second: bits[..6].ToByteArray()[0],
                    Minute: bits[6..12].ToByteArray()[0],
                    Hour: bits[12..]
                        .Concat(metadataBits[1..2])
                        .ToByteArray()[0]))
                .Resolve();

            var ticks = stream
                .ReadExactBytesResult(3)
                .Map(bytes => (int)new BigInteger(bytes, true))
                .Resolve();

            var tz = stream
                .ReadExactBytesResult(1)
                .Map(BitSequence.Of)
                .Map(bits => (
                    Sign: metadataBits[2],
                    Minutes: bits[..6].ToByteArray()[0],
                    Hours: bits[6..]
                        .Concat(metadataBits[3..5])
                        .ToByteArray()[0]))
                .Map(info => new TimeSpan(
                    hours: info.Hours * (info.Sign ? 1 : -1),
                    minutes: info.Minutes,
                    seconds: 0))
                .Resolve();

            return new DateTimeOffset(
                year, md.Month, md.Day,
                hms.Hour,hms.Minute, hms.Second,
                tz)
                + TimeSpan.FromTicks(ticks);
        }
    }
}
