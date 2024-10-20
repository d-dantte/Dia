using Axis.Dia.Core.Types;
using Axis.Dia.Json.Path;
using Axis.Luna.Extensions;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Json.Serializers
{
    internal class SerializerContext
    {
        private static readonly string MetadataRefProperty = "ref";
        private static readonly string MetadataValueAttributeProperty = "value-attribute";
        private static readonly string MetadataPropertyAttributeProperty = "property-attribute";

        private readonly Dictionary<ValuePath, Metadata> metadataMap = [];
        private readonly ReferenceMap referenceMap = new();

        public ReferenceMap ReferenceMap => referenceMap;

        internal void SerializePropertyAttributes(AttributeSet attributes, ValuePath path)
        {
            var metadata = metadataMap.GetOrAdd(path, _ => new Metadata());

            if (metadata.PropertyAttributes is not null)
                throw new InvalidOperationException($"Invalid path: duplicate property attributes. '{path}'");

            metadata.PropertyAttributes = attributes;
        }

        internal void SerializeValueAttributes(AttributeSet attributes, ValuePath path)
        {
            var metadata = metadataMap.GetOrAdd(path, _ => new Metadata());

            if (metadata.ValueAttributes is not null)
                throw new InvalidOperationException($"Invalid path: duplicate value attributes. '{path}'");

            metadata.ValueAttributes = attributes;
        }

        internal void SerializeRef(int @ref, ValuePath path)
        {
            var metadata = metadataMap.GetOrAdd(path, _ => new Metadata());

            if (metadata.Ref is not null)
                throw new InvalidOperationException($"Invalid path: duplicate ref. '{path}'");

            metadata.Ref = @ref;
        }

        /// <summary>
        /// TODO: Skip generating JObject instances for empty metadata to reduce bloat
        /// </summary>
        internal JObject GenerateMetadata()
        {
            return metadataMap
                .Select(kvp => (
                    kvp.Key,
                    Value: (
                        kvp.Value.Ref,
                        PropAttributes: Serialize(kvp.Value.PropertyAttributes),
                        ValueAttributes: Serialize(kvp.Value.ValueAttributes))))
                .Aggregate(new JObject(), (jobj, info) =>
                {
                    var metadata = new JObject();

                    if (info.Value.Ref is int @ref)
                        metadata[MetadataRefProperty] = $"0x{@ref:x}";

                    if (info.Value.PropAttributes is not null)
                        metadata[MetadataPropertyAttributeProperty] = info.Value.PropAttributes;

                    if (info.Value.ValueAttributes is not null)
                        metadata[MetadataValueAttributeProperty] = info.Value.ValueAttributes;

                    if (metadata.HasValues)
                        jobj[info.Key.ToString()] = metadata;

                    return jobj;
                });
        }

        internal static string? Serialize(AttributeSet? attributes)
        {
            if (attributes is null || attributes.Value.IsEmpty())
                return null;

            return attributes!.Value
                .Select(att =>
                {
                    var value = att.Value?.Contains(';') ?? false
                        ? att.Value!.Replace(";", "\\;")
                        : att.Value!;

                    var vtext = value is not null
                        ? $":{value}"
                        : "";

                    return $"@{att.Key}{vtext};";
                })
                .JoinUsing(" ");
        }

        #region Nested Types
        internal record Metadata
        {
            internal AttributeSet? PropertyAttributes { get; set; }

            internal AttributeSet? ValueAttributes { get; set; }

            internal int? Ref { get; set; }
        }
        #endregion
    }
}
