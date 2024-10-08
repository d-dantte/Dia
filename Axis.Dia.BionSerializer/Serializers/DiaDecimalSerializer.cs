using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using Axis.Luna.Numerics;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class DiaDecimalSerializer :
        ITypeSerializer<Core.Types.Decimal>,
        IMetadataProvider<Core.Types.Decimal>,
        IDefaultInstance<DiaDecimalSerializer>
    {
        public static DiaDecimalSerializer DefaultInstance { get; } = new();

        public TypeMetadata ExtractMetadata(Core.Types.Decimal value)
        {
            var (significand, scale) = (value.Value ?? 0.0);
            var scaleSign = int.IsPositive(scale);
            var sigSign = BigInteger.IsPositive(significand);

            // padding byte: for positive numbers that have the last bit set, an extra byte (0x0) is padded
            // at the end of the array so it isn't seen as a negative number. e.g,
            // 192 => [1100-0000] => [0000-0000, 1100-0000]
            // -64 => [1100-0000] => [1100-0000] // no padding is done here
            //
            // with the above in mind,
            // 1. if significand is positive, set the cmeta[0(D1)] bit to 1, else leave it at zero
            // 2. if scale is positive, set the cmeta[1(D2)] bit to 1, else leave it at zero
            CustomMetadata[] cmetaArray = value.Value switch
            {
                null => [],
                BigDecimal d when BigDecimal.Zero.Equals(d) => [],
                _ => [CustomMetadata.Of(
                    (sigSign ? CustomMetadata.MetadataFlags.D1 : CustomMetadata.MetadataFlags.None)
                    | (scaleSign ? CustomMetadata.MetadataFlags.D2 : CustomMetadata.MetadataFlags.None))]
            };

            return TypeMetadata
                .Of(DiaType.Decimal)
                .WithAnnotated(!value.Attributes.IsEmpty)
                .WithNull(value.IsNull)
                .WithCustomBit(!BigDecimal.Zero.Equals(value.Value ?? 0))
                .WithCustomMetadata(cmetaArray);
        }

        public void SerializeType(Core.Types.Decimal value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Write the metadata
            this.ExtractMetadata(value)
                .ApplyTo(meta => meta.Metadata.ToByteArray())
                .Consume(array => context.Buffer.Write(array));

            // Write attributes
            context.TypeSerializer.SerializeAttributeSet(value.Attributes, context);

            // Write data
            if (value.IsNull || BigDecimal.Zero.Equals(value.Value!))
                return;

            var (significand, scale) = value.Value!.Value;

            // write the scale as varbytes (since the scale is an int)
            scale.ApplyTo(Math.Abs)
                .As<BigInteger>()
                .ToByteArray(true)
                .ApplyTo(VarBytes.Of)
                .Consume(varbytes => context.Buffer.Write(varbytes));

            // write the significand as a byte chunk (since it is a bigInt)
            significand
                .ApplyTo(BigInteger.Abs)
                .ToByteArray(true)
                .ApplyTo(ByteChunks.Of)
                .Consume(chunks => context.Buffer.Write(chunks));
        }
    }
}
