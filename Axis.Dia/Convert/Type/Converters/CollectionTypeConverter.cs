using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type.Exceptions;
using Axis.Dia.Types;
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
    public class CollectionTypeConverter: IClrConverter, IDiaConverter
    {

        #region Clr Converter
        public bool CanConvert(
            DiaType sourceType,
            System.Type destinationType,
            TypeCategory destinationTypeCategory)
        {
            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            return DiaType.List.Equals(sourceType) && destinationTypeCategory switch
            {
                TypeCategory.Collection
                or TypeCategory.SingleDimensionArray => true,
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

            if (!DiaType.List.Equals(sourceInstance.Type))
                throw new ArgumentOutOfRangeException($"Invalid source type: {sourceInstance.Type}");

            if (!CanConvert(sourceInstance.Type, destinationType, context.GetTypeCategory(destinationType)))
                return Result.Of<object?>(new IncompatibleClrConversionException(
                    sourceInstance.Type,
                    destinationType));

            try
            {
                var collectionItemType = CollectionItemTypeOf(destinationType);
                var diaList = (ListValue)sourceInstance;

                if (diaList.IsNull)
                    return Result.Of((object?)null);

                object clrCollection =
                    IsArray(destinationType) ? ToArray(diaList, collectionItemType, context) :
                    IsImmutableArray(destinationType) ? ToImmutableArray(diaList, collectionItemType, context) :
                    IsImmutableList(destinationType) ? ToImmutableList(diaList, collectionItemType, context) :
                    IsSet(destinationType) ? ToSet(diaList, collectionItemType, context) :
                    IsList(destinationType) ? ToList(diaList, collectionItemType, context) :
                    ToArbitraryCollectionType(diaList, collectionItemType, destinationType, context);

                return Result.Of<object?>(clrCollection);
            }
            catch(Exception e)
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

            return sourceType.IsOrImplementsGenericInterface(typeof(IEnumerable<>));
        }

        public IResult<IDiaValue> ToDia(
            System.Type sourceType,
            object? sourceInstance,
            Dia.ConverterContext context)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            if (!CanConvert(sourceType, context.GetTypeCategory(sourceType)))
                return Result.Of<IDiaValue>(new UnknownClrSourceTypeException(sourceType));

            if (sourceInstance is null)
                return Result.Of<IDiaValue>(ListValue.Null());

            if (!sourceType.IsAssignableFrom(sourceInstance!.GetType()))
                return Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance!.GetType()));

            var collectionItemType = CollectionItemTypeOf(sourceInstance!.GetType());

            return ToListValueInvokerMap
                .GetOrAdd(collectionItemType, t => GenericToListValue
                    .MakeGenericMethod(collectionItemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(sourceInstance, context)
                .As<IResult<ListValue>>()
                .Map(list => list.As<IDiaValue>());
        }

        #endregion

        #region Array

        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToArrayInvokerMap = new();

        private static readonly MethodInfo GenericToArrayMethod = typeof(CollectionTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(ToArray)))
            .First();

        private static bool IsArray(System.Type sourceType)
            => sourceType.GetTypeCategory() == TypeCategory.SingleDimensionArray;

        private static object ToArray(
            ListValue list,
            System.Type collectionItemType,
            Clr.ConverterContext context)
        {
            return ToArrayInvokerMap
                .GetOrAdd(collectionItemType, t => GenericToArrayMethod
                    .MakeGenericMethod(collectionItemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(list, context);
        }

        private static object ToArray<TItem>(ListValue list, Clr.ConverterContext context)
        {
            var capture = new Capture<TItem[]>();

            return capture.Value = list.Value!
                .Select((item, index) =>
                {
                    return context.ReferenceTracker.TryRegisterLazyReferenceInitializer(
                        item,
                        v => capture.Value![index] = v.As<TItem>())
                        ? Result.Of(default(object?))
                        : context.ToClr(item, typeof(TItem), index.ToString());
                })
                .Select(result => result.MapAs<TItem>())
                .FoldInto(items => capture.Value = items.ToArray())
                .Resolve();
        }
        #endregion

        #region Immutable Array

        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToImmutableArrayInvokerMap = new();

        private static readonly MethodInfo GenericToImmutableArrayMethod = typeof(CollectionTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(ToImmutableArray)))
            .First();

        private static bool IsImmutableArray(
            System.Type sourceType)
            => sourceType.IsGenericType
                && sourceType.HasGenericTypeDefinition(typeof(ImmutableArray<>));

        private static object ToImmutableArray(
            ListValue list,
            System.Type collectionItemType,
            Clr.ConverterContext context)
        {
            return ToImmutableArrayInvokerMap
                .GetOrAdd(collectionItemType, t => GenericToImmutableArrayMethod
                    .MakeGenericMethod(collectionItemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(list, context);
        }

        private static object ToImmutableArray<TItem>(ListValue list, Clr.ConverterContext context)
        {
            // The immutable array is designed that it is only created AFTER all elements are available. This means
            // we cannot use placeholders to lazily resolve the value. This also inevitably means any item (or its
            // descendants) that needs a reference of the list will fail because of cyclic reference resolution exception
            return list.Value!
                .Select((item, index) => context.ToClr(item, typeof(TItem), index.ToString()))
                .Select(result => result.MapAs<TItem>())
                .FoldInto(items => ImmutableArray.CreateRange(items))
                .Resolve();
        }
        #endregion

        #region Immutable List

        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToImmutableListInvokerMap = new();

        private static readonly MethodInfo GenericToImmutableListMethod = typeof(CollectionTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(ToImmutableList)))
            .First();

        private static bool IsImmutableList(System.Type sourceType)
        {
            var isConcreteImmutableList =
                sourceType.IsGenericType
                && sourceType.HasGenericTypeDefinition(typeof(ImmutableList<>));

            var isImmutableListInterface =
                sourceType.IsInterface
                && sourceType.IsGenericType
                && sourceType.IsOrImplementsGenericInterface(typeof(IImmutableList<>));

            return isConcreteImmutableList || isImmutableListInterface;
        }

        private static object ToImmutableList(
            ListValue list,
            System.Type collectionItemType,
            Clr.ConverterContext context)
        {
            return ToImmutableListInvokerMap
                .GetOrAdd(collectionItemType, t => GenericToImmutableListMethod
                    .MakeGenericMethod(collectionItemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(list, context);
        }

        private static object ToImmutableList<TItem>(ListValue list, Clr.ConverterContext context)
        {
            // The immutable list is designed that it is only created AFTER all elements are available. This means
            // we cannot use placeholders to lazily resolve the value. This also inevitably means any item (or its
            // descendants) that needs a reference of the list will fail because of cyclic reference resolution exception
            return list.Value!
                .Select((item, index) => context.ToClr(item, typeof(TItem), index.ToString()))
                .Select(result => result.MapAs<TItem>())
                .FoldInto(items => ImmutableList.CreateRange(items))
                .Resolve();
        }
        #endregion

        #region Generic List

        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToListInvokerMap = new();

        private static readonly MethodInfo GenericToListMethod = typeof(CollectionTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(ToList)))
            .First();

        private static bool IsList(System.Type sourceType)
        {
            var isConcreteList =
                sourceType.IsGenericType
                && sourceType.HasGenericTypeDefinition(typeof(List<>));

            var isEnumerableInterface = 
                sourceType.IsInterface
                && sourceType.IsGenericType
                && sourceType.IsOrImplementsGenericInterface(typeof(IEnumerable<>));

            return isConcreteList || isEnumerableInterface;
        }

        private static object ToList(
            ListValue list,
            System.Type collectionItemType,
            Clr.ConverterContext context)
        {
            return ToListInvokerMap
                .GetOrAdd(collectionItemType, t => GenericToListMethod
                    .MakeGenericMethod(collectionItemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(list, context);
        }

        private static object ToList<TItem>(ListValue list, Clr.ConverterContext context)
        {
            var capture = new Capture<List<TItem>>();

            return capture.Value = list.Value!
                .Select((item, index) =>
                {
                    return context.ReferenceTracker.TryRegisterLazyReferenceInitializer(
                        item,
                        v => capture.Value![index] = v.As<TItem>())
                        ? Result.Of(default(object?))
                        : context.ToClr(item, typeof(TItem), index.ToString());
                })
                .Select(result => result.MapAs<TItem>())
                .FoldInto(items => capture.Value = items.ToList())
                .Resolve();
        }
        #endregion

        #region Set

        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToSetInvokerMap = new();

        private static readonly MethodInfo GenericToSetMethod = typeof(CollectionTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(ToSet)))
            .First();

        private static bool IsSet(System.Type sourceType)
        {
            var isConcreteHashSet =
                sourceType.IsGenericType
                && sourceType.HasGenericTypeDefinition(typeof(HashSet<>));

            var isSetInterface =
                sourceType.IsInterface
                && sourceType.IsGenericType
                && sourceType.IsOrImplementsGenericInterface(typeof(ISet<>));

            return isConcreteHashSet || isSetInterface;
        }

        private static object ToSet(
            ListValue list,
            System.Type collectionItemType,
            Clr.ConverterContext context)
        {
            return ToSetInvokerMap
                .GetOrAdd(collectionItemType, t => GenericToSetMethod
                    .MakeGenericMethod(collectionItemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(list, context);
        }

        private static object ToSet<TItem>(ListValue list, Clr.ConverterContext context)
        {
            var capture = new Capture<HashSet<TItem>>();

            // Being a set where order doesn't matter, we filter out unavailable values, but registering a callback
            // to append the values when eventually available.
            return capture.Value = list.Value!
                .Where((item, index) =>
                {
                    return !context.ReferenceTracker.TryRegisterLazyReferenceInitializer(
                        item,
                        v => capture.Value!.Add(v.As<TItem>()));
                })
                .Select((item, index) => context.ToClr(item, typeof(TItem), index.ToString()))
                .Select(result => result.MapAs<TItem>())
                .FoldInto(items => capture.Value = new HashSet<TItem>(items))
                .Resolve();
        }
        #endregion

        #region Arbitrary collection type

        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToArbitraryCollectionTypeInvokerMap = new();

        private static readonly MethodInfo GenericToArbitraryCollectionType = typeof(CollectionTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 2)
            .Where(minfo => minfo.Name.Equals(nameof(ToArbitraryCollectionType)))
            .First();

        private static object ToArbitraryCollectionType(
            ListValue list,
            System.Type collectionItemType,
            System.Type collectionType,
            Clr.ConverterContext context)
        {
            return ToArbitraryCollectionTypeInvokerMap
                .GetOrAdd(collectionType, t => GenericToArbitraryCollectionType
                    .MakeGenericMethod(collectionType, collectionItemType)
                    .ApplyTo(StaticInvoker.InvokerFor))
                .Invoke(list, context);
        }

        private static object ToArbitraryCollectionType<TCollection, TItem>(
            ListValue list,
            Clr.ConverterContext context)
            where TCollection : ICollection<TItem>, new()
        {
            // being a collection, there is no "standard" way to insert values into the structure, hence we cannot
            // use placeholders to effect lazy population of unavailable values.
            return list.Value!
                .Select((item, index) => context.ToClr(item, typeof(TItem), index.ToString()))
                .Select(result => result.MapAs<TItem>())
                .FoldInto(new TCollection().AddRange)
                .Resolve();
        }
        #endregion

        #region ListValue

        private static readonly ConcurrentDictionary<System.Type, StaticInvoker> ToListValueInvokerMap = new();

        private static readonly MethodInfo GenericToListValue = typeof(CollectionTypeConverter)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(minfo => minfo.IsGenericMethod)
            .Where(minfo => minfo.GetGenericArguments().Length == 1)
            .Where(minfo => minfo.Name.Equals(nameof(ToDiaList)))
            .First();

        private static IResult<ListValue> ToDiaList<TItem>(object items, Dia.ConverterContext context)
        {
            return items
                .As<IEnumerable<TItem>>()
                .Select((item, index) => context.ToDia(typeof(TItem), item, index.ToString()))
                .FoldInto(ListValue.Of);
        }
        #endregion

        private static System.Type CollectionItemTypeOf(System.Type collectionType)
        {
            var genericListType =
                collectionType.HasGenericInterfaceDefinition(typeof(IEnumerable<>)) ? collectionType :
                collectionType.TryGetGenericInterface(typeof(IEnumerable<>), out var interfaceType) ? interfaceType :
                throw new ArgumentException($"Supplied type '{collectionType.FullName}' does not implement '{typeof(IEnumerable<>)}'");

            return genericListType.GetGenericArguments()[0];
        }


        #region nested types
        internal class Capture<T>
        {
            public T? Value { get; set; }
        }
        #endregion
    }
}
