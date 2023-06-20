using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.IO.Binary
{
    public static class BinaryExtensions
    {
        private static readonly byte[] ByteMasks = new byte[]
        {
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128
        };


        internal static bool IsSet(this byte @byte, int bitIndex) => (@byte & ByteMasks[bitIndex]) == ByteMasks[bitIndex];

        internal static BitSequence AppendBits(this BitSequence bits, params byte[] bytes)
        {
            return bits
                .Concat(BitSequence.Of(bytes))
                .ApplyTo(BitSequence.Of);
        }

        internal static BitSequence AppendBits(this BitSequence bits, params bool[] addendum)
        {
            return bits
                .Concat(BitSequence.Of(addendum))
                .ApplyTo(BitSequence.Of);
        }

        /// <summary>
        /// Attempts to convert an array of custom metadata into a <see cref="VarBytes"/> instance.
        /// </summary>
        /// <param name="metadataArray">the array of custom metadata</param>
        /// <returns>the varbytes</returns>
        internal static VarBytes ToVarBytes(this CustomMetadata[] metadataArray)
        {
            ArgumentNullException.ThrowIfNull(metadataArray);

            return metadataArray
                .Select(cmeta => cmeta.RawByteValue)
                .ToArray()
                .ApplyTo(VarBytes.Of);
        }


        internal static BigInteger CustomMetadataAsInt(this TypeMetadata typeMetadata)
        {
            return typeMetadata.CustomMetadata
                .ToVarBytes()
                .ToByteArray()!
                .ApplyTo(bytes => new BigInteger(bytes));
        }
    }
}
