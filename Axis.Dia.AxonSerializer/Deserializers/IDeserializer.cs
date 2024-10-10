using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Axon.Deserializers
{
    public interface IDeserializer<TValue>
    {
        abstract static TValue Deserialize(string text, DeserializerContext? context = null);
    }

    public interface IValueDeserializer<TValue> :
        IDeserializer<TValue>
        where TValue : IDiaValue
    { }
}
