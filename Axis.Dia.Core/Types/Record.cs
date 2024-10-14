using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Utils;
using Axis.Luna.Extensions;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static Axis.Dia.Core.Types.Record;

namespace Axis.Dia.Core.Types
{
    /// <summary>
    /// Despite being a struct, it is worth noting that internally encapsulate map of properties is mutable,
    /// and as such, instances of this type aren't prime candidates for Hash-Keys
    /// </summary>
    public readonly struct Record :
        IDiaValue,
        IStructureComparable,
        IEquatable<Record>,
        INullContract<Record>,
        IRefEquatable<Record>,
        IValueEquatable<Record>,
        IDefaultContract<Record>,
        IValueContainer<Record, Property>
    {
        /// <summary>
        /// Recursion Guard.
        /// <para/>
        /// To avoid infinite recursion in situations where this sequence contains itself, and is compared with itself.
        /// </summary>
        private static readonly AsyncLocal<HashSet<(Record First, Record Second)>> EqualityRecursionGuard = new()
        {
            Value = []
        };

        private readonly Dictionary<string, (PropertyName Name, DiaValue Value)>? _properties;
        private readonly AttributeSet _attributes;

        #region Construction

        public Record(
            IEnumerable<Property>? properties,
            params Attribute[] attributes)
        {
            ArgumentNullException.ThrowIfNull(attributes);

            _attributes = attributes;

            _properties = properties?
                .ThrowIfAny(
                    prop => prop.IsDefault,
                    _ => new ArgumentException("Invalid property: default/null"))
                .ToDictionary(
                    prop => prop.Name.Name!,
                    prop => (prop.Name, prop.Value));
        }

        public Record(
            Attribute[] attributes,
            params Property[] items)
            : this(items, attributes)
        { }

        public Record(
            params Property[] items)
            : this(items, [])
        { }

        public Record()
            : this(Array.Empty<Property>())
        { }

        public static implicit operator Record(
            Property[] value)
            => new(value);

        public static Record Of(
            IEnumerable<Property>? properties,
            params Attribute[] attributes)
            => new(properties, attributes);

        public static Record Of(
            Attribute[] attributes,
            params Property[] properties)
            => new(properties, attributes);

        public static Record Of(
            params Property[] properties)
            => new(properties, Array.Empty<Attribute>());

        #endregion

        #region DiaType

        public DiaType Type => DiaType.Record;

        #endregion

        #region DefaultContract
        public static Record Default => default;

        public bool IsDefault
            => _properties is null
            && _attributes.IsDefault;
        #endregion

        #region NullContract
        public static Record Null(params
            Types.Attribute[] attributes)
            => new(null, attributes);

        public bool IsNull => _properties is null;
        #endregion

        #region IValueContainer

        public static Record Empty(
            params Types.Attribute[] attributes)
            => new([], attributes);

        public static Record Empty() => Empty([]);

        public bool IsEmpty => (_properties?.Count ?? 0) == 0;

        public int Count => IsDefault ? 0 : _properties!.Count;

        public AttributeSet Attributes => _attributes;

        public IEnumerator<Property> GetEnumerator()
            => _properties is null
            ? Enumerable.Empty<Property>().GetEnumerator()
            : _properties.Values
                .Select(Property.Of)
                .GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #region Add
        public void Add(Property item) => AddItem(item);

        public Record AddItem(Property item)
        {
            AssertNonDefault();

            _properties![item.Name.Name] = item.Tuple;
            return this;
        }

        public void AddAll(params Property[] items) => AddAllItems(items);

        public Record AddAllItems(params Property[] items)
        {
            ArgumentNullException.ThrowIfNull(items);

            items.ForEvery(Add);
            return this;
        }
        #endregion

        #region Remove
        public void Remove(Property item) => RemoveItem(item);

        public Record RemoveItem(Property item)
        {
            AssertNonDefault();
            _properties!.Remove(item.Name.Name);
            return this;
        }

        public void RemoveAll(params Property[] items) => RemoveAllItems(items);

        public Record RemoveAllItems(params Property[] items)
        {
            ArgumentNullException.ThrowIfNull(items);

            items.ForEvery(Remove);
            return this;
        }
        #endregion

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines equality of the Records. This is done by first checking that the encapsulated properties is the same instance (ref-equals).
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if records are equal, false otherwise</returns>
        public bool Equals(Record other) => RefEquals(other);
        #endregion

        #region IValueEquals

        public bool ValueEquals(Record other)
        {
            if (RefEquals(other))
                return true;

            if (IsNull && other.IsNull)
                return true;

            if (IsNull ^ other.IsNull)
                return false;

            if (!_attributes.ValueEquals(other._attributes))
                return false;

            if (_properties!.Count != other._properties!.Count)
                return false;

            return EqualityRecursionGuard.RecursionGuard(
                (First: this, Second: other),
                pair => pair.First._properties!.All(kvp =>
                {
                    if (!pair.Second._properties!.TryGetValue(kvp.Key, out var ovalue))
                        return false;

                    return
                        kvp.Value.Name.ValueEquals(ovalue.Name)
                        && kvp.Value.Value.ValueEquals(ovalue.Value);
                }),
                true);
        }

        public int ValueHash()
        {
            var attHash = _attributes.Aggregate(0, HashCode.Combine);
            var propHash = _properties
                ?.Aggregate(
                    func: (v, next) => HashCode.Combine(v, next.Key, next.Value),
                    seed: 0)
                ?? 0;

            return HashCode.Combine(attHash, propHash);
        }

        #endregion

        #region IRefEquatable
        public bool RefEquals(Record other) => ReferenceEquals(_properties, other._properties);

        public int RefHash() => HashCode.Combine(
            typeof(Record).FullName,
            _properties?.GetHashCode() ?? 0);
        #endregion

        #region Overrides

        public override string ToString()
        {
            var attLength = !_attributes.IsDefault
                ? _attributes.Count.ToString()
                : "*";

            var propLength = _properties is not null
                ? _properties.Count.ToString()
                : "*";

            return $"[@{Type} atts: {attLength}, props: {propLength}]";
        }

        public override int GetHashCode() => RefHash();

        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is Record other && ValueEquals(other);

        public static bool operator ==(
            Record left,
            Record right)
            => left.ValueEquals(right);

        public static bool operator !=(
            Record left,
            Record right)
            => !left.ValueEquals(right);

        #endregion

        #region IRefValue

        public IEnumerable<Property>? Value => IsDefault ? null : (IEnumerable<Property>)this;

        #endregion

        #region IStructureComparable
        public bool IsStructurallyEquivalent(IStructureComparable other)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (!other.Is(out Record rec))
                return false;

            if (RefEquals(rec))
                return true;

            if (Count != rec.Count)
                return false;

            if (!PropertyNameKeys.SetEquals(rec.PropertyNameKeys))
                return false;

            return this
                .Select((property, index) => (First: property.Value, Second: rec[property.Name]))
                .All(info => (info.First.Payload, info.Second.Payload) switch
                {
                    (IStructureComparable s1, IStructureComparable s2) => s1.IsStructurallyEquivalent(s2),
                    (IDiaValue dv1, IDiaValue dv2) => dv1.Type.Equals(dv2.Type),
                    _ => false
                });
        }
        #endregion

        #region Api

        public DiaValue this[PropertyName propertyName]
        {
            get => _properties![propertyName.Name].Value;
            set => SetProperty(propertyName, value);
        }

        public DiaValue this[string propertyName]
        {
            get => _properties![propertyName].Value;
            set => SetProperty(propertyName, value);
        }

        public bool ContainsProperty(
            string propertyName)
            => _properties?.ContainsKey(propertyName) ?? false;

        public bool ContainsProperty(
            PropertyName propertyName)
            => (_properties?.TryGetValue(propertyName.Name, out var value) ?? false)
            && value.Name.ValueEquals(propertyName);

        public ImmutableHashSet<PropertyName> PropertyNames => _properties
            ?.Select(prop => prop.Value.Name)
            .ToImmutableHashSet()
            ?? [];

        public ImmutableHashSet<string> PropertyNameKeys => _properties
            ?.Select(prop => prop.Value.Name.Name)
            .ToImmutableHashSet()
            ?? [];

        public ImmutableArray<DiaValue> PropertyValues => _properties
            ?.Select(prop => prop.Value.Value)
            .ToImmutableArray()
            ?? ImmutableArray<DiaValue>.Empty;

        public bool TryGet(
            string propertName,
            out DiaValue? value)
        {
            AssertNonDefault();
            if (_properties!.TryGetValue(propertName, out var tuple))
            {
                value = tuple.Value;
                return true;
            }

            value = null;
            return false;
        }

        public bool TryGet(
            PropertyName propertyName,
            out DiaValue? value)
        {
            AssertNonDefault();
            if (_properties!.TryGetValue(propertyName.Name, out var tuple)
                && tuple.Name.ValueEquals(propertyName))
            {
                value = tuple.Value;
                return true;
            }

            value = null;
            return false;
        }

        public Record SetProperty(
            string propertyName,
            DiaValue value)
        {
            ArgumentException.ThrowIfNullOrEmpty(propertyName);

            if (value.IsDefault)
                throw new ArgumentException("Invalid value: default");

            AssertNonDefault();

            if (_properties!.TryGetValue(propertyName, out var oldValue))
                _properties![propertyName] = (oldValue.Name, value);

            else
                _properties![propertyName] = (propertyName, value);

            return this;
        }

        public Record SetProperty(
            PropertyName propertyName,
            DiaValue value)
        {
            if (propertyName.IsDefault)
                throw new ArgumentException("Invalid propertyName: default");

            if (value.IsDefault)
                throw new ArgumentException("Invalid value: default");

            AssertNonDefault();
            _properties![propertyName.Name] = (propertyName, value);
            return this;
        }

        public Record SetProperty(Property property)
        {
            return SetProperty(property.Name, property.Value);
        }

        public DiaValue GetOrAdd(
            string propertyKey,
            Func<PropertyName, Property> valueProvider)
        {
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentException.ThrowIfNullOrEmpty(propertyKey);

            AssertNonDefault();
            var props = _properties!;
            var @this = this;
            return props.TryGetValue(propertyKey, out var value)
                ? value.Value
                : valueProvider
                    .Invoke(PropertyName.Of(propertyKey))
                    .ThrowIf(
                        prop => prop.IsDefault,
                        _ => new InvalidOperationException($"Invalid property provided: default"))
                    .ApplyTo(prop => (prop, @this.SetProperty(prop)))
                    .ApplyTo(tuple => tuple.prop.Value);
        }

        public bool TryAdd(string propertyName, DiaValue value)
        {
            return TryAdd(PropertyName.Of(propertyName), value);
        }

        public bool TryAdd(PropertyName propertyName, DiaValue value)
        {
            return TryAdd(Property.Of(propertyName, value));
        }

        public bool TryAdd(Property property)
        {
            if (property.IsDefault)
                throw new ArgumentException("Invalid property: default");

            AssertNonDefault();
            return _properties!.TryAdd(property.Name.Name, property.Tuple);
        }

        public bool TryRemove(string propertyName, out Property property)
        {
            ArgumentException.ThrowIfNullOrEmpty(propertyName);

            AssertNonDefault();
            if (_properties!.TryGetValue(propertyName, out var tuple))
            {
                property = tuple;
                return _properties.Remove(propertyName);
            }

            property = default;
            return false;
        }

        public bool TryRemove(PropertyName propertyName, out Property property)
        {
            if (propertyName.IsDefault)
                throw new ArgumentException("Invalid propertyName: default");

            return TryRemove(propertyName.Name, out property);
        }

        private void AssertNonDefault()
        {
            if (IsDefault)
                throw new InvalidOperationException($"Invalid record instance: default");
        }

        #endregion

        #region Nested types
        public readonly struct Property :
            IDefaultContract<Property>,
            IValueEquatable<Property>,
            IEquatable<Property>
        {
            public PropertyName Name { get; }

            public DiaValue Value { get; }

            public bool IsDefault => Name.IsDefault && Value.IsDefault;

            public static Property Default => default;

            public (PropertyName PropertyName, DiaValue Value) Tuple => (Name, Value);

            public Property(
                PropertyName name,
                DiaValue value)
            {
                if (name.IsDefault)
                    throw new ArgumentException("Invalid name: default");

                if (value.IsDefault)
                    throw new ArgumentException("Invalid value: default");

                Name = name;
                Value = value;
            }

            public static Property Of(
                PropertyName name,
                DiaValue value)
                => new(name, value);

            public static Property Of(
                (PropertyName name, DiaValue value) tuple)
                => new(tuple.name, tuple.value);

            public static implicit operator Property(
                (PropertyName name, DiaValue value) tuple)
                => new(tuple.name, tuple.value);

            #region overrides
            public bool ValueEquals(
                Property other)
                => EqualityComparer<DiaValue>.Default.Equals(Value, other.Value)
                && Name.ValueEquals(other.Name);

            public bool Equals(Property other) => ValueEquals(other);

            public override bool Equals(
                object? obj)
                => obj is Property other && ValueEquals(other);

            public static bool operator ==(
                Property left,
                Property right)
                => left.ValueEquals(right);

            public static bool operator !=(
                Property left,
                Property right)
                => !(left == right);

            public override int GetHashCode() => HashCode.Combine(Name, Value);

            public int ValueHash() => GetHashCode();
            #endregion
        }

        public readonly struct PropertyName :
            IAttributeContainer,
            IEquatable<PropertyName>,
            IValueEquatable<PropertyName>,
            IDefaultContract<PropertyName>
        {
            private readonly string _name;
            private readonly AttributeSet _attribtues;

            public static PropertyName Default => default;

            public bool IsDefault
                => _name is null
                && _attribtues.IsDefault;

            public string Name => _name;

            public bool IsIdentifier
                => !IsDefault
                && Attribute.KeyPattern.IsMatch(_name);

            public AttributeSet Attributes => _attribtues;

            public PropertyName(
                string name,
                params Attribute[] attribtues)
            {
                ArgumentNullException.ThrowIfNull(attribtues);
                ArgumentException.ThrowIfNullOrEmpty(name);

                _name = name;
                _attribtues = attribtues;
            }

            public static PropertyName Of(
                string name,
                params Attribute[] attribtues)
                => new(name, attribtues);

            public static PropertyName Of(
                string name)
                => new(name);

            public static PropertyName Of(
                String name)
                => new(name.Value!, [.. name.Attributes]);

            public static implicit operator PropertyName(string name) => new(name);
            public static implicit operator PropertyName(String name) => Of(name);

            public override int GetHashCode()
            {
                return !IsDefault
                    ? _attribtues.Aggregate(_name.GetHashCode(), HashCode.Combine)
                    : _name.GetHashCode();
            }

            public bool ValueEquals(
                PropertyName other)
                => EqualityComparer<string>.Default.Equals(_name, other._name)
                && _attribtues.ValueEquals(other._attribtues);

            public int ValueHash() => GetHashCode();

            public bool Equals(PropertyName other) => ValueEquals(other);

            public override bool Equals(
                object? obj)
                => obj is PropertyName other
                && ValueEquals(other);

            public static bool operator ==(
                PropertyName left,
                PropertyName right)
                => left.ValueEquals(right);

            public static bool operator !=(
                PropertyName left,
                PropertyName right)
                => !(left == right);
        }
        #endregion
    }
}
