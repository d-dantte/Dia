
using Axis.Dia.BionSerializer.Serializers.Contracts;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class SerializerContext: ISerializerContext
    {
        public IValueTracker ValueTracker { get; } = new ValueTracker();

        public ByteBuffer Buffer { get; } = new();

        public ITypeSerializer<IDiaType> TypeSerializer => Serializers.TypeSerializer.DefaultInstance;

        public IAttributeSetSerializer AttributeSetSerializer => Serializers.TypeSerializer.DefaultInstance;
    }
}
