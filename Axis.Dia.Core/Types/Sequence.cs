using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    /// <summary>
    /// Despite being a struct, it is worth noting that the internally encapsulated list of values is mutable,
    /// and as such, instances of this type aren't prime candidates for Hash-Keys
    /// </summary>
    public readonly struct Sequence :
        IDiaType,
        IEquatable<Sequence>,
        INullContract<Sequence>,
        IRefEquatable<Sequence>,
        IValueEquatable<Sequence>,
        IDefaultContract<Sequence>,
        IValueContainer<Sequence, ContainerValue>
    {
        private readonly List<ContainerValue>? _items;
        private readonly AttributeSet _attributes;

        #region Construction

        public Sequence(
            IEnumerable<ContainerValue>? items,
            params Attribute[] attributes)
        {
            ArgumentNullException.ThrowIfNull(attributes);

            _attributes = attributes;

            _items = items?
                .ThrowIfAny(
                    item => item.IsDefault,
                    _ => new ArgumentException("Invalid item: default"))
                .ToList();
        }

        public Sequence(
            Attribute[] attributes,
            params ContainerValue[] items)
            : this(items, attributes)
        { }

        public Sequence(
            params ContainerValue[] items)
            : this(items, Array.Empty<Attribute>())
        { }

        public Sequence()
            : this(Array.Empty<ContainerValue>())
        { }


        public static implicit operator Sequence(
            ContainerValue[] items)
            => new(items);

        public static Sequence Of(
            IEnumerable<ContainerValue>? items,
            params Attribute[] attributes)
            => new(items, attributes);

        public static Sequence Of(
            Attribute[] attributes,
            params ContainerValue[]? items)
            => new(items, attributes);

        public static Sequence Of(
            params ContainerValue[] items)
            => new(items, []);

        #endregion

        #region DiaType

        public DiaType Type => DiaType.Sequence;

        #endregion

        #region DefaultContract
        public static Sequence Default => default;

        public bool IsDefault
            => _items is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Sequence Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _items is null;
        #endregion

        #region IValueContainer

        public static Sequence Empty(
            params Types.Attribute[] attributes)
            => new([], attributes);

        public static Sequence Empty() => Empty([]);

        public bool IsEmpty => _items is null || _items.Count == 0;

        public int Count => _items is not null
            ? _items.Count
            : 0;

        public AttributeSet Attributes => _attributes;

        public bool RefEquals(IRefValue<IEnumerable<ContainerValue>> other)
        {
            if (other is Sequence otherSeq)
                return ReferenceEquals(_items, otherSeq._items);

            return false;
        }

        public IEnumerable<ContainerValue>? Value => _items?.Select(item => item);

        public IEnumerator<ContainerValue> GetEnumerator()
            => _items is null
            ? Enumerable.Empty<ContainerValue>().GetEnumerator()
            : _items!.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines equality of the sequences. This is done by first checking that the encapsulated list is the same instance (ref-equals).
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if sequences are equal, false otherwise</returns>
        public bool Equals(Sequence other) => RefEquals(other);
        #endregion

        #region IValueEquatable

        public bool ValueEquals(Sequence other)
        {
            if (IsNull && other.IsNull)
                return true;

            if (IsNull ^ other.IsNull)
                return false;

            if (!_attributes.Equals(other._attributes))
                return false;

            if (_items!.Count != other._items!.Count)
                return false;

            return _items!.SequenceEqual(other._items!);
        }

        public int ValueHash()
        {
            var attHash = _attributes.Aggregate(0, HashCode.Combine);
            var propHash = _items
                ?.Aggregate(0, HashCode.Combine)
                ?? 0;

            return HashCode.Combine(attHash, propHash);
        }

        #endregion

        #region IRefEquatable
        public bool RefEquals(Sequence other) => ReferenceEquals(_items, other._items);

        public int RefHash() => HashCode.Combine(
            typeof(Sequence).FullName,
            _items?.GetHashCode() ?? 0);
        #endregion

        #region Overrides

        public override string ToString()
        {
            var attLength = !_attributes.IsDefault
                ? _attributes.Count.ToString()
                : "*";

            var propLength = _items is not null
                ? _items.Count.ToString()
                : "*";

            return $"[@{Type} atts: {attLength}, items: {propLength}]";
        }

        public override int GetHashCode() => RefHash();

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Sequence other && Equals(other);

        public static bool operator ==(
            Sequence left,
            Sequence right)
            => left.Equals(right);

        public static bool operator !=(
            Sequence left,
            Sequence right)
            => !left.Equals(right);

        #endregion

        #region List Api - All methods here throw NullReferenceException if the instance is Default/Null

        public ContainerValue this[int index]
        {
            get => _items![index];
            set => _items![index] = value.IsDefault
                ? throw new ArgumentException($"Invalid value: default")
                : value;
        }

        public void Set(int index, IDiaValue value) => this[index] = ContainerValue.Of(value);

        public bool ContainsValue(ContainerValue value) => _items!.Contains(value);

        #region Add
        public void Add(ContainerValue value) => AddItem(value);

        public Sequence AddItem(ContainerValue item)
        {
            _items!.Add(item);
            return this;
        }

        public Sequence AddItem(
            IDiaValue value)
            => AddItem(ContainerValue.Of(value));

        public void AddAll(IEnumerable<ContainerValue> items) => AddItems(items);

        public void AddAll(
            IEnumerable<IDiaValue> items)
            => AddAll(items.Select(ContainerValue.Of));

        public void AddAll(
            params ContainerValue[] items)
            => AddAll(items as IEnumerable<ContainerValue>);

        public void AddAll(
            params IDiaValue[] items)
            => AddAll(items as IEnumerable<IDiaValue>);

        public Sequence AddItems(IEnumerable<ContainerValue> items)
        {
            _items!.AddRange(items);
            return this;
        }

        public Sequence AddItems(
            IEnumerable<IDiaValue> items)
            => AddItems(items.Select(ContainerValue.Of));

        public Sequence AddItems(
            params ContainerValue[] items)
            => AddItems(items as IEnumerable<ContainerValue>);

        public Sequence AddItems(
            params IDiaValue[] items)
            => AddItems(items.Select(ContainerValue.Of).ToArray());
        #endregion

        #region Insert
        public Sequence InsertItemAt(int index, ContainerValue item)
        {
            _items!.Insert(index, item);
            return this;
        }

        public ContainerValue InsertAt(int index, ContainerValue item)
        {
            _items!.Insert(index, item);
            return item;
        }

        public Sequence InsertItemAt(int index, IDiaValue item)
            => InsertItemAt(index, ContainerValue.Of(item));

        public ContainerValue InsertAt(int index, IDiaValue item)
            => InsertAt(index, ContainerValue.Of(item));
        #endregion

        #region Remove
        public bool TryRemove(
            ContainerValue item)
            => _items!.Remove(item);

        public bool TryRemove(
            IDiaValue item)
            => TryRemove(ContainerValue.Of(item));

        public bool TryRemoveAt(
            int index,
            out ContainerValue? item)
        {
            if (index >= 0 && index < _items!.Count)
            {
                item = _items![index];
                _items!.RemoveAt(index);
                return true;
            }

            item = null;
            return false;
        }

        #endregion

        #endregion
    }
}
