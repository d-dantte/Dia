using Axis.Dia.Bion.Deserializers.Contracts;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Text;

namespace Axis.Dia.Bion.Deserializers
{
    using DiaString = Core.Types.String;

    public class DiaStringDeserializer :
        ITypeDeserializer<DiaString>,
        IDefaultInstance<DiaStringDeserializer>
    {
        public static DiaStringDeserializer DefaultInstance { get; } = new();

        public DiaString DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.String.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.String}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return DiaString.Null(attributes!);

            else if (!typeMetadata.IsCustomFlagSet)
                return DiaString.Of(string.Empty, attributes);

            return stream
                .ReadChunks()
                .ToRawBytes()
                .ApplyTo(Encoding.Unicode.GetString)
                .ApplyTo(@string => DiaString.Of(@string, attributes));
        }
    }
}
