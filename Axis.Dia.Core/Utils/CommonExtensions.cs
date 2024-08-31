using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Utils
{
    internal static class CommonExtensions
    {
        //private static readonly Regex UnicodeControlCharacterPattern = new Regex("\\p{C}", RegexOptions.Compiled);

        ///
        /// Use <see cref="Axis.Luna.Common.StringEscape.CommonStringEscaper"/> to un/escape the character
        ////internal static string EscapeUnicodeControlCharacter(this char c)
        ////{
        ////    return c switch
        ////    {
        ////        '\0' => "\\0",
        ////        '\a' => "\\a",
        ////        '\b' => "\\b",
        ////        '\f' => "\\f",
        ////        '\n' => "\\n",
        ////        '\r' => "\\r",
        ////        '\t' => "\\t",
        ////        '\v' => "\\v",
        ////        _ => (UnicodeControlCharacterPattern.IsMatch(c.ToString()), c <= byte.MaxValue) switch
        ////        {
        ////            (true, true) => $"\\x{(int)c:x2}",
        ////            (true, false) => $"\\u{(int)c:x4}",
        ////            (_, _) => c.ToString()
        ////        }
        ////    };
        ////}
    }
}
