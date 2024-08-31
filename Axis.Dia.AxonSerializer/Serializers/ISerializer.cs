using Axis.Dia.Core;

namespace Axis.Dia.Axon.Serializers
{
    public interface ISerializer<TValue>
    {
        abstract static string Serialize(TValue value, SerializerContext context);
    }

    public interface IValueSerializer<TValue> :
        ISerializer<TValue>
        where TValue : IDiaValue
    { }
}
