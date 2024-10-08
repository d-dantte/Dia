using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Text;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public class DiaAttributeDeserializer :
        ITypeDeserializer<Core.Types.Attribute>,
        IDefaultInstance<DiaAttributeDeserializer>
    {
        public static DiaAttributeDeserializer DefaultInstance { get; } = new();

        public Core.Types.Attribute DeserializeType(
            Stream stream,
            TypeMetadata typeMetadata,
            IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Attribute.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Attribute}, actual: {typeMetadata.Type}]");

            var key = stream
                .ReadChunks()
                .ApplyTo(chunks => chunks.ToRawBytes())
                .ApplyTo(Encoding.Unicode.GetString);

            var value = typeMetadata.IsCustomFlagSet switch
            {
                false => null,
                true => stream
                    .ReadChunks()
                    .ApplyTo(chunks => chunks.ToRawBytes())
                    .ApplyTo(Encoding.Unicode.GetString)
            };

            return Core.Types.Attribute.Of(key, value);
        }
    }
}
