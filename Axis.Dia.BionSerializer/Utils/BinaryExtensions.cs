using Axis.Dia.BionSerializer.Metadata;
using Axis.Luna.BitSequence;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Utils
{
    public static class BinaryExtensions
    {
        private static readonly byte[] ByteMasks = [
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128
        ];


        internal static bool IsSet(this byte @byte, int bitIndex) => (@byte & ByteMasks[bitIndex]) == ByteMasks[bitIndex];

        internal static BitSequence ConcatenateBits(this BitSequence bits, params byte[] bytes)
        {
            return bits.Concat(BitSequence.Of(bytes));
        }

        internal static BitSequence ConcatenateBits(this BitSequence bits, params bool[] addendum)
        {
            return bits.Concat(BitSequence.Of(addendum));
        }

        internal static BitSequence ConcatenateBit(this BitSequence bits, bool bit)
        {
            return bits.Concat(BitSequence.Of(bit));
        }

        /// <summary>
        /// Attempts to convert an array of custom metadata into a <see cref="VarBytes"/> instance.
        /// </summary>
        /// <param name="customMetadata">the array of custom metadata</param>
        /// <returns>the varbytes</returns>
        internal static VarBytes ToVarBytes(this CustomMetadata[] customMetadata)
        {
            ArgumentNullException.ThrowIfNull(customMetadata);

            return customMetadata
                .Select(cmeta => cmeta.RawByteValue)
                .ApplyTo(bytes => VarBytes.Of(bytes, false));
        }

        /// <summary>
        /// Converts the <paramref name="customMetadata"/> to a <see cref="VarBytes"/> instance, removes overflows, and converts
        /// finally to a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="customMetadata">The custom metadata array</param>
        /// <param name="isUnsigned">indicates how the <see cref="BigInteger"/> should intepret the bytes used in it's creation.</param>
        internal static BigInteger ToBigInteger(this CustomMetadata[] customMetadata, bool isUnsigned = true)
        {
            ArgumentNullException.ThrowIfNull(customMetadata);

            return customMetadata
                .ToVarBytes()
                .ToBigInteger(isUnsigned);
        }

        /// <summary>
        /// Converts the <paramref name="customMetadata"/> to a <see cref="VarBytes"/> instance, removes overflows, and converts
        /// finally to a <see cref="BigInteger"/>.
        /// <para>
        ///     Note that this range only takes [D1..D7] of each custom metadata instance into consideration. i.e, a range of [2..12] is interpreted as:
        /// <code>
        /// customMetadata[0][D2]  
        /// customMetadata[0][D3]  
        /// customMetadata[0][D4]  
        /// customMetadata[0][D5]  
        /// customMetadata[0][D6]  
        /// customMetadata[0][D7]  
        /// customMetadata[1][D1]  
        /// customMetadata[1][D2]  
        /// customMetadata[1][D3]  
        /// customMetadata[1][D4]  
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="customMetadata">The custom metadata array</param>
        /// <param name="bitRange">
        ///     Designates a range of bits culled from the <paramref name="customMetadata"/> array, used in the conversion to <see cref="BigInteger"/>
        /// </param>
        /// <param name="isUnsigned">indicates how the <see cref="BigInteger"/> should intepret the bytes used in it's creation.</param>
        internal static BigInteger ToBigInteger(
            this CustomMetadata[] customMetadata,
            Range bitRange,
            bool isUnsigned = true)
        {
            ArgumentNullException.ThrowIfNull(customMetadata);

            return customMetadata
                .ToBitSequence()
                .ToByteArray(bitRange)
                .ApplyTo(bytes => new BigInteger(bytes, isUnsigned));
        }

        internal static BitSequence ToBitSequence(
            this CustomMetadata[] customMetadata)
            => customMetadata.ToBitSequence(..);

        internal static BitSequence ToBitSequence(
            this CustomMetadata[] customMetadata,
            Range bitRange)
        {
            ArgumentNullException.ThrowIfNull(customMetadata);

            return customMetadata
                .ToVarBytes()
                .ToRawByteArray()
                .ApplyTo(BitSequence.Of)
                [bitRange];
        }

        internal static CustomMetadata[] ToCustomMetadata(this VarBytes varbytes)
        {
            return varbytes
                .Select(CustomMetadata.Of)
                .ToArray();
        }

        internal static CustomMetadata[] ToCustomMetadata(this BigInteger unsignedInteger)
        {
            return unsignedInteger
                .ToVarBytes()
                .ToCustomMetadata();
        }

        /// <summary>
        /// Converts the given bit sequence to a <see cref="CustomMetadata"/> array
        /// </summary>
        /// <param name="bitSeqeunce">The bit sequence</param>
        /// <param name="isRawBits">
        ///     indicates if the <paramref name="bitSeqeunce"/> represents "raw" bits, or bits already in
        ///     the <see cref="VarBytes"/> format.
        /// </param>
        /// <returns>The <see cref="CustomMetadata"/> array</returns>
        internal static CustomMetadata[] ToCustomMetadata(
            this BitSequence bitSeqeunce,
            bool isRawBits = true)
        {
            return bitSeqeunce
                .ToByteArray()
                .ApplyTo(array => VarBytes.Of(array, isRawBits))
                .ToCustomMetadata();
        }

        internal static VarBytes ToVarBytes(this BigInteger integer, bool useSignificantBits = true)
        {
            var rawByteArray = integer.ToByteArray();

            return useSignificantBits
                ? BitSequence
                    .Of(rawByteArray).SignificantBits
                    .ApplyTo(VarBytes.Of)
                : BitSequence
                    .Of(rawByteArray)
                    .ApplyTo(VarBytes.Of);
        }

        internal static long GetSignificantBitLength(this BigInteger integer, out bool isPositive)
        {
            isPositive = BigInteger.IsPositive(integer);

            return !isPositive
                ? integer.GetByteCount() * 8
                : integer.GetBitLength();
        }

        internal static BitSequence SignificantBits(this BitSequence sequence)
        {
            if (sequence.IsDefault)
                return sequence;

            int index = sequence.Length - 1;
            for (; index >= 0; index--)
            {
                if (sequence[index])
                    break;
            }

            if (index < 0)
                return sequence;

            return sequence[..(index + 1)];
        }

        internal static byte[] ToRawByteArray(this string @string)
        {
            ArgumentNullException.ThrowIfNull(@string);

            return @string
                .Select(BitConverter.GetBytes)
                .SelectMany()
                .ToArray();
        }

        internal static string StringFromRawBytes(this byte[] bytes)
        {
            return bytes
                .Batch(2)
                .Select(duo => BitConverter.ToChar(duo.ToArray()))
                .ApplyTo(chars => new string(chars.ToArray()));
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
