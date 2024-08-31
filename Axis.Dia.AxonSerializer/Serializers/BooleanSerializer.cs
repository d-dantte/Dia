using Axis.Dia.Core;

namespace Axis.Dia.Axon.Serializers
{
    public class BooleanSerializer : IValueSerializer<Core.Types.Boolean>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Bool.ToString().ToLower()}";

        public static string Serialize(Core.Types.Boolean value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException("Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var valueText = value.Value!.Value switch
            {
                true => "true",
                false => "false"
            };

            return $"{attributeText}{valueText}";
        }
    }
}
