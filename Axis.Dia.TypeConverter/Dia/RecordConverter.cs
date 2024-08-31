using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.FInvoke;
using System.Collections.Immutable;
using System.Reflection;

namespace Axis.Dia.TypeConverter.Dia
{
    public class RecordConverter :
        IDiaConverter,
        IDefaultInstance<RecordConverter>
    {
        public static RecordConverter DefaultInstance { get; } = new();

        private static readonly MethodInfo GenericPopulateMapMethod = typeof(RecordConverter)
            .GetMethods(BindingFlags.NonPublic|BindingFlags.Static)
            .Where(method => nameof(PopulateMap).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        private static readonly Guid MetadataKeyRandomizerValue = Guid.NewGuid();
        private static readonly string ContextHashParam = "$ContextHash";

        #region PopulateMap Metadata Key Info
        private static readonly string PopulateMapMethodMetadataKeyTemplate =
            $"Axis.Dia.TypeConverter.Dia.RecordConverter.Metadata.GenericPopulateMapMethod.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";
        #endregion

        #region Object Property Accessor Metadata Key Info
        private static readonly string ObjectPropertyAccessorMetadataKeyTemplate =
            $"Axis.Dia.TypeConverter.Dia.RecordConverter.Metadata.ObjectPropertyAccessor.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";
        #endregion

        public bool CanConvert(TypeInfo sourceTypeInfo)
        {
            return /*HasNoArgConstructor(sourceTypeInfo.Type) && */ sourceTypeInfo.Category switch
            {
                TypeCategory.Map
                or TypeCategory.Record => true,
                _ => false
            };
        }

        public IDiaValue ToDia(TypeInfo sourceTypeInfo, object? sourceInstance, ConverterContext context)
        {
            if (!CanConvert(sourceTypeInfo))
                throw new InvalidOperationException(
                    $"Invalid source-type: '{sourceTypeInfo.Type}' is not a simple type");

            if (sourceInstance is null)
                return Record.Null();

            if (context.Tracker.TryAdd(sourceInstance, _ => Record.Empty(), out var rec))
            {
                // populate the instance
                rec = sourceTypeInfo.Category switch
                {
                    TypeCategory.Record => PopulateObject(rec, sourceInstance, context),

                    //TypeCategory.Map
                    _ => PopulateMap(rec, sourceTypeInfo, sourceInstance, context),
                };
            }

            return rec;
        }

        internal static Record PopulateMap(
            Record rec,
            TypeInfo sourceTypeInfo,
            object sourceInstance,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(PopulateMapMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    sourceTypeInfo.ItemType!,
                    itemType => GenericPopulateMapMethod.MakeGenericMethod(itemType))
                .InvokeFunc(rec, sourceInstance, context)
                .As<Record>();
        }

        internal static Record PopulateMap<TItemType>(
            Record rec,
            IDictionary<string, TItemType> sourceInstance,
            ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(sourceInstance);
            ArgumentNullException.ThrowIfNull(context);

            var itemTypeInfo = typeof(TItemType).ToTypeInfo();
            sourceInstance
                .Select(kvp => (kvp.Key, Item: context.ValueConverter.ToDia(itemTypeInfo, kvp.Value, context)))
                .Select(pair => (pair.Key, Item: ContainerValue.Of(pair.Item)))
                .ForEvery(pair => rec[pair.Key] = pair.Item);

            return rec;
        }

        internal static Record PopulateObject(
            Record rec,
            object sourceInstance,
            ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(sourceInstance);
            ArgumentNullException.ThrowIfNull(context);

            context.Metadata
                .GetOrAdd(
                    MetadataKey(ObjectPropertyAccessorMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, ImmutableArray<PropertyInfo>>())
                .GetOrAdd(
                    sourceInstance.GetType(),
                    sourceType => sourceType
                        .GetProperties(BindingFlags.Public|BindingFlags.Instance)
                        .Where(prop => prop.CanRead)
                        .ToImmutableArray())
                .Select(prop => (Property: prop, Value: sourceInstance.InvokeFunc(prop.GetGetMethod())))
                .Select(pair => (pair.Property, DiaValue: context.ValueConverter.ToDia(
                    pair.Property.PropertyType.ToTypeInfo(),
                    pair.Value,
                    context)))
                .ForEvery(pair => rec[pair.Property.Name] = ContainerValue.Of(pair.DiaValue));

            return rec;
        }

        private static string MetadataKey(string template, ConverterContext context)
        {
            return template.Replace(
                ContextHashParam,
                context.GetHashCode().ToString());
        }
    }
}
