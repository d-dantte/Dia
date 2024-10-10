using Axis.Dia.Bion.Utils;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Bion
{
    internal static class Extensions
    {
        public static VarBytes ToVarBytes(this int value, bool useSignificantBits = true)
        {
            return value
                .As<BigInteger>()
                .ToVarBytes(useSignificantBits);
        }

        public static VarBytes ToVarBytes(this long value, bool useSignificantBits = true)
        {
            return value
                .As<BigInteger>()
                .ToVarBytes(useSignificantBits);
        }

        /// <summary>
        /// Skips every nth value, where n = <paramref name="skipCount"/>.
        /// <para/>
        /// NOTE: <paramref name="skipCount"/> is not an index, it is a count. So skipping every 4th value will skip indexes <c>3, 7, 11,...</c> etc.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="skipCount">The skip count</param>
        /// <returns>Returns a new enumerable devoid of the skipped elements</returns>
        internal static IEnumerable<T> SkipEvery<T>(this IEnumerable<T> source, int skipCount)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (skipCount < 1)
                throw new ArgumentOutOfRangeException(nameof(skipCount));

            return source.Where((_, index) => (index + 1) % skipCount != 0);
        }

        internal static ReadOnlySpan<TItem> AsReadOnlySpan<TItem>(this TItem[] array)
        {
            return new(array);
        }
    }
}
