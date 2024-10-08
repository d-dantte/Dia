using Axis.Dia.BionSerializer.Types;
using Axis.Dia.Core;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class ValueTracker
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
