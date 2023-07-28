using Axis.Luna.Extensions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Axis.Dia.Utils.EscapeSequences
{
    public class SinglelineStringEscapeSequenceGroup : EscapeSequenceGroup
    {
        private static readonly Regex GreedyLinePattern = new(@"\\(?'gl'\r?\n\s*)", RegexOptions.Compiled);
        private static readonly Regex AsciiPattern = new(@"\\(?'ws'[0abfntrv\\""\n])", RegexOptions.Compiled);
        private static readonly Regex Hex2Pattern = new(@"\\x(?'hex'[a-fA-F0-9]{2})", RegexOptions.Compiled);
        private static readonly Regex Hex4Pattern = new(@"\\u(?'hex'[a-fA-F0-9]{4})", RegexOptions.Compiled);

        public override string Name => "SinglelineStringEscapeSequences";

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
                .ApplyTo(UnescapeHex2)
                .ApplyTo(UnescapeGreedyLine)
                .ApplyTo(UnescapeWS);
        }

        private string EscapeCharacter(char c)
        {
            return c switch
            {
                '\a' => "\\a",
                '\b' => "\\b",
                '\f' => "\\f",
                '\n' => "\\n",
                '\t' => "\\t",
                '\r' => "\\r",
                '\v' => "\\v",
                '\\' => "\\\\",
                '\"' => "\\\"",
                > '\x7f' => c switch
                {
                    <= '\xff' => $"\\x{(short)c:x}",
                    _ => $"\\u{(short)c:x}"
                },
                _ => c.ToString()
            };
        }

        private string UnescapeGreedyLine(string value)
        {
            return GreedyLinePattern.Replace(value, match => "");
        }

        private string UnescapeWS(string value)
        {
            return AsciiPattern.Replace(value, match => match.Groups["ws"].Value switch
            {
                "0" => "\0",
                "a" => "\a",
                "b" => "\b",
                "f" => "\f",
                "n" => "\n",
                "t" => "\t",
                "r" => "\r",
                "v" => "\v",
                "\"" => "\"",
                "\\" => "\\",
                _ => throw new InvalidOperationException(
                    $"Invalid Whitespace escape character: {match.Groups["ws"].Value}")
            });
        }

        private string UnescapeHex2(string value)
        {
            return Hex2Pattern.Replace(
                value,
                match => match.Groups["hex"].Value
                    .ApplyTo(hex => byte.Parse(hex, NumberStyles.HexNumber))
                    .ApplyTo(shortCode => (char)shortCode)
                    .ApplyTo(@char => @char.ToString()));
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
