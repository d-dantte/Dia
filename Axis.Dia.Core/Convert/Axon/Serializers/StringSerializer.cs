using Axis.Dia.Core.Utils;

namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class StringSerializer : ISerializer<Types.String>
    {
        private static readonly string NullTypeText = $"*.{DiaType.String.ToString().ToLower()}";

        public static string Serialize(Types.String value, SerializerContext context)
        {
            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)}::"
                : string.Empty;

            if (value.IsNull)
                return $"{attributeText}{NullTypeText}";

            var stringText = context.Options.Strings.Style switch
            {
                Options.StringStyle.Inline => SerializeInline(value.Value!, context),
                Options.StringStyle.Verbatim => SerializeVerbatim(value.Value!, context),
                _ => throw new InvalidOperationException($"Invalid string style: {context.Options.Strings.Style}")
            };

            return $"{attributeText}{stringText}";
        }

        private static string SerializeVerbatim(
            string value,
            SerializerContext context)
        {
            // in the future, support the 'new-line', and 'gredy-line' escape sequences, to aid with formatting
            // the verbatim string.
            // 1. new-line escape is a '\' followed by the '\xa' character. During interpretation, these escapes are ignored.
            // 2. greedy-line escape is a '\' followed by the '\x20' character. Same as above, this is ignored.
            // 3. Finally, the '\' character is escaped as "\\". Unlike above, this is intepreted as a '\'
            return $"`{value}`";
        }

        private static string SerializeInline(string value, SerializerContext context)
        {
            return context.Options.Strings.UseMultiline switch
            {
                false => value
                    .Select(EscapeInlineStringCharacter)
                    .JoinUsing()
                    .ApplyTo(s => $"\"{s}\""),

                true => value
                    .Batch(context.Options.Strings.MultilineThreshold)
                    .Select(batch => (LineIndex: batch.BatchIndex, Line: batch.Batch
                        .Select(EscapeInlineStringCharacter)
                        .JoinUsing()))
                    .Select(lines => lines.LineIndex switch
                    {
                        0 => $"\"{lines.Line}\"",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ \"{lines.Line}\""
                    })
                    .JoinUsing()
            };
        }

        private static string EscapeInlineStringCharacter(char character)
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
