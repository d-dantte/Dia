using Axis.Dia.Contracts;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.Convert.Binary.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct TypeMetadata : IDefaultValueProvider<TypeMetadata>
    {
        private static readonly byte TypeMask = 0xF;

        #region Fields
        private readonly VarBytes metadata;
        #endregion

        #region Construction
        public TypeMetadata(byte typeMetadata, params CustomMetadata[] customMetadata)
        {
            ArgumentNullException.ThrowIfNull(customMetadata);

            var lastIndex = customMetadata.Length - 1;
            metadata = customMetadata
                .Select((cmeta, index) => cmeta.DataByteValue | OverflowFlag(() => index < lastIndex))
                .InsertAt(0, typeMetadata | OverflowFlag(() => customMetadata.Length > 0))
                .Select(i => (byte)i)
                .ToArray()
                .ApplyTo(bytes => VarBytes.Of(bytes, false));
        }

        public TypeMetadata(VarBytes metadata)
        {
            if (metadata.Length <= 0)
                throw new ArgumentException($"Invalid metadata: {metadata}");

            this.metadata = metadata;
        }

        public static TypeMetadata Of(VarBytes metadata) => new TypeMetadata(metadata);

        public static TypeMetadata Of(byte[] varbytes) => Of(VarBytes.Of(varbytes, false));

        public static TypeMetadata Of(
            byte typeMetadata,
            params CustomMetadata[] customMetadata)
            => new TypeMetadata(typeMetadata, customMetadata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeMetadata"></param>
        /// <param name="dataCount">unsigned BigInt</param>
        /// <returns></returns>
        public static TypeMetadata Of(
            byte typeMetadata,
            BigInteger? dataCount)
            => new TypeMetadata(typeMetadata, dataCount?.ToCustomMetadata() ?? Array.Empty<CustomMetadata>());

        public static TypeMetadata Of(
            DiaType type,
            MetadataFlags flags,
            params CustomMetadata[] customMetadata)
            => new TypeMetadata(ToMetadataByte(type, flags), customMetadata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="dataCount">unsigned BigInt</param>
        /// <returns></returns>
        public static TypeMetadata Of(
            DiaType type,
            MetadataFlags flags,
            BigInteger? dataCount)
            => new TypeMetadata(
                ToMetadataByte(type, flags),
                dataCount?.ToCustomMetadata() ?? Array.Empty<CustomMetadata>());

        public static implicit operator TypeMetadata(VarBytes metadata) => new TypeMetadata(metadata);
        #endregion

        #region Properties

        /// <summary>
        /// The type represented by the type metadata. If the type is unknown (if this is a default instance, e.g), 
        /// the value is set to <c>0</c>, which is undefined on the enum.
        /// </summary>
        public DiaType Type => (DiaType)(IsDefault ? 0 : metadata[0] & TypeMask);

        /// <summary>
        /// Checks that the <see cref="DiaType"/> represented by the metadata byte is defined on the enum.
        /// </summary>
        public bool IsValidateDiaType => !Enum.IsDefined(Type);

        /// <summary>
        /// The metadata
        /// </summary>
        public VarBytes Metadata => metadata;

        /// <summary>
        /// Indicates if the annotation flag is set
        /// </summary>
        public bool IsAnnotated => !IsDefault && ToFlags().HasFlag(MetadataFlags.Annotated);

        /// <summary>
        /// Indicates if the null flag is set
        /// </summary>
        public bool IsNull => !IsDefault && ToFlags().HasFlag(MetadataFlags.Null);

        /// <summary>
        /// Indicates if the custom flag is set
        /// </summary>
        public bool IsCustomFlagSet => !IsDefault && ToFlags().HasFlag(MetadataFlags.Custom);

        /// <summary>
        /// Indicates if the overflow flag is set
        /// </summary>
        public bool IsOverflowFlagSet => !IsDefault && ToFlags().HasFlag(MetadataFlags.Overflow);

        /// <summary>
        /// The number of <see cref="CustomMetadata"/> instances present.
        /// </summary>
        public int CustomMetadataCount => IsDefault ? 0 : metadata.Length - 1;

        /// <summary>
        /// Gets an array of the contained custom metadata intances, or an empty array if non are present.
        /// </summary>
        public CustomMetadata[] CustomMetadata
            => CustomMetadataCount <= 0
                ? Array.Empty<CustomMetadata>()
                : metadata[1..]
                    .Select(Binary.Metadata.CustomMetadata.Of)
                    .ToArray();
        #endregion

        #region DefaultProvider
        /// <summary>
        /// Indicates if this is the default value for this type
        /// </summary>
        public bool IsDefault => metadata.IsDefault;

        public static TypeMetadata Default => default;
        #endregion

        #region Misc
        private MetadataFlags ToFlags()
        {
            byte metadataByte = metadata[0];
            MetadataFlags flags =
                (metadataByte.IsSet(4) ? MetadataFlags.Annotated : MetadataFlags.None)
                | (metadataByte.IsSet(5) ? MetadataFlags.Null : MetadataFlags.None)
                | (metadataByte.IsSet(6) ? MetadataFlags.Custom : MetadataFlags.None)
                | (metadataByte.IsSet(7) ? MetadataFlags.Overflow : MetadataFlags.None);

            return flags;
        }

        /// <summary>
        /// Returns the <see cref="MetadataFlags.Overflow"/> flag if the predicate is true
        /// </summary>
        /// <param name="predicate">the predicate</param>
        /// <returns>the flag to return</returns>
        private static byte OverflowFlag(Func<bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return predicate.Invoke() ? (byte)MetadataFlags.Overflow : (byte)0;
        }

        private static byte ToMetadataByte(DiaType type, MetadataFlags flags)
        {
            byte tbyte = (byte)type;
            byte fbyte = (byte)flags;
            return (byte)(tbyte | fbyte);
        }
        #endregion

        #region Overrides
        public override int GetHashCode() => metadata.GetHashCode();

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is TypeMetadata other
                && metadata.Equals(other.metadata);
        }

        public override string ToString()
        {
            return $"[Type: {Type}, Annotated: {IsAnnotated}, Null: {IsNull}, "
                + $"Custom: {IsCustomFlagSet}, CustomMetadata: {CustomMetadataCount}]";
        }
        #endregion

        #region Nested types

        [Flags]
        public enum MetadataFlags
        {
            None = 0,

            /// <summary>
            /// Indicates that the value represented by this metadata is annotated
            /// </summary>
            Annotated = 16, // 5th bit

            /// <summary>
            /// Indicates that this type represents a null value
            /// </summary>
            Null = 32,      // 6th bit

            /// <summary>
            /// Depends on the implementation of the <see cref="IPayloadSerializer{TDiaValue}"/>
            /// </summary>
            Custom = 64,    //7th bit

            /// <summary>
            /// Indicates that a custom metadata byte follows this byte
            /// </summary>
            Overflow = 128, // 8th bit
        }
        #endregion
    }
}
