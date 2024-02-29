namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class DecimalSerializer : ISerializer<Types.Decimal>
    {
        private static readonly string NullTypeText = $"*.{DiaType.Decimal.ToString().ToLower()}";

        public static string Serialize(Types.Decimal value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var decimalText = value.Value!.Value.ToString();

            decimalText = context.Options.Decimals.UseCanonicalForm
                ? $"'#{DiaType.Decimal} {decimalText}'"
                : decimalText;

            return $"{attributeText}{decimalText}";
        }
    }
}
