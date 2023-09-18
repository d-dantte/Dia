using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Collections.Immutable;
using System.Reflection;
using static Axis.Dia.Convert.Type.Clr.ConverterOptions;

namespace Axis.Dia.Convert.Type.Clr
{
    public readonly struct ConverterOptions: IDefaultValueProvider<ConverterOptions>
    {
        /// <summary>
        /// A readonly list of <see cref="IClrConverter"/> instances
        /// </summary>
        public IImmutableList<IClrConverter> ClrConverters { get; }

        /// <summary>
        /// A readonly map of poco types to properties, indicating properties on the source clr object that should be ignored by the converter
        /// </summary>
        public ImmutableDictionary<System.Type, ImmutableHashSet<PropertyInfo>> IgnoredProperties { get; }

        /// <summary>
        /// Null value behavior for record/object property values
        /// </summary>
        public NullValueBehavior NullPropertyBehavior { get; }

        /// <summary>
        /// Default value behavior for record/object property values
        /// </summary>
        public DefaultValueBehavior DefaultPropertyBehavior { get; }

        #region DefaultValueProvider

        public bool IsDefault => default(ConverterOptions).Equals(this);

        public static ConverterOptions Default => default;
        #endregion

        internal ConverterOptions(
            NullValueBehavior nullValueBehavior,
            DefaultValueBehavior defaultValueBehavior,
            IEnumerable<IClrConverter> clrConverters,
            IDictionary<System.Type, PropertyInfo[]> ignoredProperties)
        {
            NullPropertyBehavior = nullValueBehavior;
            DefaultPropertyBehavior = defaultValueBehavior;
            ClrConverters = clrConverters?.ToImmutableList() ?? throw new ArgumentNullException(nameof(clrConverters));
            IgnoredProperties = ignoredProperties?
                .ToImmutableDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToImmutableHashSet())
                ?? throw new ArgumentNullException(nameof(ignoredProperties));
        }


        #region Enums
        /// <summary>
        /// Determines how null values are processed during conversion
        /// </summary>
        public enum NullValueBehavior
        {
            Include,
            Ignore
        }

        /// <summary>
        /// Determines how default struct-values are processed during conversion
        /// </summary>
        public enum DefaultValueBehavior
        {
            Include,
            Ignore
        }
        #endregion
    }

    public class ConverterOptionsBuilder
    {
        private readonly List<IClrConverter> clrConverters = new();
        private readonly Dictionary<System.Type, PropertyInfo[]> ignoredProperties = new();
        private NullValueBehavior nullValueBehavior;
        private DefaultValueBehavior defaultValueBehavior;

        /// <summary>
        /// Adds the given converters to the underlying list
        /// </summary>
        /// <param name="converters">The converters to add.</param>
        /// <returns>This instance</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ConverterOptionsBuilder WithClrConverters(params IClrConverter[] converters)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));

            clrConverters.AddRange(
                converters.ThrowIfAny(
                    v => v is null,
                    new ArgumentException("null converters are forbidden")));
            return this;
        }

        /// <summary>
        /// Assigns a default value behavior
        /// </summary>
        /// <param name="defaultValueBehavior">The behavior</param>
        /// <returns>This instance</returns>
        public ConverterOptionsBuilder WithDefaultValueBehavior(DefaultValueBehavior defaultValueBehavior)
        {
            this.defaultValueBehavior = defaultValueBehavior;
            return this;
        }

        /// <summary>
        /// Assigns a null value behavior
        /// </summary>
        /// <param name="nullValueBehavior">The behavior</param>
        /// <returns>This instance</returns>
        public ConverterOptionsBuilder WithNullValueBehavior(NullValueBehavior nullValueBehavior)
        {
            this.nullValueBehavior = nullValueBehavior;
            return this;
        }

        /// <summary>
        /// re/sets the map of ignored properties
        /// </summary>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public ConverterOptionsBuilder WithIgnoredProperties(params PropertyInfo[] properties)
        {
            properties
                .ThrowIfAny(v => v is null, new ArgumentException("null property-names are forbidden"))
                .GroupBy(property => property.DeclaringType ?? throw new ArgumentNullException(nameof(property)))
                .ForAll(group => ignoredProperties[group.Key] = group.ToArray());
            return this;
        }

        /// <summary>
        /// re/sets the map of ignored properties
        /// </summary>
        /// <param name="pocoType"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public ConverterOptionsBuilder WithIgnoredProperties(System.Type pocoType, params string[] propertyNames)
        {
            return propertyNames
                .ThrowIfAny(v => v is null, new ArgumentException("null property-names are forbidden"))
                .Select(name => pocoType.GetProperty(name))
                .Where(property => property is not null)
                .Select(property => property!)
                .ToArray()
                .ApplyTo(WithIgnoredProperties);
        }

        public static ConverterOptionsBuilder FromOptions(ConverterOptions options)
        {
            return ConverterOptionsBuilder
                .NewBuilder()
                .WithNullValueBehavior(options.NullPropertyBehavior)
                .WithDefaultValueBehavior(options.DefaultPropertyBehavior)
                .WithIgnoredProperties(options.IgnoredProperties.SelectMany(kvp => kvp.Value).ToArray());
        }

        public static ConverterOptionsBuilder NewBuilder() => new();

        public ConverterOptionsBuilder()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConverterOptions Build()
        {
            return new ConverterOptions(
                nullValueBehavior,
                defaultValueBehavior,
                clrConverters,
                ignoredProperties);
        }
    }
}
