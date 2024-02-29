using Axis.Dia.Core.Types;
using Axis.Dia.Core.Utils;
using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class RecordSerializer : ISerializer<Record>
    {
        private static readonly string NullTypeText = $"*.{DiaType.Record.ToString().ToLower()}";
        private static readonly Regex IdentifierPattern = new Regex(
            "^[$a-zA-Z_][$a-zA-Z0-9-_]*$",
            RegexOptions.Compiled);

        public static string Serialize(Record value, SerializerContext context)
        {
            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            if (value.IsEmpty)
                return $"{attributeText}{{}}";

            var next = context.Next();
            var properties = value.Select(property => Serialize(property, next));

            var propertyText = !context.Options.Records.UseMultiline
                ? properties.JoinUsing(", ")
                : properties
                    .Select((prop, index) => $"{(index > 0 ? "," : "")}{context.Options.NewLine}{next.Indent()}{prop}")
                    .JoinUsing()
                    .ApplyTo(txt => $"{txt}{context.Options.NewLine}{context.Indent()}");

            return $"{attributeText}{{{propertyText}}}";
        }

        private static string Serialize(Record.Property property, SerializerContext context)
        {
            var attributeText = property.Name.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(property.Name.Attributes, context)}::"
                : string.Empty;

            var propertyNameText = property.Name.Name
                .Select(EscapePropertyNameCharacter)
                .JoinUsing()
                .ApplyTo(nameText => (IsIdentifier(nameText), context.Options.Records.AlwaysQuotePropertyName) switch
                {
                    (true, false) => nameText,
                    (_, _) => $"\"{nameText}\""
                });

            var propertyValueText = ValueSerializer.Serialize(property.Value, context);

            return $"{attributeText}{propertyNameText}: {propertyValueText}";
        }

        private static bool IsIdentifier(
            string propertyName)
            => IdentifierPattern.IsMatch(propertyName);

        private static string EscapePropertyNameCharacter(char character)
        {
            return character switch
            {
                '\\' => "\\\\",
                '\"' => "\\\"",
                '\n' => "\\\n",
                '\r' => "\\\r",
                '\0' => "\\\0",
                '\a' => "\\\a",
                '\b' => "\\\b",
                '\f' => "\\\f",
                '\t' => "\\\t",
                '\v' => "\\\v",
                _ => (char.IsControl(character), character < 256) switch
                {
                    (true, true) => $"\\x{(int)character:x2}",
                    (true, false) => $"\\u{(int)character:x4}",
                    (_, _) => character.ToString()
                }
            };
        }
    }
}
