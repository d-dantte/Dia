namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public interface ISerializer<TValue>
    where TValue: IDiaValue
    {
        abstract static string Serialize(TValue value, SerializerContext context);
    }
}
