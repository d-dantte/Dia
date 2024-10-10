using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.BionSerializer.Deserializers.Contracts
{
    public interface ITypeDeserializer<TDiaType> where TDiaType : IDiaType
    {
        TDiaType DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context);
    }
}
