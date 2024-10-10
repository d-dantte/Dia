using Axis.Dia.BionSerializer.Deserializers.Contracts;
using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using Axis.Luna.Numerics;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public class DiaDecimalDeserializer :
        ITypeDeserializer<Core.Types.Decimal>,
        IDefaultInstance<DiaDecimalDeserializer>
    {
        public static DiaDecimalDeserializer DefaultInstance { get; } = new();

        public Core.Types.Decimal DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Decimal.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Decimal}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return Core.Types.Decimal.Null(attributes!);

            else if (!typeMetadata.IsOverflowFlagSet)
                return Core.Types.Decimal.Of(0.0m, attributes);

            var isSignificandPositive = typeMetadata.CustomMetadata[0].IsSet(
                CustomMetadata.MetadataFlags.D1);
            var isScalePositive = typeMetadata.CustomMetadata[0].IsSet(
                CustomMetadata.MetadataFlags.D2);

            var scale = stream
                .ReadVarBytes()
                .ToBigInteger()
                .ApplyTo(bi => (int)bi)
                .ApplyTo(value => isScalePositive switch
                {
                    true => value,
                    false => 0 - value
                });

            var sig = stream
                .ReadChunks()
                .ToRawBytes()
                .ApplyTo(bytes => new BigInteger(bytes, true))
                .ApplyTo(value => isSignificandPositive switch
                {
                    true => value,
                    false => 0 - value
                });

            return new BigDecimal(sig, scale).ApplyTo(
                bd => Core.Types.Decimal.Of(bd, attributes));
        }
    }
}
