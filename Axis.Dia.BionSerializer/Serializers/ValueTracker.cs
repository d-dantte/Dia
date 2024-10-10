using Axis.Dia.Bion.Serializers.Contracts;
using Axis.Dia.Bion.Types;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Bion.Serializers
{
    public class ValueTracker: IValueTracker
    {
        private readonly Dictionary<IDiaValue, Reference> indexCache = new();

        public bool TryAdd(
            IDiaValue value,
            Func<IDiaValue, Reference> indexProvider,
            out Reference @ref)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(indexProvider);

            if (indexCache.TryGetValue(value, out @ref))
                return false;

            else
            {
                @ref = indexProvider.Invoke(value);
                indexCache.Add(value, @ref);
                return true;
            }
        }
    }
}
