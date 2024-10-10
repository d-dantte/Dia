using Axis.Dia.Bion.Deserializers.Contracts;
using Axis.Dia.Bion.Metadata;
using Axis.Dia.Bion.Utils;
using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Text;

namespace Axis.Dia.Bion.Deserializers
{
    using DiaSymbol = Core.Types.Symbol;

    public class DiaSymbolDeserializer :
        ITypeDeserializer<DiaSymbol>,
        IDefaultInstance<DiaSymbolDeserializer>
    {
        public static DiaSymbolDeserializer DefaultInstance { get; } = new();

        public DiaSymbol DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Symbol.Equals(typeMetadata.Type))
                throw new InvalidOperationException(
                    $"Invalid typemetadata: [expected:{DiaType.Symbol}, actual: {typeMetadata.Type}]");

            var attributes = typeMetadata.IsAnnotated switch
            {
                true => context.AttributeSetDeserializer.DeserializeAttributeSet(stream, context),
                false => []
            };

            if (typeMetadata.IsNull)
                return DiaSymbol.Null(attributes!);

            else if (!typeMetadata.IsCustomFlagSet)
                return DiaSymbol.Of(string.Empty, attributes);

            return stream
                .ReadChunks()
                .ToRawBytes()
                .ApplyTo(Encoding.Unicode.GetString)
                .ApplyTo(@Symbol => DiaSymbol.Of(@Symbol, attributes));
        }
    }
}
