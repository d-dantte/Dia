using Axis.Luna.Extensions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Axis.Dia.Utils.EscapeSequences
{
    public class JsonStringEscapeSequenceGroup : EscapeSequenceGroup
    {
        private static readonly Regex AsciiPattern = new(@"\\(?'ws'[bfnrt\\/""])", RegexOptions.Compiled);
        private static readonly Regex Hex4Pattern = new(@"\\u(?'hex'[a-fA-F0-9]{4})", RegexOptions.Compiled);

        public override string Name => "JsonStringEscapeSequences";

        public override string? Escape(string? value)
        {
            if (value is null)
                return null;

            return value
                .Select(EscapeCharacter)
                .JoinUsing("");
        }

        public override string? Unescape(string? value)
        {
            if (value is null)
                return null;

            return value
                .ApplyTo(UnescapeHex4)
                .ApplyTo(UnescapeWS);
        }

        private string EscapeCharacter(char c)
        {
            return c switch
            {
                '\b' => "\\b",
                '\f' => "\\f",
                '\n' => "\\n",
                '\t' => "\\t",
                '\r' => "\\r",
                '\\' => "\\\\",
                '/' => "\\/",
                '\"' => "\\\"",
                > '\xff' => $"\\u{(short)c:x}",
                _ => c.ToString()
            };
        }

        private string UnescapeWS(string value)
        {
            return AsciiPattern.Replace(value, match => match.Groups["ws"].Value switch
            {
                "b" => "\b",
                "f" => "\f",
                "n" => "\n",
                "t" => "\t",
                "r" => "\r",
                "\"" => "\"",
                "\\" => "\\",
                "/" => "/",
                _ => throw new InvalidOperationException(
                    $"Invalid Whitespace escape character: {match.Groups["ws"].Value}")
            });
        }

        private string UnescapeHex4(string value)
        {
            return Hex4Pattern.Replace(
                value,
                match => match.Groups["hex"].Value
                    .ApplyTo(hex => short.Parse(hex, NumberStyles.HexNumber))
                    .ApplyTo(shortCode => (char)shortCode)
                    .ApplyTo(@char => @char.ToString()));
        }
    }
}
