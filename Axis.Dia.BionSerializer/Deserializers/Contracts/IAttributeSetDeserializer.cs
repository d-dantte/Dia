namespace Axis.Dia.Bion.Deserializers.Contracts
{
    public interface IAttributeSetDeserializer
    {
        Core.Types.Attribute[] DeserializeAttributeSet(Stream stream, IDeserializerContext context);
    }
}
