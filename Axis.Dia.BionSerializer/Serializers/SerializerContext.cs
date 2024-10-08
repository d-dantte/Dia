namespace Axis.Dia.BionSerializer.Serializers
{
    public class SerializerContext
    {
        public ValueTracker ValueTracker { get; } = new();

        public ByteBuffer Buffer { get; } = new();

        public TypeSerializer TypeSerializer { get; } = TypeSerializer.DefaultInstance;
    }
}
