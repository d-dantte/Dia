using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public class DiaBlobDeserializer :
        ITypeDeserializer<Blob>,
        IDefaultInstance<DiaBlobDeserializer>
    {
        public static DiaBlobDeserializer DefaultInstance { get; } = new();

        public Blob DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Blob.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Blob}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return Blob.Null(attributes);

            if (!typeMetadata.IsCustomFlagSet)
                return Blob.Of([], attributes);

            // Read blob length/size
            var length = stream
                .ReadVarBytes()
                .ApplyTo(varbytes => varbytes.ToBigInteger())
                .As<int>();

            // read bytes
            var byteArray = new byte[length];
            stream.ReadExactly(byteArray, 0, byteArray.Length);

            return Blob.Of(byteArray, attributes);
        }
    }
}
