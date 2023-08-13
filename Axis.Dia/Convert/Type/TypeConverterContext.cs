using Axis.Luna.Extensions;
using System.Runtime.Serialization;

namespace Axis.Dia.Convert.Type
{
    public class TypeConverterContext
    {
        /// <summary>
        /// For tracking objects during serialization so cyclic reference issues do not occur.
        /// </summary>
        private readonly ObjectIDGenerator objectTracker;

        /// <summary>
        /// Object map, used during de-serialization, for de-referencing instances using their ids.
        /// </summary>
        private readonly Dictionary<string, object?> objectMap = new Dictionary<string, object?>();

        /// <summary>
        /// The options instance
        /// </summary>
        public TypeConverterOptions Options { get; }

        /// <summary>
        /// The current object-graph depth
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// The metadata instance used for the current serialization/deserialization session.
        /// </summary>
        public TypeConverterContextMetadata Metadata { get; }

        /// <summary>
        /// Attempts to START tracking an object.
        /// </summary>
        /// <param name="object">The object to be tracked</param>
        /// <param name="id">The unique id given to the object by the internal tracker</param>
        /// <returns>true if the object is newly tracked, false if the object has previously been tracked</returns>
        public bool TryTrack(object @object, out string id)
        {
            id = objectTracker
                .GetId(@object, out var added)
                .ToString("x");

            return added;
        }

        /// <summary>
        /// Gets the object with the given id, ignoring the producer function, or if the id isn't added,
        /// attempts to invoke the producer to generate the object
        /// </summary>
        /// <param name="objectId">The object's id</param>
        /// <param name="producer">The object producer function</param>
        /// <returns>The object that was added to the internal map</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public object? GetOrAdd(string objectId, Func<string, object?>? producer = null)
        {
            return objectMap.GetOrAdd(objectId, key =>
            {
                if (producer is null)
                    throw new ArgumentNullException($"{nameof(producer)} is null");

                return producer?.Invoke(key);
            });
        }


        internal TypeConverterContext(
            TypeConverterOptions options,
            TypeConverterContextMetadata? metadata = null)
            : this(-1, options, metadata)
        {
        }

        private TypeConverterContext(
            int depth,
            TypeConverterOptions options,
            TypeConverterContextMetadata? metadata = null)
        {
            Depth = depth;
            Options = options;
            Metadata = metadata ?? new TypeConverterContextMetadata();
            objectTracker = new ObjectIDGenerator();
        }

        public TypeConverterContext Next() => new TypeConverterContext(Depth + 1, Options, Metadata);

        public TypeConverterContext Next(int depth) => new TypeConverterContext(depth, Options, Metadata);

        public TypeConverterContext Next(
            int depth,
            TypeConverterOptions options)
            => new TypeConverterContext(depth, options, Metadata);

        public TypeConverterContext Next(
            int depth,
            TypeConverterOptions options,
            TypeConverterContextMetadata metadata)
            => new TypeConverterContext(depth, options, metadata);
    }
}
