using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Bion.Serializers.Contracts
{
    public interface ISerializerContext
    {
        IValueTracker ValueTracker { get; }

        ByteBuffer Buffer { get; }

        ITypeSerializer<IDiaType> TypeSerializer { get; }

        IAttributeSetSerializer AttributeSetSerializer { get; }
    }
}
