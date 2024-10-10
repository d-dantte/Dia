using Axis.Dia.BionSerializer.Deserializers.Contracts;
using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public class DiaDurationDeserializer :
        ITypeDeserializer<Duration>,
        IDefaultInstance<DiaDurationDeserializer>
    {
        public static DiaDurationDeserializer DefaultInstance { get; } = new();

        public Core.Types.Duration DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Duration.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Duration}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return Duration.Null(attributes!);

            else if (!typeMetadata.IsCustomFlagSet)
                return Duration.Of(TimeSpan.Zero, attributes);

            return stream
                .ReadChunks()
                .ToRawBytes()
                .ApplyTo(bytes => BitConverter.ToInt64(bytes))
                .ApplyTo(nanoseconds => Duration.Of(nanoseconds, attributes));
        }
    }
}
