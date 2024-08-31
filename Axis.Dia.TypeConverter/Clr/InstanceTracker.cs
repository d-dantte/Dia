using Axis.Dia.Core;

namespace Axis.Dia.TypeConverter.Clr
{
    public class InstanceTracker
    {
        private readonly Dictionary<IDiaValue, object> instanceCache = new();

        /// <summary>
        /// Adds the given key to the tracker, only if it doesn't exist. The value provider is only called if the
        /// value does not exist.
        /// </summary>
        /// <typeparam name="TValue">The type of the instance key</typeparam>
        /// <param name="diaValue"></param>
        /// <param name="valueProvider"></param>
        /// <param name="clrValue"></param>
        /// <returns></returns>
        public bool TryAdd<TValue>(
            IDiaValue diaValue,
            Func<IDiaValue, TValue> valueProvider,
            out TValue clrValue)
        {
            ArgumentNullException.ThrowIfNull(diaValue);
            ArgumentNullException.ThrowIfNull(valueProvider);

            if (instanceCache.TryGetValue(diaValue, out var value))
            {
                clrValue = (TValue)value;
                return false;
            }
            else
            {
                clrValue = valueProvider.Invoke(diaValue);
                instanceCache.Add(diaValue, clrValue!);
                return true;
            }
        }
    }
}
