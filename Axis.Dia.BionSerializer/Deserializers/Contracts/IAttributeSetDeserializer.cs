namespace Axis.Dia.BionSerializer.Deserializers.Contracts
{
    public interface IAttributeSetDeserializer
    {
        Core.Types.Attribute[] DeserializeAttributeSet(Stream stream, IDeserializerContext context);
    }
}
