using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar;
using System.Numerics;

namespace Axis.Dia.Utils
{
    internal static class Extensions
    {
        public static ReferenceValue ToRef<TDiaValue>(this IDiaAddressable<TDiaValue> addressable)
        where TDiaValue: IDiaAddressable<TDiaValue>, IDiaValue
        {
            return ReferenceValue.Of(addressable);
        }

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

        internal static TItem[] JoinWith<TItem>(this TItem[] first, TItem[] second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);

            var combinedArray = new TItem[first.Length + second.Length];

            Array.Copy(first, 0, combinedArray, 0, first.Length);
            Array.Copy(second, 0, combinedArray, first.Length, second.Length);

            return combinedArray;
        }

        internal static IEnumerable<TOut> Repeat<TOut>(
            this BigInteger count,
            Func<BigInteger, TOut> func)
        {
            ArgumentNullException.ThrowIfNull(func);

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            BigInteger index = BigInteger.Zero;
            while (index++ < count)
            {
                yield return func.Invoke(index);
            }
        }

        internal static string Reverse(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)
                || value.Length == 1)
                return value;

            else
                return value
                    .ToCharArray()
                    .With(Array.Reverse)
                    .ApplyTo(array => new string(array));
        }

        internal static char Peek(this BufferedTokenReader reader)
        {
            if (!reader.TryNextToken(out var token))
                throw new EndOfStreamException();

            reader.Back();
            return token;
        }

        internal static TItem FirstOrThrow<TItem>(this
            IEnumerable<TItem> items,
            Func<Exception> exceptionFactory)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(exceptionFactory);

            if (items.Any())
                return items.First();

            else return exceptionFactory
                .Invoke()
                .Throw<TItem>();
        }

        internal static IResult<T3> Combine<T1, T2, T3>(this
            IResult<T1> t1Result,
            IResult<T2> t2Result,
            Func<T1, T2, T3> combiner)
        {
            ArgumentNullException.ThrowIfNull(t1Result);
            ArgumentNullException.ThrowIfNull(t2Result);
            ArgumentNullException.ThrowIfNull(combiner);

            return Result
                .Of(() => (t1Result.Resolve(), t2Result.Resolve()))
                .Map(tuple => combiner.Invoke(tuple.Item1, tuple.Item2));
        }
    }
}
