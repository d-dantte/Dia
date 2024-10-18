using Axis.Dia.Json.Path;
using System.Collections.Immutable;

namespace Axis.Dia.Json.Deserializers
{
    using Metadata = (
        int? Ref,
        ImmutableArray<Core.Types.Attribute> ValueAttributes,
        ImmutableArray<Core.Types.Attribute> PropertyAttributes);

    using FullMetadata = (
        ValuePath Path,
        int? Ref,
        ImmutableArray<Core.Types.Attribute> ValueAttributes,
        ImmutableArray<Core.Types.Attribute> PropertyAttributes);

    internal class DeserializerContext
    {
        public ReferenceMap ReferenceMap { get; } = new();

        public ImmutableDictionary<ValuePath, Metadata> MetadataMap { get; }


        public DeserializerContext(params FullMetadata[] metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            MetadataMap = metadata.ToImmutableDictionary(
                _metadata => _metadata.Path,
                _metadata => (_metadata.Ref, _metadata.ValueAttributes, _metadata.PropertyAttributes));
        }

        internal Core.Types.Attribute[] GetMetadataValueAttributes(ValuePath path)
        {
            return MetadataMap.TryGetValue(path, out var metadata)
                ? [.. metadata.ValueAttributes]
                : [];
        }

        internal Core.Types.Attribute[] GetMetadataPropertyAttributes(ValuePath path)
        {
            return MetadataMap.TryGetValue(path, out var metadata)
                ? [.. metadata.PropertyAttributes]
                : [];
        }

        internal int? GetMetadataRef(ValuePath path)
        {
            return MetadataMap.TryGetValue(path, out var metadata)
                ? metadata.Ref
                : null;
        }
    }
}
