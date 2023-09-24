using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Axis.Dia.Utils.EscapeSequences
{
    internal class ClobEscapeSequenceGroup : EscapeSequenceGroup
    {
        private static readonly Regex AsciiPattern = new("^[0abfntrv\\\"\\\\]\\z", RegexOptions.Compiled);
        private static readonly Regex Hex2Pattern = new("^x[a-fA-F0-9]{2}\\z", RegexOptions.Compiled);
        private static readonly Regex Hex4Pattern = new("^u[a-fA-F0-9]{4}\\z", RegexOptions.Compiled);

        public override string Name => "ClobEscapeSequences";

        public ClobEscapeSequenceGroup()
        {
        }

        public override string? Escape(string? value)
        {
            if (value is null)
                return null;

            return value
                .Select(EscapeClobCharacter)
                .JoinUsing("")
                .Replace(">>", "\\>>");
        }

        public string? EscapeAll(string? value)
        {
            if (value is null)
                return null;

            return value
                .Select(EscapeCharacter)
                .JoinUsing("")
                .Replace(">>", "\\>>");
        }

        public override string? Unescape(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var reader = new BufferedTokenReader(value);
            var sb = new StringBuilder();
            while (reader.TryNextToken(out var @char))
            {
                if (@char != '\\')
                    _ = sb.Append(@char);

                else
                {
                    var position = reader.Position;
                    if (reader.TryNextToken(out var asciiEscape)
                        && AsciiPattern.IsMatch(asciiEscape.ToString()))
                    {
                        _ = sb.Append(asciiEscape switch
                        {
                            '0' => '\0',
                            'a' => '\a',
                            'b' => '\b',
                            'f' => '\f',
                            'n' => '\n',
                            't' => '\t',
                            'r' => '\r',
                            'v' => '\v',
                            '\\' => '\\',
                            '\"' => '\"',
                            _ => throw new FormatException($"Invalid ascii escape char: '{asciiEscape}'")
                        });
                    }

                    else if (reader.Reset(position).TryNextTokens(2, out var endDelim)
                        && endDelim[0] == endDelim[1] && endDelim[0] == '>')
                    {
                        _ = sb.Append(">>");
                    }

                    else if (reader.Reset(position).TryNextTokens(3, out var hex2Escape)
                        && Hex2Pattern.IsMatch(new string(hex2Escape)))
                    {
                        _ = byte
                            .Parse(hex2Escape.AsSpan()[1..], NumberStyles.HexNumber)
                            .ApplyTo(shortCode => (char)shortCode)
                            .ApplyTo(sb.Append);
                    }

                    else if (reader.Reset(position).TryNextTokens(5, out var hex4Escape)
                        && Hex4Pattern.IsMatch(new string(hex4Escape)))
                    {
                        _ = ushort
                            .Parse(hex4Escape.AsSpan()[1..], NumberStyles.HexNumber)
                            .ApplyTo(shortCode => (char)shortCode)
                            .ApplyTo(sb.Append);
                    }

                    else if (reader.Reset(position).TryNextToken(out var ws)
                        && ('\n'.Equals(ws) || '\r'.Equals(ws)))
                    {
                        UnescapeWhitespace(reader);
                    }

                    else
                    {
                        _ = reader.Reset(position);
                        throw new FormatException($"Invalid escape sequence: '\\{reader.Peek()}...'");
                    }
                }
            }

            return sb.ToString();
        }

        private void UnescapeWhitespace(BufferedTokenReader reader)
        {
            while (reader.TryNextToken(out var ws))
            {
                var isWhitespaceChar = ws switch
                {
                    ' ' or '\t' or '\v'
                    or '\r' or '\n' or '\f'
                    or '\a' or '\b' => true,
                    _ => false
                };

                if(!isWhitespaceChar)
                {
                    reader.Back();
                    break;
                }
            }
        }

        private string EscapeClobCharacter(char c)
        {
            return c switch
            {
                '\a' or '\b'
                or '\f' or '\v'
                or '\\' => EscapeCharacter(c),
                _ => c.ToString()
            };
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
    }
}
