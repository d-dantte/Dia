using Axis.Dia.BionSerializer.Types;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.BionSerializer.Serializers.Contracts
{
    public interface IValueTracker
    {
        bool TryAdd(
            IDiaValue value,
            Func<IDiaValue, Reference> indexProvider,
            out Reference @ref);
    }
}
