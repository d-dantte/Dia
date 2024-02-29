using Axis.Dia.Core.Utils;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    /// <summary>
    /// Despite being a struct, it is worth noting that internally encapsulate list of values is mutable,
    /// and as such, instances of this type aren't prime candidates for Hash-Keys
    /// </summary>
    public readonly struct Sequence :
        IDiaType,
        IEquatable<Sequence>,
        INullContract<Sequence>,
        IValueEquatable<Sequence>,
        IDefaultContract<Sequence>,
        IValueContainer<Sequence, IDiaValue>
    {
        private readonly List<IDiaValue>? _items;
        private readonly AttributeSet _attributes;

        #region Construction

        public Sequence(
            IEnumerable<IDiaValue>? items,
            params Attribute[] attributes)
        {
            ArgumentNullException.ThrowIfNull(attributes);

            _attributes = attributes;

            _items = items?
                .ThrowIfAny(
                    item => item is null,
                    _ => new ArgumentException("Invalid item: null"))
                .ToList();
        }

        public Sequence(
            Attribute[] attributes,
            params IDiaValue[] items)
            : this(items, attributes)
        { }

        public Sequence(
            params IDiaValue[] items)
            : this(items, Array.Empty<Attribute>())
        { }


        public static implicit operator Sequence(
            IDiaValue[] items)
            => new(items);

        public static Sequence Of(
            IEnumerable<IDiaValue>? items,
            params Attribute[] attributes)
            => new(items, attributes);

        public static Sequence Of(
            Attribute[] attributes,
            params IDiaValue[]? items)
            => new(items, attributes);

        public static Sequence Of(
            params IDiaValue[] items)
            => new(items, Array.Empty<Attribute>());

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
            => new(null, attributes);

        public bool IsEmpty => _items is null || _items.Count == 0;

        public int Count => _items is not null
            ? _items.Count
            : 0;

        public AttributeSet Attributes => _attributes;

        public IEnumerator<IDiaValue> GetEnumerator()
            => _items is null
            ? Enumerable.Empty<IDiaValue>().GetEnumerator()
            : _items!.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IValueEquals

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

        public override int GetHashCode() => ValueHash();

        public bool Equals(
            Sequence other)
            => ValueEquals(other);

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Record other && Equals(other);

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

        public ValueWrapper this[int index]
        {
            get => ValueWrapper.Of(_items![index]);
            set => _items![index] = value.Value;
        }

        public bool ContainsValue(
            IDiaValue value)
            => _items!.Contains(value);

        #region Add
        public void Add(
            ValueWrapper value)
            => Add(value.Value);

        public Sequence Add(IDiaValue item)
        {
            _items!.Add(item);
            return this;
        }

        public IDiaValue AddItem(IDiaValue item)
        {
            _items!.Add(item);
            return item;
        }

        public Sequence AddAll(IEnumerable<IDiaValue> items)
        {
            _items!.AddRange(items);
            return this;
        }

        public Sequence AddAll(
            params IDiaValue[] items)
            => AddAll(items as IEnumerable<IDiaValue>);

        public IEnumerable<IDiaValue> AddItems(IEnumerable<IDiaValue> items)
        {
            _ = AddAll(items);
            return items;
        }

        public IEnumerable<IDiaValue> AddItems(
            params IDiaValue[] items)
            => AddItems(items as IEnumerable<IDiaValue>);
        #endregion

        #region Insert
        public Sequence InsertAt(int index, IDiaValue item)
        {
            _items!.Insert(index, item);
            return this;
        }

        public IDiaValue InsertItemAt(int index, IDiaValue item)
        {
            _items!.Insert(index, item);
            return item;
        }
        #endregion

        #region Remove
        public bool TryRemove(
            IDiaValue item)
            => _items!.Remove(item);

        public bool TryRemoveAt(
            int index,
            out IDiaValue? item)
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
