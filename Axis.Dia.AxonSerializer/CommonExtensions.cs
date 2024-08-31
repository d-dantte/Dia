using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Axis.Dia.Axon
{
    internal static class CommonExtensions
    {
        private static readonly Regex UnicodeControlCharacterPattern = new Regex("\\p{C}", RegexOptions.Compiled);

        internal static bool IsNegative(this
            BigInteger @int)
            => @int < 0;

        internal static IEnumerable<(IEnumerable<TItem> Batch, int BatchIndex)> BatchGroup<TItem>(this
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

        internal static IEnumerable<IEnumerable<TItem>> Batch<TItem>(this
            IEnumerable<TItem> items,
            int batchSize)
            => items.BatchGroup(batchSize).Select(group => group.Batch);

        internal static string JoinUsing(this
            IEnumerable<char> chars,
            string delimiter = "")
        {
            ArgumentNullException.ThrowIfNull(chars);

            return chars
                .Select(c => c.ToString())
                .JoinUsing(delimiter);
        }

        internal static string EscapeUnicodeControlCharacter(this char c)
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

        internal static IEnumerable<TItem> SelectMany<TItem>(this
            IEnumerable<IEnumerable<TItem>> itemBatches)
            => itemBatches.SelectMany(batch => batch);

        internal static string EscapeUnicodeControlCharacters(this string @string)
        {
            return @string
                .Select(EscapeUnicodeControlCharacter)
                .SelectMany()
                .ApplyTo(chars => new string(chars.ToArray()));
        }

        internal static string Reverse(this string @string)
        {
            if (@string is null)
                return null!;

            char[] charArray = @string.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
