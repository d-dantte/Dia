using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.Core;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public interface ITypeDeserializer<TDiaType> where TDiaType : IDiaType
    {
        TDiaType DeserializeType(Stream stream, TypeMetadata typeMetadata, IDeserializerContext context);
    }

    public interface ITypeMetadataDeserializer
    {
        TypeMetadata DeserializeTypeMetadata(Stream stream, IDeserializerContext context);
    }

    public interface IValueDeserializer
    {
        IDiaValue DeserializeValue(Stream stream, IDeserializerContext context);
    }

    public interface IAttributeSetDeserializer
    {
        Core.Types.Attribute[] DeserializeAttributeSet(Stream stream, IDeserializerContext context);
    }
}
