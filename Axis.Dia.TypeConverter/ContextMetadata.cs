using Axis.Luna.Extensions;

namespace Axis.Dia.TypeConverter
{
    /// <summary>
    /// string-object map holding custom data that can be added and accessed by all converters
    /// throughout the serialization/deserialization session. Essentially, the lifecycle of this
    /// map is tied to that of the converter context
    /// </summary>
    public class ContextMetadata
    {
        private readonly Dictionary<string, object> metadataMap = new();

        public bool TryAddMetadata(string key, object value)
        {
            return metadataMap.TryAdd(key, value);
        }

        public bool TryGetMetadata<TMetadata>(string key, out TMetadata? metadata)
        {
            if (metadataMap.TryGetValue(key, out var value))
            {
                return value.Is(out metadata);
            }
            else
            {
                metadata = default;
                return false;
            }
        }

        public TMetadata GetOrAdd<TMetadata>(string key, Func<string, TMetadata> dataProvider)
        {
            ArgumentNullException.ThrowIfNull(dataProvider);

            return metadataMap
                .GetOrAdd(key, key => dataProvider.Invoke(key)!)
                .As<TMetadata>();
        }
    }
}
