using Axis.Dia.BionSerializer.Metadata;

namespace Axis.Dia.BionSerializer.Deserializers.Contracts
{
    public interface ITypeMetadataDeserializer
    {
        TypeMetadata DeserializeTypeMetadata(Stream stream, IDeserializerContext context);
    }
}
