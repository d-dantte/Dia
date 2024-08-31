using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Common.StringEscape;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Serializers
{
    public class RecordSerializer : IValueSerializer<Record>
    {
        private static readonly string NullTypeText = $"null.{DiaType.Record.ToString().ToLower()}";
        private static readonly CommonStringEscaper Escaper = new();

        public static string Serialize(Record value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            else if (context.ReferenceMap.TryAddRef(value, out var refInfo))
            {
                var axonHashText = $"#{refInfo.AxonHash:x}; ";

                if (value.IsEmpty)
                    return $"{attributeText}{{}}";

                var next = context.Next();
                var properties = value.Select(property => Serialize(property, next));

                var propertyText = !context.Options.Records.UseMultiline
                    ? properties.JoinUsing(", ")
                    : properties
                        .Select((prop, index) => $"{(index > 0 ? "," : "")}{context.Options.NewLine}{next.Indent()}{prop}")
                        .JoinUsing("")
                        .ApplyTo(txt => $"{txt}{context.Options.NewLine}{context.Indent()}");

                return $"{axonHashText}{attributeText}{{{propertyText}}}";
            }
            else return $"'Ref:{value.Type} 0x{refInfo.AxonHash:x}'";
        }

        private static string Serialize(Record.Property property, SerializerContext context)
        {
            var attributeText = !property.Name.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(property.Name.Attributes, context)}"
                : string.Empty;

            var propertyNameText = Escaper
                .Escape(property.Name.Name, IsEscapable)
                .ApplyTo(nameText => (property.Name.IsIdentifier, context.Options.Records.AlwaysQuotePropertyName) switch
                {
                    (true, false) => nameText,
                    (_, _) => $"\"{nameText}\""
                });

            var propertyValueText = ValueSerializer.Serialize(property.Value, context);

            return $"{attributeText}{propertyNameText}: {propertyValueText}";
        }

        internal static bool IsEscapable(char character)
        {
            return character switch
            {
                '\\' or '\"' => true,
                _ => char.IsControl(character)
            };
        }
    }
}
