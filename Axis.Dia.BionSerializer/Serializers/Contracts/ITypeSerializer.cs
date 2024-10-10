using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Bion.Serializers.Contracts
{
    public interface ITypeSerializer<TDiaType> where TDiaType : IDiaType
    {
        void SerializeType(TDiaType value, ISerializerContext context);
    }
}
