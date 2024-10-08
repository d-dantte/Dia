using Axis.Dia.BionSerializer.Utils;
using Axis.Luna.BitSequence;
using Axis.Luna.Common;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.BionSerializer.Metadata
{
    public readonly struct CustomMetadata : IDefaultValueProvider<CustomMetadata>
    {
        private readonly byte data;

        #region Properties
        /// <summary>
        /// Gets the byte value of the first 7 bits. The 8th bit is reserved for overflow
        /// </summary>
        public byte DataByteValue => (byte)(data & 127);

        /// <summary>
        /// Gets the raw byte value, i.e, all 8 bits.
        /// </summary>
        public byte RawByteValue => data;

        /// <summary>
        /// Gets the <see cref="MetadataFlags"/> representation of this instance.
        /// </summary>
        public MetadataFlags FlagValue
        {
            get
            {
                byte metadataByte = data;
                MetadataFlags flags =
                    (metadataByte.IsSet(0) ? MetadataFlags.D1 : MetadataFlags.None)
                    | (metadataByte.IsSet(1) ? MetadataFlags.D2 : MetadataFlags.None)
                    | (metadataByte.IsSet(2) ? MetadataFlags.D3 : MetadataFlags.None)
                    | (metadataByte.IsSet(3) ? MetadataFlags.D4 : MetadataFlags.None)
                    | (metadataByte.IsSet(4) ? MetadataFlags.D5 : MetadataFlags.None)
                    | (metadataByte.IsSet(5) ? MetadataFlags.D6 : MetadataFlags.None)
                    | (metadataByte.IsSet(6) ? MetadataFlags.D7 : MetadataFlags.None)
                    | (metadataByte.IsSet(7) ? MetadataFlags.Overflow : MetadataFlags.None);

                return flags;
            }
        }

        public bool HasOverflow => (FlagValue & MetadataFlags.Overflow) == MetadataFlags.Overflow;
        #endregion

        #region Construction
        public CustomMetadata(byte data)
        {
            this.data = data;
        }

        public static CustomMetadata Of(byte data) => new(data);

        public static CustomMetadata Of(sbyte data) => new((byte)data);

        public static CustomMetadata Of(MetadataFlags flags) => new((byte)flags);

        public static implicit operator CustomMetadata(byte data) => Of(data);

        public static implicit operator CustomMetadata(sbyte data) => Of(data);

        public static explicit operator byte(CustomMetadata data) => data.RawByteValue;

        public static explicit operator sbyte(CustomMetadata data) => (sbyte)data.RawByteValue;
        #endregion

        #region Default provider
        public static CustomMetadata Default => default;

        public bool IsDefault => data == default;
        #endregion

        #region API

        /// <summary>
        /// Checks if the given flags are set
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool IsSet(MetadataFlags flags) => (FlagValue & flags) == flags;
        #endregion

        #region Overrides
        public override string ToString() => BitSequence.Of(data).ToString();

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is CustomMetadata other
                && data == other.data;
        }

        public override int GetHashCode() => data;

        public static bool operator ==(CustomMetadata left, CustomMetadata right) => left.Equals(right);

        public static bool operator !=(CustomMetadata left, CustomMetadata right) => !left.Equals(right);
        #endregion

        #region Nested types

        [Flags]
        public enum MetadataFlags
        {
            None = (byte)0,

            D1 = 1,  // bit at index 0
            D2 = 2,  // bit at index 1
            D3 = 4,  // bit at index 2
            D4 = 8,  // bit at index 3
            D5 = 16, // bit at index 4
            D6 = 32, // bit at index 5
            D7 = 64, // bit at index 6

            /// <summary>
            /// Indicates that a custom metadata byte follows this byte
            /// </summary>
            Overflow = 128,
        }
        #endregion
    }
}
