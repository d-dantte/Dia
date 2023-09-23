using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using System.Text;
using System.Text.RegularExpressions;

namespace Axis.Dia.Convert.Axon.Parsers
{
    public class StringParser : IValueSerializer<StringValue>
    {
        #region Symbols
        public const string SymbolNameDiaString = "dia-string";
        public const string SymbolNameNullString = "null-string";
        public const string SymbolNameSinglelineString = "singleline-string";
        public const string SymbolNameMultilineString = "multiline-string";
        #endregion


        private StringParser() { }

        public static string GrammarSymbol => SymbolNameDiaString;

        public static IResult<StringValue> Parse(CSTNode symbolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);
            context.ThrowIfDefault($"Invalid {nameof(context)} instance");

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                var (AddressIndexNode, AnnotationNode, ValueNode) = symbolNode.DeconstructValueNode();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                var result = ValueNode.SymbolName switch
                {
                    SymbolNameNullString => annotationResult.Map(StringValue.Null),
                    SymbolNameSinglelineString => ParseSingleline(ValueNode, annotationResult),
                    SymbolNameMultilineString => ParseMultiline(ValueNode, annotationResult),

                    _ => Result.Of<StringValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullString}', or '{SymbolNameSinglelineString}', etc"))
                };

                return AddressIndexNode is not null
                    ? result.Combine(
                        AddressIndexParser.Parse(AddressIndexNode),
                        (value, addressIndex) => value.RelocateValue(context.Track(addressIndex)))
                    : result;
            }
            catch (Exception e)
            {
                return Result.Of<StringValue>(e);
            }
        }

        #region Singleline parser
        public static IResult<StringValue> ParseSingleline(
            CSTNode singlelineSymbolNode,
            IResult<Annotation[]> annotationsResult)
        {
            var slstring = singlelineSymbolNode.TokenValue();
            var unescapedValue = EscapeSequenceGroup
                .SinglelineStringEscapeGroup
                .Unescape(slstring[1..^1]);

            return annotationsResult.Map(annotations => StringValue.Of(unescapedValue, annotations));
        }
        #endregion

        #region Multiline parser
        private static readonly Regex MultilinePrefixPattern = new Regex(
            "^@\"\\\\(?'alignment''(?'value'[1-9]\\d*)?(\n|\r\n))",
            RegexOptions.Compiled);

        public static IResult<StringValue> ParseMultiline(
            CSTNode multilineSymbolNode,
            IResult<Annotation[]> annotationsResult)
        {
            var mlstring = multilineSymbolNode.TokenValue();
            var alignmentMatch = MultilinePrefixPattern.Match(mlstring);
            ushort? alignment = alignmentMatch.Success switch
            {
                false => null,
                true => alignmentMatch.Groups["value"].Captures.Count switch
                {
                    0 => null,
                    _ => ushort.Parse(alignmentMatch.Groups["value"].Value)
                }
            };

            var unescapedValue = EscapeSequenceGroup
                .MultilineStringEscapeGroup(alignment)
                .Unescape($"\\\n{mlstring[alignmentMatch.Length..^1]}");

            return annotationsResult.Map(annotations => StringValue.Of(unescapedValue, annotations));
        }
        #endregion

        public static IResult<string> Serialize(StringValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var intOptions = context.Options.Ints;

            var addressIndexText = context.TryGetAddressIndex(value, out var index)
                ? $"#0x{index:x}"
                : "";

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var valueText = value.IsNull switch
            {
                true => Result.Of("null.string"),
                false => context.Options.Strings.LineStyle switch
                {
                    SerializerOptions.TextLineStyle.Singleline => EscapeSequenceGroup
                        .SinglelineStringEscapeGroup
                        .Escape(value.Value)
                        .WrapIn("\"")
                        .ApplyTo(Result.Of),

                    SerializerOptions.TextLineStyle.Multiline => StringParser
                        .FormatAsMultiline(value.Value, context.Options.Strings)
                        .JoinUsing("\n")
                        .WrapIn(
                            $"@\"\\'{context.Options.Strings.AlignmentIndentation}\n",
                            $"\\\n{"".PadLeft(context.Options.Strings.AlignmentIndentation)}\"")
                        .ApplyTo(Result.Of),

                    _ => Result.Of<string>(new ArgumentException(
                        $"Invalid {nameof(SerializerOptions.TextLineStyle)}: {context.Options.Strings.LineStyle}"))
                }
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{addressIndexText}{ann}{value}");
        }

        internal static string[] FormatAsMultiline(string? value, SerializerOptions.StringOptions options)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (string.Empty.Equals(value))
                return Array.Empty<string>();

            if (string.IsNullOrWhiteSpace(value))
                return new[] { EscapeSequenceGroup.SinglelineStringEscapeGroup.Escape(value)! };

            var reader = new WordGroupReader(value);
            var buffers = new List<StringBuilder> { new StringBuilder() };

            return reader
                .ReadAllWordGroups()
                .Resolve()
                .Aggregate(buffers, (lines, nextGroup) =>
                {
                    // words
                    AppendText(
                        buffers,
                        nextGroup.Word,
                        nextGroup.Value.Length,
                        options.MaxLineLength);

                    // whitespace lines
                    foreach(var wsgroup in nextGroup.WhitespaceGroups)
                    {
                        if (!IsWslineGroup(wsgroup))
                            AppendText(buffers, wsgroup, 0, options.MaxLineLength);

                        else if (wsgroup.Length > 0)
                        {
                            var lineCode = wsgroup
                                .Replace("\n\r", "l")
                                .Replace("\n", "n")
                                .Replace("\r", "r");

                            lineCode = "l".Equals(lineCode) ? "" : lineCode;
                            buffers[^1].Append($"\\^{lineCode}");
                            buffers.Add(new StringBuilder());
                        }
                    }

                    return buffers;
                })
                .Select(sb => sb
                    .ToString()
                    .PadLeft(options.AlignmentIndentation + sb.Length))
                .ToArray();
        }

        private static void AppendText(
            List<StringBuilder> buffers,
            string text,
            int textLength,
            int maxLineLength)
        {
            if ((buffers[^1].Length + textLength) >= maxLineLength)
            {
                if (buffers[^1].Length == 0)
                    buffers[^1].Append(text);

                else
                {
                    buffers[^1].Append('\\');
                    buffers.Add(new StringBuilder(text));
                }
            }
            else buffers[^1].Append(text);
        }

        internal static int LastLength(
            List<StringBuilder> builders)
            => builders?.LastOrDefault()?.Length ?? 0;

        internal static bool IsWslineGroup(string group)
        {
            foreach(var chr in group)
            {
                if (chr != '\n' && chr != '\r')
                    return false;
            }
            return true;
        }


        #region nested types
        internal record WordGroupReader
        {
            private static readonly HashSet<char> WhitespaceChars = new HashSet<char>
            {
                '\0',
                '\a',
                '\b',
                '\f',
                '\n',
                '\t',
                '\r',
                '\v',
                ' '
            };

            private readonly string @string;

            internal int Index { get; private set; } = 0;

            public WordGroupReader(string @string)
            {
                this.@string = @string;
            }

            public bool TryNextWordGroup(out IResult<WordGroup> result)
            {
                if (Index >= @string.Length)
                {
                    result = Result.Of<WordGroup>(new EndOfStreamException());
                    return false;
                }

                var isReadingWord = true;
                var wordBuffer = new StringBuilder();
                var wsBuffers = new List<StringBuilder> { new StringBuilder() };
                var index = Index;
                for (; index < @string.Length; index++)
                {
                    var @char = @string[index];

                    if (isReadingWord)
                    {
                        if (WhitespaceChars.Contains(@char))
                        {
                            isReadingWord = false;
                            index--;
                            continue;
                        }

                        wordBuffer.Append(@char);
                    }
                    else
                    {
                        if (!WhitespaceChars.Contains(@char))
                            break;

                        if (IsLineChar(@char))
                        {
                            if (wsBuffers[^1].Length == 0 || IsLineChar(wsBuffers[^1][^1]))
                                wsBuffers[^1].Append(@char);

                            else wsBuffers.Add(new StringBuilder(@char));
                        }
                        else
                        {
                            if (wsBuffers[^1].Length == 0 || !IsLineChar(wsBuffers[^1][^1]))
                                wsBuffers[^1].Append(@char);

                            else wsBuffers.Add(new StringBuilder(@char));
                        }
                    }
                }

                Index = index;
                result = Result.Of(new WordGroup(
                    wordBuffer.ToString(),
                    wsBuffers.Select(s => s.ToString()).ToArray()));

                return true;
            }

            public IResult<WordGroup[]> ReadAllWordGroups()
            {
                var results = new List<IResult<StringParser.WordGroup>>();
                while (TryNextWordGroup(out var wordGroup))
                    results.Add(wordGroup);

                return results
                    .Fold()
                    .Map(groups => groups.ToArray());
            }

            private static bool IsLineChar(char ch)
            {
                return ch == '\n' || ch == '\r';
            }
        }

        internal readonly struct WordGroup
        {
            public string Word { get; }

            public string[] WhitespaceGroups { get; }

            public string Value => $"{Word}{WhitespaceGroups.JoinUsing("")}";

            public WordGroup(string word, string[] whitespaceGroups)
            {
                ArgumentNullException.ThrowIfNull(word);
                ArgumentNullException.ThrowIfNull(whitespaceGroups);

                Word = word;
                WhitespaceGroups = whitespaceGroups;
            }


            public override string ToString()
            {
                return $"Word: '{Word}', Wihitespace: [{Escape(WhitespaceGroups.JoinUsing(","))}]";
            }

            private static string Escape(string spaceline)
            {
                return spaceline
                    .Replace(" ", "\\s")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");
            }
        }
        #endregion
    }
}
