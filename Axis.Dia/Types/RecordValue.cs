using Axis.Dia.Contracts;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.Types
{
    using Property = KeyValuePair<SymbolValue, IDiaValue>;

    /// <summary>
    /// Note: unlike other <see cref="IDiaValue"/> implementations, implementations of <see cref="IValueContainer{TSelf, TValue}"/>
    /// can only have their null values created when null is passed into the main constructor. All other constructors yield a non-null instance
    /// </summary>
    public class RecordValue :
        IRefValue<Property[]>,
        IValueContainer<RecordValue, Property>,
        IDeepCopyable<RecordValue>,
        INullable<RecordValue>,
        IEquatable<RecordValue>,
        IValueEquatable<RecordValue, Property[]>
    {
        #region Local members
        private readonly Dictionary<string, SymbolValue>? _symbolMap;
        private readonly Dictionary<string, IDiaValue>? _valueMap;

        private readonly Annotation[] _annotations;
        #endregion

        #region RefValue
        public Property[]? Value => IsNull
            ? null
            : _valueMap!
                .Select(kvp => new Property(_symbolMap![kvp.Key], kvp.Value))
                .ToArray();

        public DiaType Type => DiaType.Record;

        public bool IsNull => _valueMap is null;

        public Annotation[] Annotations => _annotations is not null
            ? _annotations.ToArray()
            : Array.Empty<Annotation>();
        #endregion

        #region Constructors

        public RecordValue(IEnumerable<Property>? value, params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            _annotations = annotations
                .ThrowIfAny(
                    ann => ann.IsDefault,
                    _ => new ArgumentException($"'{nameof(annotations)}' list cannot contain invalid values"))
                .ToArray();

            if (value is not null)
            {
                // validate the input
                _ = value!.ThrowIfAny(
                    kvp => kvp.Key.IsNull || kvp.Value is null,
                    _ => new ArgumentException("Null keys or values are not allowed"));

                _valueMap = value!
                    .Select(kvp => (Key: kvp.Key.Value!, kvp.Value))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                _symbolMap = value!
                    .Select(kvp => (Key: kvp.Key.Value!, Value: kvp.Key))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        public RecordValue(
            IEnumerable<(SymbolValue key, IDiaValue value)>? value,
            params Annotation[] annotations)
            : this(value?.Select(kvp => KeyValuePair.Create(kvp.key, kvp.value)), annotations)
        {
        }

        public RecordValue(params Annotation[] annotations)
        : this(Enumerable.Empty<Property>(), annotations)
        { }

        public RecordValue()
        : this(Enumerable.Empty<Property>())
        {
        }

        public static implicit operator RecordValue(Property[]? value) => new RecordValue(value);

        public static implicit operator RecordValue((SymbolValue key, IDiaValue value)[]? value) => new RecordValue(value);

        public static RecordValue Of(IEnumerable<Property>? value) => Of(value, Array.Empty<Annotation>());

        public static RecordValue Of(
            Annotation[] annotations,
            params Property[]? values)
            => new RecordValue(values, annotations);

        public static RecordValue Of(
            Annotation[] annotations,
            params (SymbolValue key, IDiaValue value)[]? values)
            => new RecordValue(values, annotations);

        public static RecordValue Of(
            IEnumerable<Property>? values,
            params Annotation[] annotations)
            => new RecordValue(values, annotations);

        public static RecordValue Of(params Property[] values) => new RecordValue(values);
        #endregion

        #region DeepCopyable
        public RecordValue DeepCopy() => new RecordValue(Value, Annotations);
        #endregion

        #region Nullable
        public static RecordValue Null(
            params Annotation[] annotations)
            => new RecordValue(null as IEnumerable<Property>, annotations);
        #endregion

        #region Container
        public static RecordValue Empty(params Annotation[] annotations)
        {
            return new RecordValue(Array.Empty<Property>(), annotations);
        }

        public bool IsEmpty => _valueMap?.Count == 0;

        public int Count => _valueMap?.Count ?? -1;
        #endregion

        #region ValueEquatable
        public bool ValueEquals(RecordValue? other) => ValueEquals(other, false);

        public bool ValueEquals(RecordValue? other, bool ignorePropertyKeyAnnotations)
        {
            if (other is null)
                return false;

            if (IsNull && other.IsNull)
                return true;

            if (IsNull ^ other.IsNull)
                return false;

            return
                (ignorePropertyKeyAnnotations
                    ? Extensions.SetEquals(_symbolMap!.Keys, other._symbolMap!.Keys)
                    : Extensions.SetEquals(_symbolMap!.Values, other._symbolMap!.Values))
                && _symbolMap.Keys.All(key => _valueMap![key].Equals(other._valueMap![key]));
        }
        #endregion

        #region Equatable
        public bool Equals(RecordValue? other) => Equals(other, false);

        public bool Equals(RecordValue? other, bool ignorePropertyKeyAnnotations)
        {
            return
                ValueEquals(other, ignorePropertyKeyAnnotations)
                && Extensions.NullOrTrue(Annotations, other!.Annotations, Enumerable.SequenceEqual);
        }
        #endregion

        #region Overrides
        public override bool Equals(
            [NotNullWhen(true)] object? obj)
            => obj is RecordValue other && Equals(other);

        public override int GetHashCode()
        {
            var attHash = Annotations.Aggregate(0, HashCode.Combine);
            var propertyHash = Value?.Aggregate(
                0,
                (hash, next) => HashCode.Combine(
                    hash,
                    next.Key.GetHashCode(),
                    next.Value.GetHashCode())) ?? 0;

            return HashCode.Combine(attHash, propertyHash);
        }

        public override string ToString()
        {
            var attText = Annotations
                .Select(att => att.ToString()!)
                .JoinUsing(", ");

            return $"[type: {Type}, count: {{{Value?.Length.ToString() ?? ("null")}}}, annotations: {attText}]";
        }
        #endregion

        #region operators

        public static bool operator ==(RecordValue lhs, RecordValue rhs) => lhs.Equals(rhs);

        public static bool operator !=(RecordValue lhs, RecordValue rhs) => !(lhs != rhs);
        #endregion

        #region ValueWrapper Helpers
        public ValueWrapper this[string propertyName, params Annotation[] annotations]
        {
            set
            {
                if (IsNull)
                    throw new InvalidOperationException("Record is null");

                ArgumentNullException.ThrowIfNull(propertyName);
                ArgumentNullException.ThrowIfNull(annotations);

                var propertySymbol = SymbolValue.Of(propertyName, annotations);
                this[propertySymbol] = value.Value;
            }
        }
        #endregion

        #region Map members
        public IDiaValue this[SymbolValue propertyName]
        {
            set
            {
                if (IsNull)
                    throw new InvalidOperationException("Record is null");

                if (propertyName.IsNull)
                    throw new ArgumentException($"Invalid {nameof(propertyName)}: '{propertyName}'");

                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                _valueMap![propertyName.Value!] = value;

                _ = _symbolMap!.Remove(propertyName.Value!, out _);
                _symbolMap![propertyName.Value!] = propertyName;
            }

            get
            {
                if (IsNull)
                    throw new InvalidOperationException("Record is null");

                if (propertyName.IsNull)
                    throw new ArgumentException($"Invalid {nameof(propertyName)}: '{propertyName}'");

                if(_valueMap!.TryGetValue(propertyName.Value!, out var value))
                {
                    if (propertyName.HasAnnotations())
                    {
                        if (_symbolMap![propertyName.Value!].Equals(propertyName))
                            return value;

                        else throw new KeyNotFoundException($"{propertyName}");
                    }

                    return value;
                }

                throw new KeyNotFoundException($"{propertyName}");
            }
        }

        public bool ContainsKey(SymbolValue propertyName)
        {
            if (IsNull)
                throw new InvalidOperationException("Record is null");

            if (propertyName.IsNull)
                return false;

            return _symbolMap!.TryGetValue(propertyName.Value!, out var symbol)
                && symbol.Equals(propertyName);
        }

        public bool ContainsKey(string propertyName)
        {
            if (IsNull)
                throw new InvalidOperationException("Record is null");

            if (propertyName is null)
                return false;

            return _valueMap!.ContainsKey(propertyName);
        }

        public SymbolValue[]? Keys => _symbolMap?.Values.ToArray();

        public IDiaValue[]? Values => _valueMap?.Values.ToArray();

        public bool TryGetKeySymbol(string key, out SymbolValue keySymbol)
        {
            if (IsNull)
                throw new InvalidOperationException("Record is null");

            if (key is null)
                throw new ArgumentNullException(nameof(key));

            return _symbolMap!.TryGetValue(key, out keySymbol);
        }

        /// <summary>
        /// Gets the value associated with the given <paramref name="propertyName"/>, or adds the property.
        /// <para>
        /// Note:
        /// <list type="number">
        ///     <item>Properties are searched based only on the symbols value - annotations aren't considered</item>
        ///     <item>If the value already exists (based on the symbols value), it is ONLY returned</item>
        ///     <item>If the value is absent, the symbol is added, along with the provided value</item>
        ///     <item>
        ///         If the intent is to change the annotation of the symbol regardless of the keys presence, use 
        ///         the <c>RecordValue[SymbolValue]</c> indexer method
        ///     </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="valueProvider">The value provider</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If the Record is a default/null record</exception>
        /// <exception cref="ArgumentException">If the <paramref name="propertyName"/> is a null value</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="valueProvider"/> is null</exception>
        public IDiaValue GetOrAdd(SymbolValue propertyName, Func<SymbolValue, IDiaValue> valueProvider)
        {
            if (IsNull)
                throw new InvalidOperationException("Record is null");

            if (propertyName.IsNull)
                throw new ArgumentException($"Null '{nameof(propertyName)}' symbol not allowed");

            if (valueProvider is null)
                throw new ArgumentNullException(nameof(valueProvider));

            if (_valueMap!.TryGetValue(propertyName.Value!, out var value))
                return value;

            return this[propertyName] = valueProvider.Invoke(propertyName);
        }

        public bool TryAdd(SymbolValue propertyName, IDiaValue value)
        {
            if (IsNull)
                throw new InvalidOperationException("Record is null");

            if (propertyName.IsNull)
                throw new ArgumentException($"Null '{nameof(propertyName)}' symbol not allowed");

            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (_valueMap!.ContainsKey(propertyName.Value!))
                return false;

            this[propertyName] = value;
            return true;
        }

        public bool TryRemove(SymbolValue propertyName, out IDiaValue? value)
        {
            if (IsNull)
                throw new InvalidOperationException("Record is null");

            if (propertyName.IsNull)
                throw new ArgumentException($"Null '{nameof(propertyName)}' symbol not allowed");

            return
                _valueMap!.Remove(propertyName.Value!, out value)
                && _symbolMap!.Remove(propertyName.Value!);
        }

        public bool TryGet(SymbolValue propertyName, out IDiaValue? value)
        {
            if (IsNull)
                throw new InvalidOperationException("Record is null");

            if (propertyName.IsNull)
                throw new ArgumentException($"Null '{nameof(propertyName)}' symbol not allowed");

            return _valueMap!.TryGetValue(propertyName.Value!, out value);

        }
        #endregion
    }
}
