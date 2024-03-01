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

            var stringText = (context.Options.Strings.UseCanonicalForm, context.Options.Strings.Style) switch
            {
                (true, _) => SerializeCanonicalForm(value.Value!, context),
                (false, Options.StringStyle.Inline) => SerializeInline(value.Value!, context),
                (false, Options.StringStyle.Verbatim) => SerializeVerbatim(value.Value!, context),
                _ => throw new InvalidOperationException($"Invalid string style: {context.Options.Strings.Style}")
            };

            return $"{attributeText}{stringText}";
        }

        private static string SerializeVerbatim(
            string value,
            SerializerContext context)
        {
            // supports the 'new-line' escape sequences, to aid with formatting the verbatim string.
            // 1. new-line escape is a '\' followed by the '\n' character, or the '\r\n' sequence. During interpretation, these escapes are ignored.
            // 2. '`' is escaped as '\`'.
            // 3. Finally, the '\' character is escaped as "\\". Unlike above, this is intepreted as a '\'
            //return $"`{value}`";
            return context.Options.Strings.VerbatimLineThreshold switch
            {
                null => $"`{value}`",
                ushort threshold => StringSerializer
                    .BreakVerbatimLines(value, threshold)
                    .Select(EscapeBSol)
                    .Select(EscapeTick)
                    .Select(EscapeLinebreak)
                    .Select((line, index) => index switch
                    {
                        0 => $"\\{Environment.NewLine}{line}",
                        _ => line
                    })
                    .JoinUsing()
                    .ApplyTo(SanitizeVerbatim)
            };
        }

        private static string SerializeInline(string value, SerializerContext context)
        {
            return context.Options.Strings.UseMultiline switch
            {
                false => value
                    .EscapeUnicodeControlCharacters()
                    .ApplyTo(s => $"\"{s}\""),

                true => value
                    .Batch(context.Options.Strings.MultilineLineThreshold)
                    .Select(ToString)
                    .Select(EscapeBSol)
                    .Select(EscapeDQuote)
                    .Select(CommonExtensions.EscapeUnicodeControlCharacters)
                    .Select((line, index) => index switch
                    {
                        0 => $"\"{line}\"",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ \"{line}\""
                    })
                    .JoinUsing()
            };
        }

        private static string SerializeCanonicalForm(string value, SerializerContext context)
        {
            return context.Options.Strings.UseMultiline switch
            {
                false => value
                    .EscapeUnicodeControlCharacters()
                    .ApplyTo(s => $"'#{DiaType.String} {s}'"),

                true => value
                    .Batch(context.Options.Strings.MultilineLineThreshold)
                    .Select(ToString)
                    .Select(EscapeBSol)
                    .Select(EscapeDQuote)
                    .Select(CommonExtensions.EscapeUnicodeControlCharacters)
                    .Select((line, index) => index switch
                    {
                        0 => $"'#{DiaType.String} {line}'",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ '{line}'"
                    })
                    .JoinUsing()
            };
        }

        private static string EscapeBSol(
            string value)
            => value?.Replace("\\", "\\\\")!;

        private static string EscapeDQuote(
            string value)
            => value?.Replace("\"", "\\\"")!;

        private static string EscapeTick(
            string value)
            => value?.Replace("`", "\\`")!;

        private static string EscapeLinebreak(string value)
        {

        }

        /// <summary>
        /// Break the string if
        /// <list type="number">
        /// <item>A new-line sequence is encountered: \n, \r, \r\n</item>
        /// <item>Character count of <paramref name="lineThreshold"/> have been encountered</item>
        /// </list>
        /// </summary>
        /// <param name="value">The string to break up into lines</param>
        /// <param name="lineThreshold">The maximum number of characters that may exist in a line</param>
        /// <returns></returns>
        private static IEnumerable<string> BreakVerbatimLines(string value, ushort lineThreshold)
        {

        }

        /// <summary>
        /// If the final sequence in the string is an escaped new-line, remove the sequence.
        /// </summary>
        /// <param name="verbatim"></param>
        /// <returns></returns>
        private static string SanitizeVerbatim(string verbatim)
        {

        }

        private static string ToString(
            IEnumerable<char> chars)
            => new string(chars.ToArray());
    }
}
