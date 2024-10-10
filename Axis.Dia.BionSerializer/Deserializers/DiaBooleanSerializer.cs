using Axis.Dia.Bion.Deserializers.Contracts;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Core;

namespace Axis.Dia.Bion.Deserializers
{
    public class DiaBooleanDeserializer :
        ITypeDeserializer<Core.Types.Boolean>,
        IDefaultInstance<DiaBooleanDeserializer>
    {
        public static DiaBooleanDeserializer DefaultInstance { get; } = new();

        public Core.Types.Boolean DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Bool.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Bool}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return Core.Types.Boolean.Null(attributes!);

            return Core.Types.Boolean.Of(typeMetadata.IsCustomFlagSet, attributes!);
        }
    }
}
