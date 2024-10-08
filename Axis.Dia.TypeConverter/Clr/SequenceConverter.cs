using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.FInvoke;
using System.Collections.Immutable;
using System.Reflection;

namespace Axis.Dia.TypeConverter.Clr
{
    public class SequenceConverter :
        IClrConverter,
        IDefaultInstance<SequenceConverter>
    {
        public static SequenceConverter DefaultInstance { get; } = new();

        private static readonly Guid MetadataKeyRandomizerValue = Guid.NewGuid();
        private static readonly string ContextHashParam = "$ContextHash";

        public bool CanConvert(DiaType sourceType, TypeInfo destinationTypeInfo)
        {
            return sourceType switch
            {
                DiaType.Sequence =>
                    IsArray(destinationTypeInfo)
                    || IsImmutableArray(destinationTypeInfo)
                    || IsImmutableList(destinationTypeInfo)
                    || IsSet(destinationTypeInfo)
                    || IsCollection(destinationTypeInfo)
                    || IsEnumerable(destinationTypeInfo),
                _ => false
            };
        }

        public object? ToClr(
            IDiaValue sourceInstance,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(sourceInstance);
            ArgumentNullException.ThrowIfNull(context);

            if (!CanConvert(sourceInstance.Type, destinationTypeInfo))
                throw new InvalidOperationException(
                    $"Invalid sequence conversion [source: {sourceInstance.Type}, destination: {destinationTypeInfo.Type}]");

            var sequence = sourceInstance.As<Sequence>();

            if (sequence.IsNull)
                return null;

            return destinationTypeInfo switch
            {
                TypeInfo info when IsArray(info) => ToArray(sequence, destinationTypeInfo, context),
                TypeInfo info when IsImmutableArray(info) => ToImmutableArray(sequence, destinationTypeInfo, context),
                TypeInfo info when IsImmutableList(info) => ToImmutableList(sequence, destinationTypeInfo, context),
                TypeInfo info when IsSet(info) => ToSet(sequence, destinationTypeInfo, context),
                //TypeInfo info when IsCollection(info) || IsEnumerable(info)
                _ => ToCollection(sequence, destinationTypeInfo, context),
            };
        }

        #region Array

        private static readonly string ToArrayMethodMetadataKeyTemplate =
            $"{typeof(SequenceConverter).FullName}.{nameof(ToArray)}.Metadata.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";

        private static readonly MethodInfo ToArrayMethod = typeof(SequenceConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(method => nameof(ToArray).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        internal static Array ToArray(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(ToArrayMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    destinationTypeInfo.ItemType!,
                    itemType => ToArrayMethod.MakeGenericMethod(itemType))
                .InvokeFunc(sequence, destinationTypeInfo, context)
                .As<Array>();
        }

        internal static TItemType[] ToArray<TItemType>(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            if (!IsArray(destinationTypeInfo))
                throw new ArgumentException($"Invalid {nameof(destinationTypeInfo)}: Not an Array");

            var itemTypeInfo = typeof(TItemType).ToTypeInfo();
            if (context.Tracker.TryAdd(sequence, destinationTypeInfo.Type, (seq, _) => new TItemType[seq.As<Sequence>().Count], out var array))
            {
                sequence
                    .Select(value => context.ValueConverter.ToClr(value.Payload, itemTypeInfo, context))
                    .Select(value => value.As<TItemType>())
                    .ForEvery((index, value) => array[(int)index] = value);
            }

            return array;
        }

        internal static bool IsArray(TypeInfo typeInfo)
        {
            return TypeCategory.SingleDimensionArray.Equals(typeInfo.Category);
        }
        #endregion

        #region ImmutableArray

        private static readonly string ToImmutableArrayMethodMetadataKeyTemplate =
            $"{typeof(SequenceConverter).FullName}.{nameof(ToImmutableArray)}.Metadata.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";

        private static readonly MethodInfo ToImmutableArrayMethod = typeof(SequenceConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(method => nameof(ToImmutableArray).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        internal static object ToImmutableArray(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(ToImmutableArrayMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    destinationTypeInfo.ItemType!,
                    itemType => ToImmutableArrayMethod.MakeGenericMethod(itemType))
                .InvokeFunc(sequence, destinationTypeInfo, context);
        }

        internal static ImmutableArray<TItemType> ToImmutableArray<TItemType>(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            if (!IsImmutableArray(destinationTypeInfo))
                throw new ArgumentException($"Invalid {nameof(destinationTypeInfo)}: Not an ImmutableArray");

            var itemTypeInfo = typeof(TItemType).ToTypeInfo();
            return sequence
                .Select(value => context.ValueConverter.ToClr(value.Payload, itemTypeInfo, context))
                .Select(value => value.As<TItemType>())
                .ToImmutableArray();
        }

        internal static bool IsImmutableArray(TypeInfo typeInfo)
        {
            return
                TypeCategory.Sequence.Equals(typeInfo.Category)
                && typeInfo.Type.IsOrExtendsGenericBase(typeof(ImmutableArray<>));
        }
        #endregion

        #region ImmutableList

        private static readonly string ToImmutableListMethodMetadataKeyTemplate =
            $"{typeof(SequenceConverter).FullName}.{nameof(ToImmutableList)}.Metadata.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";

        private static readonly MethodInfo ToImmutableListMethod = typeof(SequenceConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(method => nameof(ToImmutableList).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        internal static object ToImmutableList(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(ToImmutableListMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    destinationTypeInfo.ItemType!,
                    itemType => ToImmutableListMethod.MakeGenericMethod(itemType))
                .InvokeFunc(sequence, destinationTypeInfo, context);
        }

        internal static ImmutableList<TItemType> ToImmutableList<TItemType>(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            if (!IsImmutableList(destinationTypeInfo))
                throw new ArgumentException($"Invalid {nameof(destinationTypeInfo)}: Not an ImmutableList");

            var itemTypeInfo = typeof(TItemType).ToTypeInfo();
            return sequence
                .Select(value => context.ValueConverter.ToClr(value.Payload, itemTypeInfo, context))
                .Select(value => value.As<TItemType>())
                .ToImmutableList();
        }

        internal static bool IsImmutableList(TypeInfo typeInfo)
        {
            return
                TypeCategory.Sequence.Equals(typeInfo.Category)
                && typeInfo.Type.IsOrExtendsGenericBase(typeof(ImmutableList<>));
        }
        #endregion

        #region ICollection

        private static readonly string ToCollectionMethodMetadataKeyTemplate =
            $"{typeof(SequenceConverter).FullName}.{nameof(ToCollection)}.Metadata.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";

        private static readonly MethodInfo ToCollectionMethod = typeof(SequenceConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(method => nameof(ToCollection).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        internal static object ToCollection(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(ToCollectionMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    destinationTypeInfo.ItemType!,
                    itemType => ToCollectionMethod.MakeGenericMethod(itemType))
                .InvokeFunc(sequence, destinationTypeInfo, context);
        }

        internal static ICollection<TItemType> ToCollection<TItemType>(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            if (!IsCollection(destinationTypeInfo) && !IsEnumerable(destinationTypeInfo))
                throw new ArgumentException($"Invalid {nameof(destinationTypeInfo)}: Not a supported collection/enumerable");

            var itemTypeInfo = typeof(TItemType).ToTypeInfo();
            if (context.Tracker.TryAdd(sequence, destinationTypeInfo.Type, (seq, type) => NewCollection<TItemType>(type), out var collection))
            {
                sequence
                    .Select(value => context.ValueConverter.ToClr(value.Payload, itemTypeInfo, context))
                    .Select(value => value.As<TItemType>())
                    .ForEvery((index, value) => collection.Add(value));
            }

            return collection;
        }

        internal static bool IsCollection(TypeInfo typeInfo)
        {
            return
                TypeCategory.Sequence.Equals(typeInfo.Category)
                && typeInfo.Type.IsOrImplementsGenericInterface(typeof(ICollection<>));
        }

        internal static bool IsEnumerable(TypeInfo typeInfo)
        {
            return
                TypeCategory.Sequence.Equals(typeInfo.Category)
                && typeInfo.Type.IsOrImplementsGenericInterface(typeof(IEnumerable<>));
        }

        internal static ICollection<TItemType> NewCollection<TItemType>(Type destinationType)
        {
            return destinationType switch
            {
                Type t when t.Equals(typeof(List<TItemType>)) => new List<TItemType>(),
                Type t when t.Equals(typeof(LinkedList<TItemType>)) => new LinkedList<TItemType>(),
                Type t when t.HasNoArgConstructor() => Activator.CreateInstance(t).As<ICollection<TItemType>>(),

                // collection interfaces
                Type t when t.Equals(typeof(IEnumerable<TItemType>))
                || t.Equals(typeof(ICollection<TItemType>))
                || t.Equals(typeof(IList<TItemType>))
                || t.Equals(typeof(IReadOnlyList<TItemType>))
                || t.Equals(typeof(IReadOnlyCollection<TItemType>)) => new List<TItemType>(),

                _ => throw new InvalidOperationException($"Invalid collection type: '{destinationType}'")
            };
        }
        #endregion

        #region ISet

        private static readonly string ToSetMethodMetadataKeyTemplate =
            $"{typeof(SequenceConverter).FullName}.{nameof(ToSet)}.Metadata.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";

        private static readonly MethodInfo ToSetMethod = typeof(SequenceConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(method => nameof(ToSet).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        internal static object ToSet(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(ToSetMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    destinationTypeInfo.ItemType!,
                    itemType => ToSetMethod.MakeGenericMethod(itemType))
                .InvokeFunc(sequence, destinationTypeInfo, context);
        }

        internal static ISet<TItemType> ToSet<TItemType>(
            Sequence sequence,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            if (!IsSet(destinationTypeInfo))
                throw new ArgumentException($"Invalid {nameof(destinationTypeInfo)}: Not a Set");

            var itemTypeInfo = typeof(TItemType).ToTypeInfo();
            if (context.Tracker.TryAdd(sequence, destinationTypeInfo.Type, (seq, type) => NewSet<TItemType>(type), out var set))
            {
                sequence
                    .Select(value => context.ValueConverter.ToClr(value.Payload, itemTypeInfo, context))
                    .Select(value => value.As<TItemType>())
                    .ForEvery((index, value) => set.Add(value));
            }

            return set;
        }

        internal static ISet<TItemType> NewSet<TItemType>(Type destinationType)
        {
            return destinationType switch
            {
                Type t when t.Equals(typeof(HashSet<TItemType>)) => new HashSet<TItemType>(),
                Type t when t.HasNoArgConstructor() => Activator.CreateInstance(t).As<ISet<TItemType>>(),

                // set interfaces
                Type t when t.Equals(typeof(ISet<TItemType>)) => new HashSet<TItemType>(),

                _ => throw new InvalidOperationException($"Invalid set type: {destinationType}")
            };
        }

        internal static bool IsSet(TypeInfo typeInfo)
        {
            return
                TypeCategory.Sequence.Equals(typeInfo.Category)
                && typeInfo.Type.IsOrImplementsGenericInterface(typeof(ISet<>));
        }
        #endregion


        private static string MetadataKey(string template, ConverterContext context)
        {
            return template.Replace(
                ContextHashParam,
                context.GetHashCode().ToString());
        }
    }
}
