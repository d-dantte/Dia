using Axis.Dia.Core.Contracts;

namespace Axis.Dia.BionSerializer.Deserializers.Contracts
{
    public interface IValueDeserializer
    {
        IDiaValue DeserializeValue(Stream stream, IDeserializerContext context);
    }
}
