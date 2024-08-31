using Axis.Dia.Core;

namespace Axis.Dia.TypeConverter.Dia
{
    public class InstanceTracker
    {
        private readonly Dictionary<object, IDiaValue> instanceCache = new();

        /// <summary>
        /// Adds the given key to the tracker, only if it doesn't exist. The value provider is only called if the
        /// value does not exist.
        /// </summary>
        /// <typeparam name="TDiaValue">The specific type of the value to be added</typeparam>
        /// <param name="clrValue">The clr value constituting the key</param>
        /// <param name="valueProvider">The lambda providing the value when needed</param>
        /// <param name="diaValue">The dia value contained in the tracker - either created newly, or already present</param>
        /// <returns>True if the key was added, false if it already existed</returns>
        public bool TryAdd<TDiaValue>(
            object clrValue,
            Func<object, TDiaValue> valueProvider,
            out TDiaValue diaValue)
            where TDiaValue : IDiaValue
        {
            ArgumentNullException.ThrowIfNull(clrValue);
            ArgumentNullException.ThrowIfNull(valueProvider);

            if (instanceCache.TryGetValue(clrValue, out var value))
            {
                diaValue = (TDiaValue)value;
                return false;
            }
            else
            {
                diaValue = valueProvider.Invoke(clrValue);
                instanceCache.Add(clrValue, diaValue);
                return true;
            }
        }
    }
}
