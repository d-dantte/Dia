using Axis.Dia.Utils;
using System.Text.RegularExpressions;

namespace Axis.Dia.Types
{
    public record Annotation
    {
        private static readonly Regex AttributePattern = new(
            "^(?'key'[a-zA-Z_](([.-])?[a-zA-Z0-9_])*):(?'value'.+)\\z",
            RegexOptions.Compiled);

        public SymbolValue Symbol { get; }

        /// <summary>
        /// Indicates if this annotation conforms to the symbol's Identifier pattern
        /// </summary>
        public bool IsIdentifier => Symbol.IsIdentifier;

        /// <summary>
        /// Indicates if this symbol conforms to the attribute pattern <c>key:value</c>
        /// </summary>
        public bool IsAttribute => AttributePattern.IsMatch(Symbol.Value!);


        public Annotation(SymbolValue symbol)
        {
            Symbol = symbol
                .ThrowIf(
                    s => s.IsNull,
                    _ => new ArgumentException($"Invalid symbol: null value"))
                .ThrowIf(
                    s => s.Annotations.Length > 0,
                    _ => new ArgumentException("Invalid symbol: annotation symbol cannot be annotated"));
        }

        public static Annotation Of(SymbolValue symbol) => new Annotation(symbol);

        public static Annotation Of(string symbol) => new Annotation(symbol);

        public static Annotation[] Of(
            params string[] symbols)
            => symbols.Select(Of).ToArray();


        public static implicit operator Annotation(SymbolValue symbol) => new Annotation(symbol);
        public static implicit operator Annotation(string symbol) => new Annotation(symbol);


        /// <summary>
        /// Returns this annotation, deconstructed into it's attribute form, if it indeed is an attribute.
        /// </summary>
        /// <param name="attribute">the output attribute</param>
        /// <returns>true if this is an attribute and the deconstruction is successful, false otherwise.</returns>
        public bool TryGetAttribute(out KeyValuePair<string, string> attribute)
        {
            var kvpMatch = AttributePattern.Match(Symbol.Value!);
            if (kvpMatch.Success)
            {
                attribute = KeyValuePair.Create(
                    kvpMatch.Groups["key"].Value,
                    EscapeSequenceGroup.SymbolEscapeGroup.Unescape(kvpMatch.Groups["value"].Value)!);
                return true;
            }

            attribute = default;
            return false;
        }

        /// <summary>
        /// Returns this symbol's value if it comforms to the Identifier pattern, otherwise, returns null
        /// </summary>
        /// <param name="identifier">the output identifier if the pattern matches</param>
        /// <returns>True if this is an identifier, false otherwise</returns>
        public bool TryGetIdentifier(out string identifier) => Symbol.TryGetIdentifier(out identifier);
    }
}
