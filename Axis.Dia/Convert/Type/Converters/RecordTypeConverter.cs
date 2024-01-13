using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type.Exceptions;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Luna.FInvoke;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace Axis.Dia.Convert.Type.Converters
{
    /// <summary>
    /// Converts dia list type into a corresponding supported clr collection type, and vice versa.
    /// </summary>
    public class RecordTypeConverter : IClrConverter, IDiaConverter
    {

        #region Clr Converter
        public bool CanConvert(
            DiaType sourceType,
            System.Type destinationType,
            TypeCategory destinationTypeCategory)
        {
            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            return DiaType.Record.Equals(sourceType) && destinationTypeCategory switch
            {
                TypeCategory.Map
                or TypeCategory.Record => true,
                _ => false
            };
        }


        public IResult<object?> ToClr(
            IDiaValue sourceInstance,
            System.Type destinationType,
            Clr.ConverterContext context)
        {
            if (sourceInstance is null)
                throw new ArgumentNullException(nameof(sourceInstance));

            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            if (!DiaType.Record.Equals(sourceInstance.Type))
                throw new ArgumentOutOfRangeException($"Invalid source type: {sourceInstance.Type}");

            if (!CanConvert(sourceInstance.Type, destinationType, context.GetTypeCategory(destinationType)))
                return Result.Of<object?>(new IncompatibleClrConversionException(
                    sourceInstance.Type,
                    destinationType));

            try
            {
                if (sourceInstance!.IsNull)
                    return Result.Of<object?>(destinationType.DefaultValue());

                object? clrValue = destinationType.GetTypeCategory() switch
                {
                    TypeCategory.Record => ToObject((RecordValue)sourceInstance, destinationType, context),
                    TypeCategory.Map => ToMap((RecordValue)sourceInstance, DictionaryItemTypeOf(destinationType), context),
                    _ => throw new IncompatibleClrConversionException(sourceInstance.Type, destinationType)
                };

                return Result.Of(clrValue);
            }
            catch (Exception e)
            {
                return Result.Of<object?>(e);
            }
        }

        #endregion

        #region Dia Converter
        public bool CanConvert(System.Type sourceType, TypeCategory sourceTypeCategory)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            return sourceTypeCategory switch
            {
                TypeCategory.Map
                or TypeCategory.Record => true,
                _ => false
            };
        }

        public IResult<IDiaValue> ToDia(System.Type sourceType, object? sourceInstance, Dia.ConverterContext context)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            if (!CanConvert(sourceType, context.GetTypeCategory(sourceType)))
                return Result.Of<IDiaValue>(new UnknownClrSourceTypeException(sourceType));

            if (sourceInstance is null)
                return Result.Of<IDiaValue>(RecordValue.Null());

            if (!sourceType.IsAssignableFrom(sourceInstance!.GetType()))
                return Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance!.GetType()));

            try
            {
                var recordValue = sourceType.GetTypeCategory() switch
                {
                    TypeCategory.Record => FromObject(sourceType, sourceInstance, context),
                    TypeCategory.Map => FromMap(DictionaryItemTypeOf(sourceType), sourceInstance, context),
                    _ => throw new IncompatibleDiaConversionException(sourceType, DiaType.Record)
                };

                return Result.Of(recordValue);
            }
            catch(Exception e)
            {
                return Result.Of<IDiaValue>(e);
            }
        }

        #endregion

        #region Map Helpers

        private static System.Type DictionaryItemTypeOf(System.Type dictionaryType)
        {
            var genericListType =
                dictionaryType.HasGenericInterfaceDefinition(typeof(IDictionary<,>)) ? dictionaryType :
                dictionaryType.TryGetGenericInterface(typeof(IDictionary<,>), out var interfaceType) ? interfaceType :
                throw new ArgumentException($"Supplied type '{dictionaryType.FullName}' does not implement '{typeof(IDictionary<,>)}'");

            return genericListType.GetGenericArguments()[1];
        }

        #region To
        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToMapInvokerMap = new();

        private static readonly MethodInfo GenericToMapMethod = typeof(RecordTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(ToMap)))
            .First();

        private static object ToMap(
            RecordValue record,
            System.Type itemType,
            Clr.ConverterContext context)
        {
            return ToMapInvokerMap
                .GetOrAdd(itemType, t => GenericToMapMethod
                    .MakeGenericMethod(itemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(record, context);
        }

        private static IDictionary<string, TItem>? ToMap<TItem>(
            RecordValue record,
            Clr.ConverterContext context)
        {
            var capture = new Capture<IDictionary<string, TItem>>();
            var includeNulls = Dia.ConverterOptions.NullValueBehavior.Include.Equals(context.Options.NullPropertyBehavior);
            var includeDefaults = Dia.ConverterOptions.DefaultValueBehavior.Include.Equals(context.Options.DefaultPropertyBehavior);
            var itemComparer = EqualityComparer<TItem>.Default;

            // populate the instance
            return record.Value!
                .Where(property => !property.Value.IsNull || includeNulls)
                .Where(property =>
                {
                    var notRegistered = !context.ReferenceTracker.TryRegisterLazyReferenceInitializer(property.Value, v =>
                    {
                        if (v.TryCast<TItem>(out var item)
                            && (!itemComparer.Equals(default, item) || includeDefaults))
                            capture.Value![property.Key.Value!] = item;
                    });

                    return notRegistered;
                })
                .Select(property => context
                    .ToClr(property.Value, typeof(TItem), property.Key.Value!)
                    .Map(value => (name: property.Key.Value!, value: value.As<TItem>())))
                .FoldInto(items => capture.Value = items
                    .Where(item => !itemComparer.Equals(default, item.value) || includeDefaults)
                    .ToDictionary(item => item.name, item => item.value))
                .Resolve();
        }
        #endregion

        #region From
        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> FromMapInvokerMap = new();

        private static readonly MethodInfo GenericFromMapMethod = typeof(RecordTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(FromMap)))
            .First();

        private static IDiaValue FromMap(
            System.Type itemType,
            object map,
            Dia.ConverterContext context)
        {
            return FromMapInvokerMap
                .GetOrAdd(itemType, t => GenericFromMapMethod
                    .MakeGenericMethod(itemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(map, context)
                .As<IDiaValue>();
        }

        private static RecordValue FromMap<TItem>(
            IDictionary<string, TItem> map,
            Dia.ConverterContext context)
        {
            var includeNulls = Dia.ConverterOptions.NullValueBehavior.Include.Equals(context.Options.NullPropertyBehavior);
            var includeDefaults = Dia.ConverterOptions.DefaultValueBehavior.Include.Equals(context.Options.DefaultPropertyBehavior);
            var itemComparer = EqualityComparer<TItem>.Default;

            return map
                .Where(kvp => kvp.Value is not null || includeNulls)
                .Where(kvp => !itemComparer.Equals(default, kvp.Value) || includeDefaults)
                .Select(kvp => KeyValuePair.Create(
                    SymbolValue.Of(kvp.Key),
                    context.ToDia(typeof(TItem), kvp.Value, kvp.Key)))
                .Select(kvp => kvp.Value.Map(value => KeyValuePair.Create(kvp.Key, value)))
                .FoldInto(RecordValue.Of)
                .Resolve();
        }
        #endregion

        #endregion

        #region Object Helpers

        #region To
        private static readonly ConcurrentDictionary<System.Type, ImmutableArray<InitializerInfo>> Initializers = new();
        private static readonly ConcurrentDictionary<System.Type, Dictionary<string, SetterInfo>> Setters = new();

        private static Dictionary<string, SetterInfo> BuildSetters(System.Type pocoType)
        {
            if (pocoType is null)
                throw new ArgumentNullException(nameof(pocoType));

            return pocoType
                .GetProperties()
                .Where(propInfo => propInfo.GetSetMethod() is not null)
                .ToDictionary(propInfo => propInfo.Name, SetterInfo.Of);
        }

        private static ImmutableArray<InitializerInfo> BuildInitializers(System.Type pocoType)
        {
            if (pocoType is null)
                throw new ArgumentNullException(nameof(pocoType));

            return pocoType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Select(InitializerInfo.Of)
                .OrderByDescending(initializer => initializer.Rank)
                .ApplyTo(ImmutableArray.CreateRange);
        }


        private static object? ToObject(
            RecordValue record,
            System.Type destinationType,
            Clr.ConverterContext context)
        {
            var itemComparer = EqualityComparer<object>.Default;
            var includeNulls = Dia.ConverterOptions.NullValueBehavior.Include.Equals(context.Options.NullPropertyBehavior);
            var includeDefaults = Dia.ConverterOptions.DefaultValueBehavior.Include.Equals(context.Options.DefaultPropertyBehavior);
            var initializer = Initializers
                .GetOrAdd(destinationType, BuildInitializers)
                .Where(initializer => initializer.CanInitialize(record, out _))
                .FirstOrThrow(new InvalidOperationException($"Cannot find suitable constructor for type: '{destinationType.FullName}'"));

            // create the instance
            var clrInstance = initializer.Initialize(record, context, out var initializedProperties);

            // populate the instance
            var setters = Setters.GetOrAdd(destinationType, BuildSetters).Values;
            record.Value!
                .Where(property => !property.Value.IsNull || includeNulls)
                .Where(kvp => !initializedProperties.Contains(kvp.Key))
                .ForAll(kvp =>
                {
                    var setter = setters
                        .Where(setter => setter.IsEquivalentPropertyName(kvp.Key.Value!))
                        .FirstOrDefault();

                    if (setter is null)
                        return;

                    var isRegistered = context.ReferenceTracker.TryRegisterLazyReferenceInitializer(kvp.Value, v =>
                    {
                        var vdefault = v?.GetType()?.DefaultValue();

                        if (!itemComparer.Equals(vdefault, v) || includeDefaults)
                            setter.SetValue(clrInstance, v);
                    });

                    if (!isRegistered)
                    {
                        var propClrInstance = context
                            .ToClr(kvp.Value, setter.Property.PropertyType, setter.Property.Name)
                            .Resolve();

                        setter.SetValue(clrInstance, propClrInstance);
                    }
                });

            return clrInstance;
        }
        #endregion

        #region From
        private static readonly ConcurrentDictionary<System.Type, Dictionary<string, GetterInfo>> Getters = new();

        private static Dictionary<string, GetterInfo> BuildGetters(System.Type pocoType)
        {
            if (pocoType is null)
                throw new ArgumentNullException(nameof(pocoType));

            return pocoType
                .GetProperties()
                .Where(prop => !"Item".Equals(prop.Name)) // exclude built-in 'item' property used for indexers
                .Where(propInfo => propInfo.GetGetMethod() is not null)
                .ToDictionary(propInfo => propInfo.Name, GetterInfo.Of);
        }

        private static bool IgnoreNullValue(object? clrValue, Dia.ConverterOptions.NullValueBehavior nullBehavior)
        {
            if (clrValue is not null)
                return false;

            return Dia.ConverterOptions.NullValueBehavior.Ignore.Equals(nullBehavior);
        }

        private static bool IgnoreDefaultValue(object? clrValue, Dia.ConverterOptions.DefaultValueBehavior defaultBehavior)
        {
            var valueDefault = clrValue?.GetType()?.DefaultValue();
            if (!EqualityComparer<object>.Default.Equals(valueDefault, clrValue))
                return false;

            return Dia.ConverterOptions.DefaultValueBehavior.Ignore.Equals(defaultBehavior);
        }

        private static IDiaValue FromObject(
            System.Type sourceType,
            object sourceInstance,
            Dia.ConverterContext context)
        {
            var getters = Getters.GetOrAdd(sourceType, BuildGetters).Values;
            return getters.Aggregate(RecordValue.Empty(), (r, getter) =>
            {
                // get the property value from ToDiathe instance.
                var clrValue = getter.GetValue(sourceInstance);

                // if it is null or default, handle it according to options.NullBehavior/DefaultBehavior
                // populate the record
                if (!IgnoreNullValue(clrValue, context.Options.NullPropertyBehavior)
                    && !IgnoreDefaultValue(clrValue, context.Options.DefaultPropertyBehavior))
                {
                    var propertyNameSymbol = SymbolValue.Of(getter.Property.Name);
                    var clrPropertyValue = getter.GetValue(sourceInstance);
                    var propertyValue = context.ToDia(
                        getter.Property.PropertyType,
                        clrPropertyValue,
                        getter.Property.Name);
                    r[propertyNameSymbol] = propertyValue.Resolve();
                }

                return r;
            });
        }
        #endregion

        #endregion

        #region Misc
        /// <summary>
        /// Checks that the given identifiers are equal after transformation.
        /// </summary>
        /// <param name="identifier1">The first identifier</param>
        /// <param name="identifier2">The second identifier</param>
        /// <returns>True if they are equal, false otherwise</returns>
        internal static bool AreEquivalent(string identifier1, string identifier2)
        {
            return TransformIdentifier(identifier1).Equals(TransformIdentifier(identifier2));
        }

        /// <summary>
        /// <list type="number">
        ///     <item>Convert the identifier to small-case</item>
        ///     <item>Strip any underscore characters from it</item>
        /// </list>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static string TransformIdentifier(string identifier)
        {
            if (identifier is null)
                throw new ArgumentNullException(nameof(identifier));

            return identifier
                .ToLower()
                .Replace("_", "");
        }
        #endregion

        #region Nested types

        internal class InitializerInfo
        {
            private readonly HashSet<string> argumentNameCache;

            internal ConstructorInfo Constructor { get; }

            internal ConstructorInvoker ConstructorInvoker { get; }

            internal int Rank => Constructor.GetParameters().Length;

            internal InitializerInfo(ConstructorInfo constructor)
            {
                if (constructor is null)
                    throw new ArgumentNullException(nameof(constructor));

                Constructor = constructor;
                ConstructorInvoker = ConstructorInvoker.InvokerFor(constructor);
                argumentNameCache = Constructor
                    .GetParameters()
                    .Select(param => param.Name!)
                    .Select(TransformIdentifier)
                    .ApplyTo(argNames => new HashSet<string>(argNames));
            }

            internal static InitializerInfo Of(ConstructorInfo constructor) => new InitializerInfo(constructor);

            internal bool CanInitialize(RecordValue record, out HashSet<SymbolValue> propertyNames)
            {
                var localPropNames = propertyNames = new HashSet<SymbolValue>();

                if (record is null)
                    return false;

                // no-arg constructor
                if (argumentNameCache.Count == 0)
                    return true;

                record.Value!
                    .Select(kvp => kvp.Key)
                    .Where(identifier => argumentNameCache.Contains(TransformIdentifier(identifier.Value!)))
                    .Consume(ids => localPropNames.AddRange(ids));

                return argumentNameCache.Count == localPropNames.Count;
            }

            internal object Initialize(
                RecordValue record,
                Clr.ConverterContext context,
                out HashSet<SymbolValue> propertyNames)
            {
                if (record is null)
                    throw new ArgumentNullException(nameof(record));

                var localPropNames = propertyNames = new HashSet<SymbolValue>();

                // no-arg constructor
                if (argumentNameCache.Count == 0)
                    return ConstructorInvoker.New();

                var recordMap = record.Value!
                    .Select(kvp => (Key: TransformIdentifier(kvp.Key.Value!), Value: kvp))
                    .ToDictionary(info => info.Key, info => info.Value);

                return Constructor
                    .GetParameters()
                    .Select(param => (Name: TransformIdentifier(param.Name!), Type: param.ParameterType))
                    .Select(argDef =>
                    {
                        var recordProperty = recordMap[argDef.Name];
                        localPropNames.Add(recordProperty.Key);

                        return context.ToClr(
                            recordProperty.Value,
                            argDef.Type,
                            recordProperty.Key.Value!);
                    })
                    .FoldInto(args => ConstructorInvoker.New(args.ToArray()))
                    .Resolve();
            }
        }

        internal class SetterInfo
        {
            internal PropertyInfo Property { get; }

            internal InstanceInvoker Invoker { get; }

            internal SetterInfo(PropertyInfo property)
            {
                if (property is null)
                    throw new ArgumentNullException(nameof(property));

                Property = property;

                var setter = property.GetSetMethod() ?? throw new ArgumentException($"Supplied property is readonly: '{property.Name}'");
                Invoker = InstanceInvoker.InvokerFor(setter);
            }

            internal static SetterInfo Of(PropertyInfo property) => new SetterInfo(property);

            /// <summary>
            /// Indicates if the given <paramref name="recordPropertyName"/> is equivalent to the underlying <see cref="PropertyInfo"/>.Name
            /// <para>
            /// Note: names are compatible if they are equal when transformed. See <seealso cref="RecordTypeConverter.AreEquivalent(string, string)"/>
            /// </para>
            /// </summary>
            /// <param name="recordPropertyName">The name of the <see cref="RecordValue"/>'s property</param>
            /// <returns>True if the names are compatible, false otherwise</returns>
            internal bool IsEquivalentPropertyName(string recordPropertyName)
            {
                return AreEquivalent(recordPropertyName, Property.Name);
            }

            internal void SetValue(object clrInstance, object? propertyValue)
            {
                _ = Invoker.Invoke(clrInstance, propertyValue);
            }
        }

        internal class GetterInfo
        {
            internal PropertyInfo Property { get; }

            internal InstanceInvoker Invoker { get; }

            internal GetterInfo(PropertyInfo property)
            {
                if (property is null)
                    throw new ArgumentNullException(nameof(property));

                Property = property;

                var getter = property.GetGetMethod() ?? throw new ArgumentException($"Supplied property is writeonly: '{property.Name}'");
                Invoker = InstanceInvoker.InvokerFor(getter);
            }

            internal static GetterInfo Of(PropertyInfo property) => new GetterInfo(property);

            /// <summary>
            /// Indicates if the given <paramref name="recordPropertyName"/> is equivalent to the underlying <see cref="PropertyInfo"/>.Name
            /// <para>
            /// Note: names are compatible if they are equal when transformed. See <seealso cref="RecordTypeConverter.AreEquivalent(string, string)"/>
            /// </para>
            /// </summary>
            /// <param name="recordPropertyName">The name of the <see cref="RecordValue"/>'s property</param>
            /// <returns>True if the names are compatible, false otherwise</returns>
            internal bool IsEquivalentPropertyName(string recordPropertyName)
            {
                return AreEquivalent(recordPropertyName, Property.Name);
            }

            internal object? GetValue(object clrInstance)
            {
                return Invoker.Invoke(clrInstance);
            }
        }

        internal class Capture<T>
        {
            public T? Value { get; set; }
        }

        #endregion
    }
}
