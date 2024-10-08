using Axis.Dia.BionSerializer.Types;
using Axis.Dia.BionSerializer.Utils;
using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Metadata
{
    /// <summary>
    /// </summary>
    public readonly struct TypeMetadata :
        IEquatable<TypeMetadata>,
        IDefaultValueProvider<TypeMetadata>
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
                .ApplyTo(bytes => VarBytes.Of(bytes, false));
        }

        public TypeMetadata(VarBytes metadata)
        {
            if (metadata.Length <= 0)
                throw new ArgumentException($"Invalid metadata: {metadata}");

            this.metadata = metadata;
        }

        public static TypeMetadata Of(VarBytes metadata) => new(metadata);

        public static TypeMetadata Of(byte[] varbytes) => Of(VarBytes.Of(varbytes, false));

        public static TypeMetadata Of(
            byte typeMetadata,
            params CustomMetadata[] customMetadata)
            => new(typeMetadata, customMetadata);

        public static TypeMetadata Of(
            DiaType type,
            MetadataFlags flags,
            params CustomMetadata[] customMetadata)
            => new(ToMetadataByte(type, flags), customMetadata);

        public static TypeMetadata Of(
            byte typeMetadata,
            BigInteger? data)
            => new(typeMetadata, data?.ToCustomMetadata() ?? []);

        public static TypeMetadata Of(
            DiaType type,
            MetadataFlags flags,
            BigInteger? data)
            => new(ToMetadataByte(type, flags), data?.ToCustomMetadata() ?? []);

        public static TypeMetadata Of(DiaType type)
            => new(ToMetadataByte(type, MetadataFlags.None));

        public static TypeMetadata Of(
            IDiaValue value,
            params CustomMetadata[] customMetadata)
        {
            ArgumentNullException.ThrowIfNull(value);

            var type = value.Type;
            var flags = MetadataFlags.None;

            // null?
            flags |= value.As<INullable>().IsNull switch
            {
                true => MetadataFlags.Null,
                false => MetadataFlags.None,
            };

            // attributes?
            flags |= value.As<IAttributeContainer>().Attributes.IsEmpty switch
            {
                true => MetadataFlags.None,
                false => MetadataFlags.Annotated,
            };

            return Of(type, flags, customMetadata);
        }

        public static implicit operator TypeMetadata(VarBytes metadata) => new(metadata);
        #endregion

        #region Properties

        public int Length => metadata.Length;

        /// <summary>
        /// Gets the byte representation of the <see cref="DiaType"/>.
        /// </summary>
        public byte TypeByte => (byte)(IsDefault ? 0 : metadata[0] & TypeMask);

        /// <summary>
        /// Converts the <see cref="TypeByte"/> to a <see cref="DiaType"/> instance.
        /// </summary>
        public DiaType Type => (DiaType)TypeByte;

        /// <summary>
        /// Determines if this metadata represents a reference.
        /// </summary>
        public bool IsReferenceType => Reference.ReferenceType == (DiaType)TypeByte;

        /// <summary>
        /// Checks that the <see cref="DiaType"/> represented by the metadata byte is defined on the enum.
        /// </summary>
        public bool IsValidDiaType => Enum.IsDefined(Type);

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

        public MetadataFlags Flags => ToFlags();

        /// <summary>
        /// Gets an array of the contained custom metadata intances, or an empty array if non are present.
        /// </summary>
        public CustomMetadata[] CustomMetadata
            => CustomMetadataCount > 0
                ? metadata[1..]
                    .Select(BionSerializer.Metadata.CustomMetadata.Of)
                    .ToArray()
                : [];
        #endregion

        #region DefaultProvider
        /// <summary>
        /// Indicates if this is the default value for this type
        /// </summary>
        public bool IsDefault => metadata.IsDefault;

        public static TypeMetadata Default => default;
        #endregion

        #region API
        public TypeMetadata WithFlags(MetadataFlags flags)
        {
            return TypeMetadata.Of(Type, flags, CustomMetadata);
        }

        public TypeMetadata WithNull(bool setNull)
        {
            return setNull switch
            {
                true => TypeMetadata.Of(Type, Flags | MetadataFlags.Null, CustomMetadata),
                false => TypeMetadata.Of(Type, Flags & ~MetadataFlags.Null, CustomMetadata)
            };
        }

        public TypeMetadata WithAnnotated(bool setAnnotated)
        {
            return setAnnotated switch
            {
                true => TypeMetadata.Of(Type, Flags | MetadataFlags.Annotated, CustomMetadata),
                false => TypeMetadata.Of(Type, Flags & ~MetadataFlags.Annotated, CustomMetadata)
            };
        }

        public TypeMetadata WithCustomBit(bool setCustom)
        {
            return setCustom switch
            {
                true => TypeMetadata.Of(Type, Flags | MetadataFlags.Custom, CustomMetadata),
                false => TypeMetadata.Of(Type, Flags & ~MetadataFlags.Custom, CustomMetadata)
            };
        }

        public TypeMetadata WithCustomMetadata(params CustomMetadata[] metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            var flags = metadata.Length > 0
                ? Flags | MetadataFlags.Overflow
                : Flags;

            return TypeMetadata.Of(Type, flags, metadata);
        }

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

        public bool Equals(TypeMetadata other)
        {
            return metadata.Equals(other.metadata);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is TypeMetadata other && Equals(other);
        }

        public override string ToString()
        {
            return $"[Type: {Type}, Annotated: {IsAnnotated}, Null: {IsNull}, "
                + $"Custom: {IsCustomFlagSet}, CustomMetadata: {CustomMetadataCount}]";
        }

        public static bool operator ==(TypeMetadata left, TypeMetadata right) => left.Equals(right);

        public static bool operator !=(TypeMetadata left, TypeMetadata right) => !left.Equals(right);
        #endregion

        #region Nested types

        [Flags]
        public enum MetadataFlags
        {
            None = 0,

            /// <summary>
            /// Indicates that the value represented by this metadata is annotated with attributes
            /// </summary>
            Annotated = 16, // 5th bit

            /// <summary>
            /// Indicates that this type represents a null value
            /// </summary>
            Null = 32,      // 6th bit

            /// <summary>
            /// Depends on the implementation of the specific de/serializer.
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
