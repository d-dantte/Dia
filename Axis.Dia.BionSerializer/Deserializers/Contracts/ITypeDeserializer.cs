using Axis.Dia.Bion.Metadata;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Bion.Deserializers.Contracts
{
    public interface ITypeDeserializer<TDiaType> where TDiaType : IDiaType
    {
        TDiaType DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context);
    }
}
