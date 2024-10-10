using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Luna.Extensions;
using Axis.Luna.Optional;
using Axis.Pulsar.Core.CST;
using Axis.Luna.Common.StringEscape;
using System.Text.RegularExpressions;

namespace Axis.Dia.Axon.Deserializers
{
    /// <summary>
    /// 
    /// </summary>
    public class StringDeserializer : IValueDeserializer<Core.Types.String>
    {
        public static readonly string ExceptionData = "RecognizerError";

        private static readonly string Query = $"{Symbol_MLString}|{Symbol_SLString}";
        private static readonly CommonStringEscaper Escaper = new();
        private static readonly Regex EscapedWhitespacePattern = new("\\\\\\s+");

        #region Symbol
        private const string Symbol_AttributeList = "attribute-list";
        private const string Symbol_String = "dia-string";
        private const string Symbol_Null = "null-string";
        private const string Symbol_SLString = "singleline-string";
        private const string Symbol_SLStringSegment = "singleline-string-segment";
        private const string Symbol_MLString = "multiline-string";
        #endregion

        public static Core.Types.String Deserialize(string text, DeserializerContext? context = null)
        {
            var recognizer = GrammarUtil.LanguageContext.Grammar[Symbol_String] ??
                throw new InvalidOperationException($"Invalid grammar symbol: {Symbol_String}");

            if (!recognizer.TryRecognize(text, "root", GrammarUtil.LanguageContext, out var result))
            {
                var exception = new FormatException($"Invalid string format: {text}");
                exception.Data[ExceptionData] = result;
                throw exception;
            }

            return result.Map<ISymbolNode, Core.Types.String>(Deserialize);
        }

        internal static Core.Types.String Deserialize(ISymbolNode stringNode)
        {
            stringNode = stringNode
                .ThrowIfNull(() => new ArgumentNullException(nameof(stringNode)))
                .ThrowIf(
                    node => !Symbol_String.Equals(node.Symbol),
                    node => new ArgumentException($"Invalid node symbol: {node.Symbol}"));

            var attributes = stringNode
                .FindNodes(Symbol_AttributeList)
                .Select(AttributeDeserializer.Deserialize)
                .FirstOrOptional()
                .ValueOr([]);

            if (stringNode.TryFindNodes(Symbol_Null, out var nodes))
                return Core.Types.String.Null(attributes!);

            return stringNode
                .FindNodes(Query)
                .Select(n => n.Symbol switch
                {
                    Symbol_MLString => n
                        .Tokens[1..^1]
                        .ToString()!
                        .ApplyTo(UnescapeMultilineEscapeSequences),

                    //Symbol_SLString/Symbol_SLStringSegment
                    _ => n
                        .FindNodes(Symbol_SLStringSegment)
                        .Select(n => n.Tokens[1..^1])
                        .Select(t => t.ToString())
                        .JoinUsing("")
                })
                .First()
                .ApplyTo(Escaper.UnescapeString)
                .ApplyTo(str => Core.Types.String.Of(str, attributes?.ToArray() ?? []));
        }

        internal static string UnescapeMultilineEscapeSequences(string escapedString)
        {
            return EscapedWhitespacePattern
                .Replace(escapedString, string.Empty)
                .Replace("\\`", "`");
        }
    }
}
