using Axis.Dia.Contracts;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    /// <summary>
    /// A structure that represents a reference/pointer to another <see cref="IDiaAddressable{TType}"/> instance
    /// </summary>
    public readonly struct ReferenceValue :
        IDiaReference,
        IDeepCopyable<ReferenceValue>,
        IEquatable<ReferenceValue>,
        IValueEquatable<ReferenceValue>,
        IDefaultValueProvider<ReferenceValue>
    {
        #region Local members
        private readonly IDiaAddressProvider? _value;
        private readonly Guid _address;

        private readonly Annotation[] _annotations;
        #endregion

        #region Properties

        public Guid ValueAddress => _address;

        public IDiaAddressProvider? Value => _value;

        public bool IsLinked => _value is not null;

        #endregion

        #region DiaValue

        public DiaType Type => DiaType.Ref;

        public bool IsNull => false;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();

        #endregion

        #region DefaultValueProvider
        public static ReferenceValue Default => default;

        public bool IsDefault => default(ReferenceValue).Equals(this);
        #endregion

        #region Constructors
        private ReferenceValue(Guid address, IDiaAddressProvider? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _address = address.ThrowIfDefault($"Invalid {nameof(address)} value: '{address}'");
            _value = value;
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain invalid values"))
                .ToArray();
        }

        /// <summary>
        /// Creates a linked reference - i.e, a reference with an address and it's referant value
        /// </summary>
        /// <param name="value">The value instance</param>
        /// <param name="annotations">the annotations to assign to this reference</param>
        /// <returns>the newly created linked reference</returns>
        public static ReferenceValue Of(
            IDiaAddressProvider value,
            params Annotation[] annotations)
            => new(value.Address, value, annotations);

        /// <summary>
        /// Creates an un-linked reference - i.e, a reference with only an address.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="annotations">The annotations to assign to this reference</param>
        /// <returns>The newly created un-linked reference</returns>
        public static ReferenceValue Of(
            Guid address,
            params Annotation[] annotations)
            => new(address, null, annotations);

        /// <summary>
        /// Creates an un-linked reference - i.e, a reference with only an address.
        /// </summary>
        /// <param name="address">The address</param>
        /// <param name="value">The value instance</param>
        /// <param name="annotations">The annotations to assign to this reference</param>
        /// <returns>The newly created un-linked reference</returns>
        public static ReferenceValue Of(
            Guid address,
            IDiaAddressProvider value,
            params Annotation[] annotations)
            => new(address, value, annotations);
        #endregion

        #region API

        /// <summary>
        /// Link the given <paramref name="addressableValue"/> to this reference, relocatingi the value if specified
        /// </summary>
        /// <typeparam name="TDiaValue">The type of the <see cref="IDiaAddressable{TType}"/> instance.</typeparam>
        /// <param name="addressableValue">The instance to link to this reference</param>
        /// <param name="shouldRelocateValue">Indicating if the value should be relocated before linking</param>
        /// <returns>The linked value</returns>
        /// <exception cref="InvalidOperationException">If linking was not successful</exception>
        public IDiaReference LinkValue<TDiaValue>(IDiaAddressable<TDiaValue> addressableValue)
        where TDiaValue : IDiaAddressable<TDiaValue>, IDiaValue
        {
            ArgumentNullException.ThrowIfNull(addressableValue);

            if (IsLinked)
                throw new InvalidOperationException($"Cannot link an already linked reference instance.");

            var linkedAddressableValue = addressableValue.RelocateValue(_address);
            return ReferenceValue.Of(_address, linkedAddressableValue, _annotations);
        }
        #endregion

        #region DeepCopyable
        public ReferenceValue DeepCopy() => new(ValueAddress, Value, Annotations);
        #endregion

        #region ValueEquatable
        public bool ValueEquals(ReferenceValue other)
        {
            return _address.Equals(other.ValueAddress);
        }
        #endregion

        #region Equatable
        public bool Equals(ReferenceValue other)
        {
            return ValueEquals(other)
                && Enumerable.SequenceEqual(Annotations, other._annotations);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is ReferenceValue other && Equals(other);

        public override int GetHashCode()
        {
            return Annotations.Aggregate(
                ValueAddress.GetHashCode(),
                (code, next) => HashCode.Combine(code, next));
        }

        public override string ToString()
        {
            var attText = Annotations
                .Select(att => att.ToString()!)
                .JoinUsing(", ");

            return $"[type: {Type}, address: {_address}, annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(ReferenceValue lhs, ReferenceValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(ReferenceValue lhs, ReferenceValue rhs) => !(lhs != rhs);
        #endregion
    }
}
