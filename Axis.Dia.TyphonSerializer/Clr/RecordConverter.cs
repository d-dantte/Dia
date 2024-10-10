using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using Axis.Luna.FInvoke;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Axis.Dia.Typhon.Clr
{
    public class RecordConverter :
        IClrConverter,
        IDefaultInstance<RecordConverter>
    {
        private static readonly Guid MetadataKeyRandomizerValue = Guid.NewGuid();
        private static readonly string ContextHashParam = "$ContextHash";

        public static RecordConverter DefaultInstance { get; } = new();

        public bool CanConvert(DiaType sourceType, TypeInfo destinationTypeInfo)
        {
            return sourceType switch
            {
                DiaType.Record => destinationTypeInfo.Category switch
                {
                    TypeCategory.Map
                    or TypeCategory.Record => true,
                    _ => false
                },
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
                    $"Invalid record conversion [source: {sourceInstance.Type}, destination: {destinationTypeInfo.Type}]");

            var record = sourceInstance.As<Record>();

            if (record.IsNull)
                return null;

            return destinationTypeInfo switch
            {
                TypeInfo info when IsDefaultMap(info) => ToMap(record, destinationTypeInfo, context),
                
                //TypeInfo info when IsRecord(info)
                _ => ToRecord(record, destinationTypeInfo, context)
            };
        }

        #region Map

        private static readonly string ToMapMethodMetadataKeyTemplate =
            $"{typeof(RecordConverter).FullName}.{nameof(ToMap)}.Metadata.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";

        private static readonly MethodInfo ToMapMethod = typeof(RecordConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(method => nameof(ToMap).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        internal static object? ToMap(
            Record record,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(ToMapMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    destinationTypeInfo.ItemType!,
                    itemType => ToMapMethod.MakeGenericMethod(itemType))
                .InvokeFunc(record, destinationTypeInfo, context);
        }

        internal static IDictionary<string, TItemType> ToMap<TItemType>(
            Record record,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            if (!IsDefaultMap(destinationTypeInfo))
                throw new ArgumentException($"Invalid {nameof(destinationTypeInfo)}: Not a supported dictionary/map");

            var itemTypeInfo = typeof(TItemType).ToTypeInfo();
            if (context.Tracker.TryAdd(record, destinationTypeInfo.Type, (rec, type) => NewMap<TItemType>(type), out var map))
            {
                record
                    .Where(property => FilterNulls(property, context.Options))
                    .Select(property => (
                        Name: ToMapKey(property.Name, context.Options),
                        Value: context.ValueConverter.ToClr(property.Value.Payload, itemTypeInfo, context)))
                    .ForEvery(property => map[property.Name] = property.Value.As<TItemType>());
            }

            return map;
        }

        internal static bool IsDefaultMap(TypeInfo typeInfo)
        {
            return TypeCategory.Map.Equals(typeInfo.Category)
                && (typeInfo.Type.IsInterface
                || typeInfo.Type.HasNoArgConstructor());
        }

        internal static IDictionary<string, TItemType> NewMap<TItemType>(Type destinationType)
        {
            return destinationType switch
            {
                Type t when t.Equals(typeof(IDictionary<string, TItemType>))
                || t.Equals(typeof(Dictionary<string, TItemType>)) => new Dictionary<string, TItemType>(),

                Type t when t.HasNoArgConstructor() => Activator
                    .CreateInstance(t)
                    .As<IDictionary<string, TItemType>>(),

                _ => throw new InvalidOperationException($"Invalid map type: '{destinationType}'")
            };
        }

        internal static string ToMapKey(Record.PropertyName name, Options options)
        {
            if (!options.Records.IncludePropertyAttributesInMapKey)
                return name.Name;

            return name.Attributes
                .OrderBy(prop => prop.Key)
                .Aggregate(new StringBuilder(), (sb, next) =>
                {
                    if (next.Value is null)
                        return sb.Append(next.Key).Append("; ");

                    return sb
                        .Append(next.Key).Append(':')
                        .Append(next.Value).Append("; ");
                })
                .ApplyTo(atts => atts.Length switch
                {
                    0 => name.Name,
                    _ => $"{name.Name}[{atts.ToString()[..^1]}]"
                });
        }

        #endregion

        #region Record

        private static readonly string ToRecordMethodMetadataKeyTemplate =
            $"{typeof(RecordConverter).FullName}.{nameof(ToRecord)}.Metadata.Cache.{MetadataKeyRandomizerValue}.{ContextHashParam}";

        private static readonly MethodInfo ToRecordMethod = typeof(RecordConverter)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(method => nameof(ToRecord).Equals(method.Name))
            .Where(method => method.IsGenericMethod)
            .First();

        internal static object? ToRecord(
            Record record,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            return context.Metadata
                .GetOrAdd(
                    MetadataKey(ToRecordMethodMetadataKeyTemplate, context),
                    _ => new Dictionary<Type, MethodInfo>())
                .GetOrAdd(
                    destinationTypeInfo.Type,
                    recordType => ToRecordMethod.MakeGenericMethod(recordType))
                .InvokeFunc(record, destinationTypeInfo, context);
        }

        internal static TRecord? ToRecord<TRecord>(
            Record record,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
            where TRecord : new()
        {
            if (!IsRecord(destinationTypeInfo))
                throw new ArgumentException($"Invalid {nameof(destinationTypeInfo)}: Not a Record");

            if (context.Tracker.TryAdd(record, destinationTypeInfo.Type, (rec, type) => new TRecord(), out var rec))
            {
                // create a map of clr properties
                var propMap = destinationTypeInfo.Type
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(property => property.CanWrite)
                    .Aggregate(new Dictionary<string, PropertyInfo>(), (map, next) =>
                    {
                        var normalizedName = context.Options.Records.PropertyNameNormalizer.Invoke(Options.PropertyNameSource.Clr, next.Name);
                        if (!map.TryAdd(normalizedName, next))
                            throw new InvalidOperationException(
                                $"Invalid property name: name-clash for property '{next.Name}'");

                        return map;
                    });

                // NOTE: propMap should be cached.

                // populate the record
                var visitedProperties = new HashSet<string>();
                record
                    .Select(prop => (
                        DiaProp: prop,
                        NormalizedName: context.Options.Records.PropertyNameNormalizer.Invoke(Options.PropertyNameSource.Dia, prop.Name.Name)))

                    // ignore nulls if the options says so
                    .Where(prop => FilterNulls(prop.DiaProp, context.Options))

                    // filter out properties not included in the clr property map
                    .Where(prop => propMap.ContainsKey(prop.NormalizedName))
                    .Select(prop => (
                        prop.NormalizedName,
                        prop.DiaProp,
                        ClrProp: propMap[prop.NormalizedName]))

                    // convert to clr
                    .Select(prop => (
                        prop.NormalizedName,
                        prop.DiaProp,
                        prop.ClrProp,
                        ClrValue: context.ValueConverter.ToClr(
                            prop.DiaProp.Value.Payload,
                            prop.ClrProp.PropertyType.ToTypeInfo(),
                            context)))

                    // set property values
                    .ForEvery(info =>
                    {
                        if (visitedProperties.Contains(info.NormalizedName))
                            throw new InvalidOperationException(
                                $"Invalid property name: name-clash for property '{info.DiaProp.Name.Name}'");

                        visitedProperties.Add(info.NormalizedName);
                        rec.InvokeAction(info.ClrProp.GetSetMethod(), info.ClrValue);
                    });
            }

            return rec;
        }

        /// <summary>
        /// Convert the property name to a universal value that will match both clr and dia property names.
        /// <para/>
        /// This algorithm interprets names as case-insensitive, no-underscore/no-dash, string of characters.
        /// </summary>
        /// <param name="nameSource">The source of the name</param>
        /// <param name="name">The name to normalize</param>
        /// <returns></returns>
        internal static string NormalizePropertyName(Options.PropertyNameSource nameSource, string name)
        {
            return name?
                .Replace("_", "")
                .Replace("-", "")
                .ToLowerInvariant()
                ?? string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsRecord(TypeInfo typeInfo)
        {
            return TypeCategory.Record.Equals(typeInfo.Category);
        }
        #endregion

        /// <summary>
        /// Returns true if the value is not null, else if null, checks if we want to ignore nulls, returning false if we do,
        /// or true if we don't.
        /// </summary>
        internal static bool FilterNulls(Record.Property property, Options options)
        {
            if (options.Records.IgnoreNulls && property.Value.Payload.As<INullable>().IsNull)
                return false;

            return true;
        }


        private static string MetadataKey(string template, ConverterContext context)
        {
            return template.Replace(
                ContextHashParam,
                context.GetHashCode().ToString());
        }
    }
}
