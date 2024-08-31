using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Core.Types
{
    /// <summary>
    /// Despite being a struct, it is worth noting that internally encapsulate map of properties is mutable,
    /// and as such, instances of this type aren't prime candidates for Hash-Keys
    /// </summary>
    public readonly struct Record :
        IDiaValue,
        IEquatable<Record>,
        INullContract<Record>,
        IRefEquatable<Record>,
        IValueEquatable<Record>,
        IDefaultContract<Record>,
        IValueContainer<Record, Record.Property>
    {
        private readonly Dictionary<string, (PropertyName Name, ContainerValue Value)>? _properties;
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

        public bool IsEmpty => _properties is null || _properties.Count == 0;

        public int Count => _properties is not null ? _properties.Count : 0;

        public AttributeSet Attributes => _attributes;

        public IEnumerator<Property> GetEnumerator()
            => _properties is null
            ? Enumerable.Empty<Property>().GetEnumerator()
            : _properties.Values
                .Select(Property.Of)
                .GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

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
            if (IsNull && other.IsNull)
                return true;

            if (IsNull ^ other.IsNull)
                return false;

            if (!_attributes.Equals(other._attributes))
                return false;

            if (_properties!.Count != other._properties!.Count)
                return false;

            var props = _properties;
            return props.All(kvp =>
            {
                if (!other._properties!.TryGetValue(kvp.Key, out var ovalue))
                    return false;

                return kvp.Value.Equals(ovalue);
            });
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
            => obj is Record other && Equals(other);

        public static bool operator ==(
            Record left,
            Record right)
            => left.Equals(right);

        public static bool operator !=(
            Record left,
            Record right)
            => !left.Equals(right);

        #endregion

        #region Map Api - All methods here throw NullReferenceException if the instance is Default/Null

        public ContainerValue this[PropertyName propertyKey]
        {
            get => _properties![propertyKey.Name].Value;
            set => _properties![propertyKey.Name] = (propertyKey, value);
        }

        public ContainerValue this[string propertyName]
        {
            get => _properties![propertyName].Value;
            set
            {
                if (_properties!.TryGetValue(propertyName, out var tuple))
                    _properties![propertyName] = (tuple.Name, value);

                _properties![propertyName] = (PropertyName.Of(propertyName), value);
            }
        }

        public bool ContainsProperty(
            string propertyKey)
            => _properties!.ContainsKey(propertyKey);

        public ImmutableDictionary<string, PropertyName> PropertyMap => _properties!
            .Select(kvp => (kvp.Key, kvp.Value.Name))
            .ToImmutableDictionary(
                tuple => tuple.Key,
                tuple => tuple.Name);

        public ImmutableArray<ContainerValue> Values => _properties!
            .Select(kvp => kvp.Value.Value)
            .ToImmutableArray();

        public IEnumerable<Property>? Value => _properties?.Select(kvp => Property.Of(kvp.Value.Name, kvp.Value.Value));

        public bool TryGet(
            string propertyKey,
            out ContainerValue? value)
        {
            if (_properties!.TryGetValue(propertyKey, out var tuple))
            {
                value = tuple.Value;
                return true;
            }

            value = null;
            return false;
        }

        public Record SetProperty(
            string propertyKey,
            ContainerValue value)
            => SetProperty(PropertyName.Of(propertyKey), value);

        public Record SetProperty(
            PropertyName propertyName,
            ContainerValue value)
        {
            this[propertyName] = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="valueProvider"></param>
        /// <returns></returns>
        public ContainerValue GetOrAdd(
            string propertyKey,
            Func<PropertyName, Property> valueProvider)
        {
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentException.ThrowIfNullOrEmpty(propertyKey);

            var props = _properties!;
            return _properties!.TryGetValue(propertyKey, out var value)
                ? value.Value
                : valueProvider
                    .Invoke(PropertyName.Of(propertyKey))
                    .ThrowIf(
                        prop => prop.Value.IsDefault,
                        _ => new InvalidOperationException($"Invalid value provided: default"))
                    .ApplyTo(prop => props[prop.Name.Name] = prop.Tuple)
                    .Value;
        }

        /// <summary>
        /// Adds the value if it already does not exist
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAdd(PropertyName propertyKey, ContainerValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return _properties!.TryAdd(propertyKey.Name, (propertyKey, value));
        }

        /// <summary>
        /// Attempts to remove the property with the given name
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRemove(string propertyKey, out ContainerValue? value)
        {
            if (_properties!.TryGetValue(propertyKey, out var tuple))
            {
                value = tuple.Value;
                return _properties!.Remove(propertyKey); ;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to remove the property with the given name
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRemove(PropertyName propertyKey, out ContainerValue? value)
        {
            return TryRemove(propertyKey.Name, out value);
        }


        public Record AddAll(IEnumerable<Property> properties)
        {
            var props = _properties;
            properties
                .ThrowIfNull(() => new ArgumentNullException(nameof(properties)))
                .ForEvery(prop => props![prop.Name.Name] = prop.Tuple);

            return this;
        }

        public Record AddAll(
            params Property[] properties)
            => AddAll(properties as IEnumerable<Property>);

        public IEnumerable<Property> AddProperties(IEnumerable<Property> properties)
        {
            _ = AddAll(properties);
            return properties;
        }

        public IEnumerable<Property> AddProperties(
            params Property[] items)
            => AddProperties(items as IEnumerable<Property>);

        #endregion

        #region Nested types
        public readonly struct Property :
            IDefaultContract<Property>,
            IEquatable<Property>
        {
            public PropertyName Name { get; }

            public ContainerValue Value { get; }

            public bool IsDefault => Name.IsDefault && Value.IsDefault;

            public static Property Default => default;

            public (PropertyName PropertyName, ContainerValue Value) Tuple => (Name, Value);

            public Property(
                PropertyName name,
                ContainerValue value)
            {
                Name = name;
                Value = value;
            }

            public static Property Of(
                PropertyName name,
                ContainerValue value)
                => new(name, value);

            public static Property Of(
                (PropertyName name, ContainerValue value) tuple)
                => new(tuple.name, tuple.value);

            public static implicit operator Property(
                (PropertyName name, ContainerValue value) tuple)
                => new(tuple.name, tuple.value);

            #region overrides
            public bool Equals(
                Property other)
                => EqualityComparer<ContainerValue>.Default.Equals(Value, other.Value)
                && Name.Equals(other.Name);

            public override bool Equals(
                object? obj)
                => obj is Property other && Equals(other);

            public static bool operator ==(
                Property left,
                Property right)
                => left.Equals(right);

            public static bool operator !=(
                Property left,
                Property right)
                => !(left == right);

            public override int GetHashCode() => HashCode.Combine(Name, Value);
            #endregion
        }

        public readonly struct PropertyName :
            IAttributeContainer,
            IEquatable<PropertyName>,
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

            public static implicit operator PropertyName(string name) => new(name);

            public override int GetHashCode()
            {
                return !IsDefault
                    ? _attribtues.Aggregate(_name.GetHashCode(), HashCode.Combine)
                    : _name.GetHashCode();
            }

            public bool Equals(
                PropertyName other)
                => EqualityComparer<string>.Default.Equals(_name, other._name)
                && _attribtues.Equals(other._attribtues);

            public override bool Equals(
                object? obj)
                => obj is PropertyName other
                && Equals(other);

            public static bool operator ==(
                PropertyName left,
                PropertyName right)
                => left.Equals(right);

            public static bool operator !=(
                PropertyName left,
                PropertyName right)
                => !(left == right);
        }
        #endregion
    }
}
