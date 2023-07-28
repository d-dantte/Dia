using Axis.Dia.Utils.EscapeSequences;

namespace Axis.Dia.Utils
{
    /// <summary>
    /// Defines a group of characters and their respective escape sequences, and also provides APIs for converting and replacing
    /// these within strings.
    /// </summary>
    public abstract class EscapeSequenceGroup
    {
        /// <summary>
        /// Name of the escape sequence group
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Applies escape to the characters in the string. e.g
        /// <code>
        /// // raw string
        /// "pi symbol is π"
        /// 
        /// // after escaping
        /// "pi symbol is \u03c0"
        /// </code>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract string? Escape(string? value);

        /// <summary>
        /// Replaces escape sequences with their actual characters. e.g
        /// <code>  
        /// // escaped string
        /// "pi symbol is \u03c0"
        /// 
        /// // after unescaping
        /// "pi symbol is π"
        /// </code>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract string? Unescape(string? value);


        private static readonly EscapeSequenceGroup _symbolEscape = new SymbolEscapeSequenceGroup();
        private static readonly EscapeSequenceGroup _stringEscape = new SinglelineStringEscapeSequenceGroup();

        /// <summary>
        /// 
        /// </summary>
        public static EscapeSequenceGroup SymbolEscapeGroup => _symbolEscape;

        /// <summary>
        /// 
        /// </summary>
        public static EscapeSequenceGroup SinglelineStringEscapeGroup => _stringEscape;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alignmentSpaceCount"></param>
        /// <returns></returns>
        public static EscapeSequenceGroup MultilineStringEscapeGroup(
            ushort? alignmentSpaceCount = null)
            => new MultilineStringEscapeSequenceGroup(alignmentSpaceCount);
    }
}
