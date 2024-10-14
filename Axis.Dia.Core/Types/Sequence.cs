using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Utils;
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
        IStructureComparable,
        IEquatable<Sequence>,
        INullContract<Sequence>,
        IRefEquatable<Sequence>,
        IValueEquatable<Sequence>,
        IDefaultContract<Sequence>,
        IValueContainer<Sequence, DiaValue>
    {
        /// <summary>
        /// Recursion Guard.
        /// <para/>
        /// To avoid infinite recursion in situations where this sequence contains itself, and is compared with itself.
        /// </summary>
        private static readonly AsyncLocal<HashSet<(Sequence First, Sequence Second)>> EqualityRecursionGuard = new()
        {
            Value = []
        };

        private readonly List<DiaValue>? _items;
        private readonly AttributeSet _attributes;

        #region Construction

        public Sequence(
            IEnumerable<DiaValue>? items,
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
            params DiaValue[] items)
            : this(items, attributes)
        { }

        public Sequence(
            params DiaValue[] items)
            : this(items, Array.Empty<Attribute>())
        { }

        public Sequence()
            : this(Array.Empty<DiaValue>())
        { }


        public static implicit operator Sequence(
            DiaValue[] items)
            => new(items);

        public static Sequence Of(
            IEnumerable<DiaValue>? items,
            params Attribute[] attributes)
            => new(items, attributes);

        public static Sequence Of(
            Attribute[] attributes,
            params DiaValue[]? items)
            => new(items, attributes);

        public static Sequence Of(
            params DiaValue[] items)
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

        public bool IsEmpty => (_items?.Count ?? 0) == 0;

        public int Count => IsEmpty ? 0 : _items!.Count;

        public AttributeSet Attributes => _attributes;

        public IEnumerable<DiaValue>? Value => _items?.Select(item => item);

        public IEnumerator<DiaValue> GetEnumerator()
            => _items is null
            ? Enumerable.Empty<DiaValue>().GetEnumerator()
            : _items!.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #region Add
        public void Add(DiaValue item) => AddItem(item);

        public Sequence AddItem(DiaValue item)
        {
            AssertNonDefault();

            _items!.Add(item);
            return this;
        }

        public void AddAll(params DiaValue[] items) => AddAllItems(items);

        public Sequence AddAllItems(params DiaValue[] items)
        {
            ArgumentNullException.ThrowIfNull(items);

            items.ForEvery(Add);
            return this;
        }
        #endregion

        #region Remove
        public void Remove(DiaValue item) => RemoveItem(item);

        public Sequence RemoveItem(DiaValue item)
        {
            AssertNonDefault();
            _items!.Remove(item);
            return this;
        }

        public void RemoveAll(params DiaValue[] items) => RemoveAllItems(items);

        public Sequence RemoveAllItems(params DiaValue[] items)
        {
            ArgumentNullException.ThrowIfNull(items);

            items.ForEvery(Remove);
            return this;
        }
        #endregion

        #endregion

        #region IRefValue

        public bool RefEquals(IRefValue<IEnumerable<DiaValue>> other)
        {
            if (other is Sequence otherSeq)
                return ReferenceEquals(_items, otherSeq._items);

            return false;
        }

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
            if (RefEquals(other))
                return true;

            if (IsNull && other.IsNull)
                return true;

            if (IsNull ^ other.IsNull)
                return false;

            if (!_attributes.ValueEquals(other._attributes))
                return false;

            if (_items!.Count != other._items!.Count)
                return false;

            if (_items.Count == 0)
                return true;

            return EqualityRecursionGuard.RecursionGuard(
                (First: this, Second: other),
                pair => pair.First._items!
                    .Select((item, index) => (Item: item, Index: index))
                    .All(tuple => tuple.Item.ValueEquals(pair.Second._items![tuple.Index])),
                true);
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

        #region IStructureComparable
        public bool IsStructurallyEquivalent(IStructureComparable other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (!other.Is(out Sequence seq))
                return false;

            if (RefEquals(seq))
                return true;

            if (Count != seq.Count)
                return false;

            return this
                .Select((item, index) => (First: item, Second: seq[index]))
                .All(info => (info.First.Payload, info.Second.Payload) switch
                {
                    (IStructureComparable s1, IStructureComparable s2) => s1.IsStructurallyEquivalent(s2),
                    (IDiaValue dv1, IDiaValue dv2) => dv1.Type.Equals(dv2.Type),
                    _ => false
                });
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

        public override int GetHashCode() => RefHash();

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Sequence other && ValueEquals(other);

        public static bool operator ==(
            Sequence left,
            Sequence right)
            => left.ValueEquals(right);

        public static bool operator !=(
            Sequence left,
            Sequence right)
            => !left.ValueEquals(right);

        #endregion

        #region Api

        public DiaValue this[int index]
        {
            get
            {
                AssertNonDefault();
                return _items![index];
            }
            set
            {
                AssertNonDefault();
                _items![index] = value.IsDefault
                    ? throw new ArgumentException($"Invalid value: default")
                    : value;
            }
        }

        public void Set(int index, DiaValue value) => this[index] = value;

        public bool ContainsValue(DiaValue value) => AssertNonDefault(@this => @this._items!.Contains(value));

        #region Insert
        public Sequence InsertItemAt(int index, DiaValue item)
        {
            AssertNonDefault();
            _items!.Insert(index, item);
            return this;
        }

        public DiaValue InsertAt(int index, DiaValue item)
        {
            AssertNonDefault();
            _items!.Insert(index, item);
            return item;
        }

        public Sequence InsertItemAt(int index, IDiaValue item)
            => InsertItemAt(index, DiaValue.Of(item));

        public DiaValue InsertAt(int index, IDiaValue item)
            => InsertAt(index, DiaValue.Of(item));
        #endregion

        #region Remove
        public bool TryRemove(
            DiaValue item)
            => AssertNonDefault(@this => @this._items!.Remove(item));

        public bool TryRemove(
            IDiaValue item)
            => TryRemove(DiaValue.Of(item));

        public bool TryRemoveAt(
            int index,
            out DiaValue? item)
        {
            AssertNonDefault();
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

        private void AssertNonDefault()
        {
            if (IsDefault)
                throw new InvalidOperationException($"Invalid record instance: default");
        }

        private T AssertNonDefault<T>(Func<Sequence, T> func)
        {
            AssertNonDefault();
            return func.Invoke(this);
        }

        #endregion
    }
}
