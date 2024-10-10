namespace Axis.Dia.Bion.Deserializers.Contracts
{
    public interface IDeserializerContext
    {
        IValueTracker ValueTracker { get; }

        IValueDeserializer ValueDeserializer { get; }

        IAttributeSetDeserializer AttributeSetDeserializer { get; }
    }
}
