using Axis.Dia.Bion.Types;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Bion.Serializers.Contracts
{
    public interface IValueTracker
    {
        bool TryAdd(
            IDiaValue value,
            Func<IDiaValue, Reference> indexProvider,
            out Reference @ref);
    }
}
