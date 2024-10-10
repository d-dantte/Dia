using Axis.Dia.Core.Contracts;

namespace Axis.Dia.BionSerializer.Serializers.Contracts
{
    public interface ISerializerContext
    {
        IValueTracker ValueTracker { get; }

        ByteBuffer Buffer { get; }

        ITypeSerializer<IDiaType> TypeSerializer { get; }

        IAttributeSetSerializer AttributeSetSerializer { get; }
    }
}
