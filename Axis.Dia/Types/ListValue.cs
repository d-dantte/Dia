using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    /// <summary>
    /// Note: unlike other <see cref="IDiaValue"/> implementations, implementations of <see cref="IValueContainer{TSelf, TValue}"/>
    /// can only have their null values created when null is passed into the main constructor. All other constructors yield a non-null instance
    /// </summary>
    public class ListValue :
        IRefValue<IDiaValue[]>,
        IValueContainer<ListValue, IDiaValue>,
        IDeepCopyable<ListValue>,
        INullable<ListValue>,
        IEquatable<ListValue>,
        IEnumerable<ValueWrapper>,
        IValueEquatable<ListValue, IDiaValue[]>
    {
        #region Local members
        private readonly List<IDiaValue>? _value;

        private readonly Annotation[] _annotations;
        #endregion

        #region RefValue
        public IDiaValue[]? Value => _value?.ToArray();

        public DiaType Type => DiaType.List;

        public bool IsNull => _value is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors
        public ListValue(IEnumerable<IDiaValue>? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _value = value?.ToList();
            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain invalid values"))
                .ToArray();
        }

        public ListValue(params Annotation[] annotations)
        : this(Enumerable.Empty<IDiaValue>(), annotations)
        { }

        public ListValue()
        : this(Enumerable.Empty<IDiaValue>())
        {
        }

        public static implicit operator ListValue(IDiaValue[]? value) => new ListValue(value);

        public static ListValue Of(IEnumerable<IDiaValue>? value) => Of(value, Array.Empty<Annotation>());

        public static ListValue Of(
            Annotation[] annotations,
            params IDiaValue[] values)
            => new ListValue(values, annotations);

        public static ListValue Of(
            IEnumerable<IDiaValue>? values,
            params Annotation[] annotations)
            => new ListValue(values, annotations);

        public static ListValue Of(params IDiaValue[] values) => new ListValue(values);
        #endregion

        #region DeepCopyable
        public ListValue DeepCopy() => new ListValue(_value?.ToArray(), Annotations);
        #endregion

        #region Nullable
        public static ListValue Null(
            params Annotation[] annotations)
            => new ListValue(null, annotations);
        #endregion

        #region Container
        public static ListValue Empty(params Annotation[] annotations)
        {
            return new ListValue(Array.Empty<IDiaValue>(), annotations);
        }

        public bool IsEmpty => _value?.Count == 0;

        public int Count => _value?.Count ?? -1;
        #endregion

        #region ValueEquatable
        public bool ValueEquals(ListValue other)
        {
            return Extensions.NullOrTrue(_value, other._value, Enumerable.SequenceEqual);
        }
        #endregion

        #region Equatable
        public bool Equals(ListValue? other)
        {
            if (other is null)
                return false;

            return
                ValueEquals(other)
                && Extensions.NullOrTrue(Annotations, other.Annotations, Enumerable.SequenceEqual);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is ListValue other && Equals(other);

        public override int GetHashCode()
        {
            var attHash = Annotations.Aggregate(0, HashCode.Combine);
            var listHash = _value?.Aggregate(0, HashCode.Combine) ?? 0;

            return HashCode.Combine(attHash, listHash);
        }

        public override string ToString()
        {
            var attText = Annotations
                .Select(att => att.ToString()!)
                .JoinUsing(", ");

            return $"[type: {Type}, count: {{{_value?.Count.ToString() ?? ("null")}}}, annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(ListValue lhs, ListValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(ListValue lhs, ListValue rhs) => !(lhs != rhs);
        #endregion

        #region ValueWrapper Helpers
        public IEnumerator<ValueWrapper> GetEnumerator()
        {
            if (IsNull)
                throw new InvalidOperationException("List is null");

            return _value!
                .Select(value => new ValueWrapper(value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(ValueWrapper value)
        {
            if (IsNull)
                throw new InvalidOperationException("List is null");

            _value!.Add(value.Value);
        }
        #endregion

        #region List members

        public ListValue AddValue(IDiaValue value)
        {
            if (IsNull)
                throw new InvalidOperationException("List is null");

            ArgumentNullException.ThrowIfNull(value);

            _value!.Add(value);
            return this;
        }

        public bool Contains(IDiaValue value)
        {
            if (IsNull)
                throw new InvalidOperationException("List is null");

            ArgumentNullException.ThrowIfNull(value);

            return _value!.Contains(value);
        }

        public bool Remove(IDiaValue value)
        {
            if (IsNull)
                throw new InvalidOperationException("List is null");

            ArgumentNullException.ThrowIfNull(value);

            return _value!.Remove(value);
        }

        public void RemoveAt(int index)
        {
            if (IsNull)
                throw new InvalidOperationException("List is null");

            if (index < 0 || index >= _value!.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be < 0");

            _value!.RemoveAt(index);
        }

        public IDiaValue this[int index]
        {
            get
            {
                if (IsNull)
                    throw new InvalidOperationException("List is null");

                return _value![index];
            }
            set
            {
                if (IsNull)
                    throw new InvalidOperationException("List is null");

                _value![index] = value;
            }
        }
        #endregion
    }
}
