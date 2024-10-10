using Axis.Dia.Core.Types;

namespace Axis.Dia.BionSerializer.Serializers.Contracts
{
    public interface IAttributeSetSerializer
    {
        void SerializeAttributeSet(AttributeSet attributes, ISerializerContext context);
    }
}
