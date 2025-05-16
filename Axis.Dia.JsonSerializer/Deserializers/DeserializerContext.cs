using System.Collections.Immutable;

namespace Axis.Dia.Json.Deserializers
{
    public class DeserializerContext
    {
        private readonly List<ValueDeserializer.Ref> _refs = [];

        internal ImmutableArray<ValueDeserializer.Ref> RefInstances => [.. _refs];

        internal ReferenceMap ReferenceMap { get; } = new();

        internal void AddRef(ValueDeserializer.Ref @ref)
        {
            _refs.Add(@ref);
        }

        public void ResolveRefs()
        {
            foreach (var @ref in _refs)
            {
                @ref.Resolve(ReferenceMap);
            }
        }
    }
}
