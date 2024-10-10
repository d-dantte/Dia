using Axis.Dia.Bion.Metadata;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Bion.Deserializers.Contracts
{
    public interface IValueContainerDeserializer<TDiaType, TValue>
        where TDiaType : IValueContainer<TDiaType, TValue>
    {
        TDiaType DeserializeInstance(Stream stream, TypeMetadata metadata, IDeserializerContext context);

        TDiaType DeserializeItems(Stream stream, TDiaType instance, IDeserializerContext context);
    }
}
