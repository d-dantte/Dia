using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Text;

namespace Axis.Dia.Axon.Serializers
{
    public class StringSerializer : IValueSerializer<Core.Types.String>
    {
        private static readonly string NullTypeText = $"null.{DiaType.String.ToString().ToLower()}";

        public static string Serialize(Core.Types.String value, SerializerContext context)
        {
            if (context.IsDefault)
                throw new ArgumentException($"Invalid context: default");

            var attributeText = !value.Attributes.IsEmpty
                ? $"{AttributeSerializer.Serialize(value.Attributes, context)} "
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
            // supports the 'new-line' escape sequences, to aid with formatting the verbatim string.
            // 1. new-line escape is a '\' followed by the '\n' character, or the '\r\n' sequence. During interpretation, these escapes are ignored.
            // 2. '`' is escaped as '\`'.
            // 3. Finally, the '\' character is escaped as "\\". Unlike above, this is intepreted as a '\'
            //return $"`{value}`";
            return context.Options.Strings.LineThreshold switch
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
                    .JoinUsing("")
                    .ApplyTo(SanitizeVerbatim)
            };
        }

        private static string SerializeInline(string value, SerializerContext context)
        {
            return context.Options.Strings.LineThreshold switch
            {
                null => value
                    .EscapeUnicodeControlCharacters()
                    .ApplyTo(s => $"\"{s}\""),

                ushort threshold => value
                    .Batch(threshold)
                    .Select(ToString)
                    .Select(EscapeBSol)
                    .Select(EscapeDQuote)
                    .Select(CommonExtensions.EscapeUnicodeControlCharacters)
                    .Select((line, index) => index switch
                    {
                        0 => $"\"{line}\"",
                        _ => $"{context.Options.NewLine}{context.Indent()}+ \"{line}\""
                    })
                    .JoinUsing("")
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

        internal static string EscapeLinebreak(string value)
        {
            if (value is null || value[^1] == '\n' || value[^1] == '\r')
                return value!;

            else return $"{value}\\{Environment.NewLine}";
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
        internal static IEnumerable<string> BreakVerbatimLines(string value, ushort lineThreshold)
        {
            if (string.IsNullOrEmpty(value))
                yield break;

            var line = new StringBuilder();
            var charCount = 0;
            for (int cnt = 0; cnt < value.Length; cnt++)
            {
                var @char = value[cnt];
                line.Append(@char);
                charCount++;

                if (@char == '\n')
                {
                    yield return line.ToString();

                    line.Clear();
                    charCount = 0;
                }

                else if (@char == '\r')
                {
                    var next = cnt + 1;
                    if (next < value.Length && value[next] == '\n')
                    {
                        cnt++;
                        line.Append(value[next]);
                    }

                    yield return line.ToString();

                    line.Clear();
                    charCount = 0;
                }

                else if (charCount == lineThreshold)
                {
                    yield return line.ToString();

                    line.Clear();
                    charCount = 0;
                }
            }

            if (line.Length > 0)
                yield return line.ToString();
        }

        /// <summary>
        /// If the final sequence in the string is an escaped new-line, remove the sequence.
        /// </summary>
        /// <param name="verbatim"></param>
        /// <returns></returns>
        internal static string SanitizeVerbatim(string verbatim)
        {
            if (verbatim is null)
                return verbatim!;

            if (verbatim.EndsWith("\\\n"))
                return verbatim[..^2];

            if (verbatim.EndsWith("\\\r"))
                return verbatim[..^2];

            if (verbatim.EndsWith("\\\r\n"))
                return verbatim[..^3];

            return verbatim;
        }

        private static string ToString(
            IEnumerable<char> chars)
            => new(chars.ToArray());
    }
}
