using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Typhon.Clr
{
    /// <summary>
    /// Maps a combination of dia-values + destination types, to a clr object. This helps track clr values created for the combination of dia values and the receiving type.
    /// The idea behind this is, since in a Dia-Value graph a value can be referenced multiple times, the values of these references, while converting to a clr graph,
    /// will be created and reused only if the exact same instance is needed - and the instance is determined by the receiver type (property type, list/array/enumerable type).
    /// </summary>
    public class InstanceTracker
    {
        private readonly Dictionary<(IDiaValue, Type), object> instanceCache = new();

        /// <summary>
        /// Attempts to add a new value into the cache. If added, return true, else return false; either way, return the clr instance.
        /// </summary>
        /// <typeparam name="TValue">The clr instance type</typeparam>
        /// <param name="diaValue">The dia value</param>
        /// <param name="destinationType">The receiving type</param>
        /// <param name="valueProvider">The value provider, called if a value needs to be added</param>
        /// <param name="clrValue">The value that was added to, or retrieved from the cache</param>
        /// <returns></returns>
        public bool TryAdd<TValue>(
            IDiaValue diaValue,
            Type destinationType,
            Func<IDiaValue, Type, TValue> valueProvider,
            out TValue clrValue)
        {
            ArgumentNullException.ThrowIfNull(diaValue);
            ArgumentNullException.ThrowIfNull(valueProvider);

            if (instanceCache.TryGetValue((diaValue, destinationType), out var value))
            {
                clrValue = (TValue)value;
                return false;
            }
            else
            {
                clrValue = valueProvider.Invoke(diaValue, destinationType);
                instanceCache.Add((diaValue, destinationType), clrValue!);
                return true;
            }
        }
    }
}
