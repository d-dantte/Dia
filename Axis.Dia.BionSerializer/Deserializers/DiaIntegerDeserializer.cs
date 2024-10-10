using Axis.Dia.Bion.Deserializers.Contracts;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Utils;
using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Bion.Deserializers
{
    public class DiaIntegerDeserializer :
        ITypeDeserializer<Integer>,
        IDefaultInstance<DiaIntegerDeserializer>
    {
        public static DiaIntegerDeserializer DefaultInstance { get; } = new();

        public Core.Types.Integer DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Int.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Int}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return Integer.Null(attributes!);

            else if (!typeMetadata.IsCustomFlagSet)
                return Integer.Of(0, attributes);

            return stream
                .ReadChunks()
                .ToRawBytes()
                .ApplyTo(bytes => new BigInteger(bytes))
                .ApplyTo(bi => Integer.Of(bi, attributes));
        }
    }
}
