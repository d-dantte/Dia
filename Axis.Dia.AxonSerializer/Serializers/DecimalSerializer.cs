using Axis.Dia.Core;

namespace Axis.Dia.Axon.Serializers
{
    public class DecimalSerializer : IValueSerializer<Core.Types.Decimal>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Decimal.ToString().ToLower()}";

        public static string Serialize(Core.Types.Decimal value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var decimalText = context.Options.Decimals.Notation switch
            {
                Options.DecimalNotation.Scientific => value.Value!.Value.ToScientificString(),
                _ => value.Value!.Value.ToNonScientificString()
            };

            return $"{attributeText}{decimalText}";
        }
    }
}
