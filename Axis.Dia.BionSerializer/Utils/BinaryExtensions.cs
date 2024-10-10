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

        internal static BigInteger ToBigInteger(this BitSequence sequence, bool isUnsigned = true)
        {
            var bytes = sequence.ToByteArray();
            return new BigInteger(bytes, isUnsigned);
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
    }
}
