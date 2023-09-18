using Axis.Luna.Common;

namespace Axis.Dia.Convert.Type
{
    /// <summary>
    /// A structure that keeps track of instances that have been serialized/deserialized by the converter.
    /// <para>
    /// </para>
    /// </summary>
    public class ObjectTracker
    {
        private readonly Dictionary<ObjectKey, (object Object, Guid Id)> objectMap = new();
        private readonly Dictionary<Guid, ObjectKey> idMap = new();

        /// <summary>
        /// Attempts to START tracking an object, returning the ID, and a value indicating if the object was newly tracked, or otherwise.
        /// </summary>
        /// <param name="object">The object to be tracked</param>
        /// <param name="id">A hex representation of the tracking ID for the given object</param>
        /// <returns>True if the object was newly tracked by this call, false otherwise.</returns>
        public bool TryTrack(object @object, out Guid id)
        {
            ArgumentNullException.ThrowIfNull(@object);

            var key = ObjectKey.Of(@object);
            if (objectMap.TryGetValue(key, out var trackInfo))
            {
                id = trackInfo.Id;
                return false;
            }
            else
            {
                idMap[id = Guid.NewGuid()] = key;
                objectMap[key] = (@object, id);
                return true;
            }

        }

        /// <summary>
        /// Attempts to add an object and its ID to the underlying map.
        /// <list type="number">
        /// <item>Objects cannot be added twice. If duplicates are detected, the method returns false</item>
        /// <item>Ids cannot be assigned twice. if duplicates are detected, the method returns false</item>
        /// <item><see cref="Guid.Empty"/> is not a valid assignable id. false is returned if detected</item>
        /// </list>
        /// </summary>
        /// <param name="object">The object to map to the given id</param>
        /// <param name="id">The id to map to the given object</param>
        /// <returns>True if the map was successfully added, false otherwise</returns>
        public bool TryAdd(object @object, Guid id)
        {
            ArgumentNullException.ThrowIfNull(@object);

            if (Guid.Empty.Equals(id))
                throw new ArgumentException($"Supplied id is invalid: '{id}'");

            if (idMap.ContainsKey(id))
                return false;

            var key = ObjectKey.Of(@object);

            if (objectMap.ContainsKey(key))
                return false;

            objectMap[key] = (@object, id);
            idMap[id] = key;
            return true;
        }

        /// <summary>
        /// Attempts to get the object mapped to the given id
        /// </summary>
        /// <param name="id">The id to search for</param>
        /// <param name="object">The object mapped to the given ID</param>
        /// <returns>True if the id exists in the map, false otherwise</returns>
        public bool TryGetObject(Guid id, out object? @object)
        {
            if (idMap.TryGetValue(id, out var key))
            {
                @object = objectMap[key].Object;
                return true;
            }

            //else
            @object = null;
            return false;
        }

        /// <summary>
        /// Checks if the object is being tracked.
        /// </summary>
        /// <param name="object">The object to chekc</param>
        /// <param name="id">The Id, if it is being tracked, </param>
        /// <returns></returns>
        public bool IsTracked(object @object, out Guid id)
        {
            ArgumentNullException.ThrowIfNull(@object);

            var key = ObjectKey.Of(@object);

            if (objectMap.TryGetValue(key, out var trackInfo))
            {
                id = trackInfo.Id;
                return true;
            }

            id = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Checks if the given id is assigned to any of the objects being tracked
        /// </summary>
        /// <param name="id">The id to check</param>
        /// <returns>True if the ID is assigned to a tracked object, false otherwise</returns>
        public bool IsAssigned(Guid id)
        {
            return idMap.ContainsKey(id);
        }


        internal readonly struct ObjectKey: IDefaultValueProvider<ObjectKey>
        {
            internal int HashCode { get; }

            internal System.Type Type { get; }

            #region DefaultValueProvider
            public bool IsDefault => default(ObjectKey).Equals(this);

            public static ObjectKey Default => default;
            #endregion

            private ObjectKey(object @object)
            {
                ArgumentNullException.ThrowIfNull(@object);

                HashCode = @object.GetHashCode();
                Type = @object.GetType();
            }

            internal static ObjectKey Of(object @object) => new(@object);
        }
    }
}
