using System.Collections.Immutable;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Axis.Dia.Core.Utils
{
    internal static class CommonExtensions
    {
        private static readonly Regex UnicodeControlCharacterPattern = new Regex("\\p{C}", RegexOptions.Compiled);

        public static TOut ApplyTo<TIn, TOut>(this
            TIn @in,
            Func<TIn, TOut> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);

            return mapper.Invoke(@in);
        }

        //public static bool AttributesSetEquals(this
        //    ImmutableArray<Types.Attribute> first,
        //    ImmutableArray<Types.Attribute> second)
        //{
        //    if (first.IsDefault && second.IsDefault)
        //        return true;

        //    if (first.IsDefault ^ second.IsDefault)
        //        return false;

        //    if (first.Length != second.Length)
        //        return false;

        //    var firstSet = first.ToImmutableHashSet();
        //    return second.All(firstSet.Contains);
        //}

        public static bool IsNegative(this
            BigInteger @int)
            => @int < 0;

        public static IEnumerable<(IEnumerable<TItem> Batch, int BatchIndex)> Batch<TItem>(this
            IEnumerable<TItem> items,
            int batchSize)
        {
            ArgumentNullException.ThrowIfNull(items);

            if (batchSize < 1)
                throw new ArgumentOutOfRangeException(nameof(batchSize));

            int count = 0;
            int index = 0;
            var batch = new List<TItem>();

            foreach (var value in items)
            {
                batch.Add(value);

                if (++count % batchSize == 0)
                {
                    yield return (batch, index++);

                    batch = new List<TItem>();
                }
            }

            if (batch.Count > 0)
                yield return (batch, index++);
        }

        public static string JoinUsing(this
            IEnumerable<string> strings,
            string delimiter = "")
        {
            ArgumentNullException.ThrowIfNull(strings);
            ArgumentNullException.ThrowIfNull(delimiter);

            return string.Join(delimiter, strings);
        }

        public static string JoinUsing(this
            IEnumerable<char> chars,
            string delimiter = "")
        {
            ArgumentNullException.ThrowIfNull(chars);

            return chars
                .Select(c => c.ToString())
                .JoinUsing(delimiter);
        }

        public static string EscapeUnicodeControlCharacter(this char c)
        {
            return c switch
            {
                '\0' => "\\0",
                '\a' => "\\a",
                '\b' => "\\b",
                '\f' => "\\f",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                '\v' => "\\v",
                _ => (UnicodeControlCharacterPattern.IsMatch(c.ToString()), c <= byte.MaxValue) switch
                {
                    (true, true) => $"\\x{(int)c:x2}",
                    (true, false) => $"\\u{(int)c:x4}",
                    (_, _) => c.ToString()
                }
            };
        }

        public static IEnumerable<TItem> SelectMany<TItem>(this
            IEnumerable<IEnumerable<TItem>> itemBatches)
            => itemBatches.SelectMany(batch => batch);

        public static string EscapeUnicodeControlCharacters(this string @string)
        {
            return @string
                .Select(EscapeUnicodeControlCharacter)
                .SelectMany()
                .ApplyTo(chars => new string(chars.ToArray()));
        }

        public static string Reverse(this string @string)
        {
            if (@string is null)
                return null!;

            char[] charArray = @string.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
