using Axis.Dia.BionSerializer.Metadata;
using Axis.Dia.Core;

namespace Axis.Dia.BionSerializer.Serializers
{
    public interface IMetadataProvider<TDiaType> where TDiaType : IDiaType
    {
        TypeMetadata ExtractMetadata(TDiaType value);
    }
}
