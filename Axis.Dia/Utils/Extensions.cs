using Axis.Luna.Extensions;

namespace Axis.Dia.Utils
{
    public static class Extensions
    {
        internal static bool IsNull<TValue>(this TValue value) => value is null;

        internal static bool NullOrTrue<TValue>(
            TValue? lhs,
            TValue? rhs,
            Func<TValue, TValue, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            if (lhs is not null && rhs is not null)
                return predicate.Invoke(lhs!, rhs!);

            if (lhs is null && rhs is null)
                return true;

            return false;
        }

        internal static bool SetEquals<TItem>(IEnumerable<TItem> first, IEnumerable<TItem> second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            var set = first as HashSet<TItem> ?? new HashSet<TItem>(first);
            var count = 0;
            return second
                .Distinct()
                .All(item =>
                {
                    count++;
                    return set.Contains(item);
                })
                && count == set.Count;
        }

        /// <summary>
        /// Skips every "nth" item in the list. example:
        /// <code>
        /// var arr = [0,0,0,1,0,0,0,1,0,0]
        /// var result = SkipEveryNth(arr, 4); // skips every 4th item
        /// Console.WriteLine(result); // yieilds: [0,0,0,0,0,0,0,0]
        /// </code>
        /// </summary>
        /// <typeparam name="TItem">the type of the sequence</typeparam>
        /// <param name="items">the sequence</param>
        /// <param name="skipPosition">the nth position to skip</param>
        /// <returns>a new sequence with the items skipped</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static IEnumerable<TItem> SkipEveryNth<TItem>(this IEnumerable<TItem> items, int skipPosition)
        {
            ArgumentNullException.ThrowIfNull(items);

            if (skipPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(skipPosition));

            if (skipPosition == 1)
                yield break;

            var trigger = skipPosition - 1;
            using var enumerator = items.GetEnumerator();
            for (int index = 0; enumerator.MoveNext(); index++)
            {
                if (index % skipPosition == trigger)
                    continue;

                yield return enumerator.Current;
            }
        }

        internal static IEnumerable<TItem> JoinUsing<TItem>(
            this IEnumerable<IEnumerable<TItem>> items,
            params TItem[] delimiter)
        {
            return items
                .Join(delimiter)
                .SelectMany();
        }

        private static IEnumerable<IEnumerable<TItem>> Join<TItem>(
            this IEnumerable<IEnumerable<TItem>> items,
            params TItem[] delimiter)
        {
            using var enumerator = items.GetEnumerator();

            if (enumerator.MoveNext())
                yield return enumerator.Current;

            while (enumerator.MoveNext())
            {
                yield return delimiter;
                yield return enumerator.Current;
            }
        }

        public static TItem[] JoinWith<TItem>(this TItem[] first, TItem[] second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            var combinedArray = new TItem[first.Length + second.Length];

            Buffer.BlockCopy(first, 0, combinedArray, 0, first.Length);
            Buffer.BlockCopy(second, 0, combinedArray, first.Length, second.Length);

            return combinedArray;
        }
    }
}
