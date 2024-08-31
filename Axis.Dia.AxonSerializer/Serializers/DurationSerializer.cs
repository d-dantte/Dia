using Axis.Dia.Core;

namespace Axis.Dia.Axon.Serializers
{
    public class DurationSerializer : IValueSerializer<Core.Types.Duration>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Duration.ToString().ToLower()}";

        public static string Serialize(Core.Types.Duration value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            return $"{attributeText}'{DiaType.Duration} {value.Value!.Value:dd\\.hh\\:mm\\:ss\\.fffffff}'";
        }
    }
}
