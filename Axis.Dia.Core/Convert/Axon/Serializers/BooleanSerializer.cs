namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class BooleanSerializer : ISerializer<Types.Boolean>
    {
        private static readonly string NullTypeText = $"*.{DiaType.Bool.ToString().ToLower()}";

        public static string Serialize(Types.Boolean value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var valueText = value.Value!.Value switch
            {
                true => "true",
                false => "false"
            };

            valueText = context.Options.Bools.UseCanonicalForm
                ? $"'#{DiaType.Bool} {valueText}'"
                : valueText;

            return $"{attributeText}{valueText}";
        }
    }
}
