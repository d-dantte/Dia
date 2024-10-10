using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    public readonly struct BlobValue :
        IRefValue<byte[]>,
        IDeepCopyable<BlobValue>,
        INullable<BlobValue>,
        IEquatable<BlobValue>,
        IValueEquatable<BlobValue>,
        IDiaAddressable<BlobValue>
    {
        #region Local members
        private readonly byte[]? _value;
        private readonly Guid _address;

        private readonly Annotation[] _annotations;
        #endregion

        #region DiaReference
        public Guid Address => _address;

        public BlobValue RelocateValue(Guid newAddress) => new(newAddress, _value, Annotations);
        #endregion

        #region RefValue
        public byte[]? Value => _value;

        public DiaType Type => DiaType.Blob;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors
        public BlobValue(Guid address, byte[]? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _address = address.ThrowIfDefault(
                _ => new ArgumentException($"Invalid {nameof(address)}: default"));
            _value = value;
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"Invalid {nameof(annotations)}: contains default"))
                .ToArray();
        }

        public BlobValue(byte[]? value, params Annotation[] annotations)
            :this(Guid.NewGuid(), value, annotations)
        {
        }

        public BlobValue(params Annotation[] annotations)
        : this(null, annotations)
        { }

        public static implicit operator BlobValue(byte[]? value) => new BlobValue(value);
        public static implicit operator BlobValue(Span<byte> value) => new BlobValue(value.ToArray());

        public static BlobValue Of(byte[]? value) => Of(value, Array.Empty<Annotation>());

        public static BlobValue Of(byte[]? value, params Annotation[] annotations) => new(value, annotations);

        public static BlobValue Of(
            Guid address,
            byte[]? value,
            params Annotation[] annotations)
            => new(address, value, annotations);
        #endregion

        #region DeepCopyable
        public BlobValue DeepCopy() => new BlobValue(_value, Annotations);
        #endregion

        #region Nullable
        public static BlobValue Null(
            params Annotation[] annotations)
            => new BlobValue(null, annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(BlobValue other)
        {
            return Extensions.NullOrTrue(
                _value,
                other._value,
                Enumerable.SequenceEqual);
        }
        #endregion

        #region Equatable
        public bool ValueEquals(BlobValue other)
        {
            return
                Enumerable.SequenceEqual(Annotations, other.Annotations)
                && ValueEquals(other);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is BlobValue other && ValueEquals(other);

        public override int GetHashCode()
        {
            var attHash = Annotations.Aggregate(0, HashCode.Combine);
            var byteHash = _value?.Aggregate(0, HashCode.Combine) ?? 0;

            return HashCode.Combine(attHash, byteHash);
        }

        public override string ToString()
        {
            var attText = Annotations
                .Select(att => att.ToString()!)
                .JoinUsing(", ");


            return $"[type: {Type}, count: {{{_value?.Length.ToString() ?? ("null")}}}, annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(BlobValue lhs, BlobValue rhs) => lhs.ValueEquals(rhs);

        public static bool operator !=(BlobValue lhs, BlobValue rhs) => !(lhs != rhs);
        #endregion
    }
}
