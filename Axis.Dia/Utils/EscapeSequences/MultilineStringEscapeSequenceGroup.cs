using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Axis.Dia.Utils.EscapeSequences
{
    internal class MultilineStringEscapeSequenceGroup : EscapeSequenceGroup
    {
        private static readonly Regex AsciiPattern = new("^[0abfntrv\\\"\\\\]\\z", RegexOptions.Compiled);
        private static readonly Regex Hex2Pattern = new("^x[a-fA-F0-9]{2}\\z", RegexOptions.Compiled);
        private static readonly Regex Hex4Pattern = new("^u[a-fA-F0-9]{4}\\z", RegexOptions.Compiled);

        public override string Name => "MultilineStringEscapeSequences";

        public ushort? Alignment { get; }

        public MultilineStringEscapeSequenceGroup(ushort? alignment = null)
        {
            Alignment = alignment;
        }

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
                        sb.Append(asciiEscape switch
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
                        UnescapeWhitespace(reader, sb);
                    }

                    else if (reader.Reset(position).TryNextToken(out var cws) && ('^'.Equals(cws)))
                    {
                        var isDefaultNewLine = true;
                        var cwsPosition = reader.Position;
                        while (reader.TryNextToken(out cws))
                        {
                            if ('\n'.Equals(cws) || '\r'.Equals(cws))
                            {
                                if (isDefaultNewLine)
                                    _ = sb.Append(Environment.NewLine);

                                break;
                            }

                            else
                            {
                                _ = sb.Append(cws switch
                                {
                                    'l' => Environment.NewLine,
                                    'r' => "\r",
                                    'n' => "\n",
                                    _ => throw new FormatException($"Invalid escape sequence: '\\^{reader.Reset(cwsPosition).Peek()}...'")
                                });

                                isDefaultNewLine = false;
                            }
                        }

                        UnescapeWhitespace(reader, sb);
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

        private void UnescapeWhitespace(BufferedTokenReader reader, StringBuilder sb)
        {
            int? alignmentSpaces = null;
            while (reader.TryNextToken(out var ws))
            {
                if (ws != '\n' && ws != '\r' && ws != '\t' && ws != ' ')
                {
                    sb.Append(ws);
                    break;
                }

                if (ws == '\n' || ws == '\r')
                    alignmentSpaces = null;

                if (ws == '\t' || ws == ' ')
                {
                    if (Alignment is not null)
                    {
                        alignmentSpaces ??= 0;
                        alignmentSpaces++;

                        if (alignmentSpaces == Alignment)
                            break;
                    }
                }
            }
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
