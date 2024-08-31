using Axis.Dia.AxonSerializer;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon
{
    public class DeserializerContext
    {
        private readonly Dictionary<int, Action> _valueResolvers = new();

        internal ReferenceMap ReferenceMap { get; } = new();

        internal bool TryAddValueResolver(int key, Action valueResolver)
        {
            return _valueResolvers.TryAdd(key, valueResolver);
        }

        /// <summary>
        /// Executes any reference resolvers added during the deserialization pass.
        /// </summary>
        internal void ExecuteResolvers()
        {
            _valueResolvers.Values.ForEvery(resolver => resolver.Invoke());
        }
    }
}
