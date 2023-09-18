using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Type
{
    /// <summary>
    /// string-object map holding custom data that can be added and accessed by all converters
    /// throughout the serialization/deserialization session. Essentially, the lifecycle of this
    /// map is tied to that of the <see cref="TypeConverterContext"/>
    /// </summary>
    public class MetadataMap
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
                return value.TryCast(out metadata);
            }
            else
            {
                metadata = default;
                return false;
            }
        }
    }
}
