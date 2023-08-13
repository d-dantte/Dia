namespace Axis.Dia.Convert.Type
{
    /// <summary>
    /// A structure that keeps track of instances that have been serialized/deserialized by the converter.
    /// </summary>
    public class TypeConverterContextMetadata
    {
        private readonly Dictionary<string, Metadata> metadataMap = new Dictionary<string, Metadata>();

        /// <summary>
        /// Gets all keys in the underlying map
        /// </summary>
        public IEnumerable<string> Keys => metadataMap.Keys;

        /// <summary>
        /// Gets the number of items in the underlying map
        /// </summary>
        public int Count => metadataMap.Count;

        /// <summary>
        /// Attempts to add metadata for the given instance into the underlying map. if successful, the new id for the metadata
        /// is returned. If it cannot be added (because it already exists), it's id is returned.
        /// </summary>
        /// <param name="key">The key for the data</param>
        /// <param name="data">The data</param>
        /// <param name="id">The metadata id</param>
        /// <returns>True if the info was added, false if it already existed.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool TryAddMetadata(string key, string data, out Guid id)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (metadataMap.TryGetValue(key, out var metadata))
            {
                id = metadata.Id;
                return false;
            }

            metadata = new Metadata(data);
            id = metadata.Id;
            metadataMap[key] = metadata;
            return true;
        }

        /// <summary>
        /// Gets the data stored using the given key, if it exists.
        /// </summary>
        /// <param name="key">The key to search with</param>
        /// <param name="data">The data</param>
        /// <returns>True if the data was found, false otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool TryGetMetadata(string key, out string? data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (metadataMap.TryGetValue(key, out var metadata))
            {
                data = metadata.Data;
                return true;
            }

            data = null;
            return false;
        }

        /// <summary>
        /// Removes the data for the given key and guid from the underlying map
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="id">The guid</param>
        /// <param name="data">The data</param>
        /// <returns>True if the data was removed, False otherwise.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool TryRemoveMetadata(string key, Guid id, out string? data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (metadataMap.TryGetValue(key, out var metadata)
                && metadata.Id == id)
            {
                metadataMap.Remove(key);
                data = metadata.Data;
                return true;
            }

            data = null;
            return false;
        }

        private struct Metadata
        {
            public string Data { get; }

            public Guid Id { get; }

            public Metadata(string data)
            {
                Id = Guid.NewGuid();
                Data = data;
            }
        }
    }
}
