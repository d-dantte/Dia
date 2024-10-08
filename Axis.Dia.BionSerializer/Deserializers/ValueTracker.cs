using Axis.Dia.Core;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Deserializers
{
    public interface IValueTracker
    {
        bool TryAdd(
            BigInteger streamIndex,
            Func<BigInteger, IDiaValue> indexProvider,
            out IDiaValue? value);
    }

    public class ValueTracker: IValueTracker
    {
        private readonly Dictionary<BigInteger, IDiaValue> indexCache = new();

        public bool TryAdd(
            BigInteger streamIndex,
            Func<BigInteger, IDiaValue> indexProvider,
            out IDiaValue? value)
        {
            ArgumentNullException.ThrowIfNull(indexProvider);

            if (streamIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(streamIndex));

            if (indexCache.TryGetValue(streamIndex, out value))
                return false;

            else
            {
                value = indexProvider.Invoke(streamIndex);
                indexCache.Add(streamIndex, value);
                return true;
            }
        }
    }
}
