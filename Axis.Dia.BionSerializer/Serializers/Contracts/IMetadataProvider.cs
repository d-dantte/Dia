using Axis.Dia.Bion.Metadata;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Bion.Serializers.Contracts
{
    public interface IMetadataProvider<TDiaType> where TDiaType : IDiaType
    {
        TypeMetadata ExtractMetadata(TDiaType value);
    }
}
