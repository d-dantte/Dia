using Axis.Dia.Core;

namespace Axis.Dia.BionSerializer.Serializers
{
    public interface ITypeSerializer<TDiaType> where TDiaType : IDiaType
    {
        void SerializeType(TDiaType value, SerializerContext context);
    }
}
