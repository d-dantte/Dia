using Axis.Dia.Bion.Metadata;

namespace Axis.Dia.Bion.Deserializers.Contracts
{
    public interface ITypeMetadataDeserializer
    {
        TypeMetadata DeserializeTypeMetadata(Stream stream, IDeserializerContext context);
    }
}
