using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.BionSerializer.Serializers.Contracts
{
    public interface IMetadataProvider<TDiaType> where TDiaType : IDiaType
    {
        TypeMetadata ExtractMetadata(TDiaType value);
    }
}
