using System.Collections.Immutable;
using System.Reflection;
using Axis.Luna.Extensions;
using static Axis.Dia.Convert.Type.TypeConverterOptions;

namespace Axis.Dia.Convert.Type
{
    public class TypeConverterOptions
    {
        /// <summary>
        /// A readonly list of <see cref="IClrConverter"/> instances
        /// </summary>
        public IImmutableList<IClrConverter> ClrConverters { get; }

        /// <summary>
        /// A readonly list of <see cref="IDiaConverter"/> instances
        /// </summary>
        public IImmutableList<IDiaConverter> DiaConverters { get; }

        /// <summary>
        /// A readonly map of poco types to property names, indicating properties that should be ignored by the <see cref="IonConverters"/>
        /// when converting objects
        /// </summary>
        public IImmutableDictionary<System.Type, IImmutableSet<PropertyInfo>> IgnoredProperties { get; }

        /// <summary>
        /// Null value behavior
        /// </summary>
        public NullValueBehavior NullBehavior { get; }

        /// <summary>
        /// Default value behavior
        /// </summary>
        public DefaultValueBehavior DefaultBehavior { get; }


        internal TypeConverterOptions(
            NullValueBehavior nullValueBehavior,
            DefaultValueBehavior defaultValueBehavior,
            IEnumerable<IClrConverter> clrConverters,
            IEnumerable<IDiaConverter> ionConverters,
            IDictionary<System.Type, PropertyInfo[]> ignoredProperties)
        {
            NullBehavior = nullValueBehavior;
            DefaultBehavior = defaultValueBehavior;
            ClrConverters = clrConverters?.ToImmutableList() ?? throw new ArgumentNullException(nameof(clrConverters));
            DiaConverters = ionConverters?.ToImmutableList() ?? throw new ArgumentNullException(nameof(ionConverters));
            IgnoredProperties = ignoredProperties?
                .ToImmutableDictionary(
                    kvp => kvp.Key,
                    kvp => (IImmutableSet<PropertyInfo>)kvp.Value.ToImmutableHashSet())
                ?? throw new ArgumentNullException(nameof(ignoredProperties));
        }


        #region Enums
        /// <summary>
        /// Determines how null values are processed when converting Clr pocos to ion
        /// </summary>
        public enum NullValueBehavior
        {
            Include,
            Ignore
        }

        /// <summary>
        /// Determines how default struct-values are processed when converting Clr pocos to ion
        /// </summary>
        public enum DefaultValueBehavior
        {
            Include,
            Ignore
        }
        #endregion
    }


    public class TypeConverterOptionsBuilder
    {
        private readonly List<IClrConverter> clrConverters = new List<IClrConverter>();
        private readonly List<IDiaConverter> diaConverters = new List<IDiaConverter>();
        private readonly Dictionary<System.Type, PropertyInfo[]> ignoredProperties = new Dictionary<System.Type, PropertyInfo[]>();
        private NullValueBehavior nullValueBehavior;
        private DefaultValueBehavior defaultValueBehavior;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="converters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public TypeConverterOptionsBuilder WithClrConverters(params IClrConverter[] converters)
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
        /// 
        /// </summary>
        /// <param name="converters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public TypeConverterOptionsBuilder WithIonConverters(params IDiaConverter[] converters)
        {
            if (converters is null)
                throw new ArgumentNullException(nameof(converters));

            diaConverters.AddRange(
                converters.ThrowIfAny(
                    v => v is null,
                    new ArgumentException("null converters are forbidden")));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValueBehavior"></param>
        /// <returns></returns>
        public TypeConverterOptionsBuilder WithDefaultValueBehavior(DefaultValueBehavior defaultValueBehavior)
        {
            this.defaultValueBehavior = defaultValueBehavior;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nullValueBehavior"></param>
        /// <returns></returns>
        public TypeConverterOptionsBuilder WithNullValueBehavior(NullValueBehavior nullValueBehavior)
        {
            this.nullValueBehavior = nullValueBehavior;
            return this;
        }

        /// <summary>
        /// re/sets the map of ignored properties
        /// </summary>
        /// <param name="pocoType"></param>
        /// <param name="propertyNames"></param>
        /// <returns></returns>
        public TypeConverterOptionsBuilder WithIgnoredProperties(params PropertyInfo[] properties)
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
        public TypeConverterOptionsBuilder WithIgnoredProperties(System.Type pocoType, params string[] propertyNames)
        {
            return propertyNames
                .ThrowIfAny(v => v is null, new ArgumentException("null property-names are forbidden"))
                .Select(name => pocoType.GetProperty(name))
                .Where(property => property is not null)
                .Select(property => property!)
                .ToArray()
                .ApplyTo(WithIgnoredProperties);
        }

        public static TypeConverterOptionsBuilder FromOptions(TypeConverterOptions options)
        {
            return TypeConverterOptionsBuilder
                .NewBuilder()
                .WithNullValueBehavior(options.NullBehavior)
                .WithDefaultValueBehavior(options.DefaultBehavior)
                .WithClrConverters(options.ClrConverters.ToArray())
                .WithIgnoredProperties(options.IgnoredProperties.SelectMany(kvp => kvp.Value).ToArray());
        }

        public static TypeConverterOptionsBuilder NewBuilder() => new TypeConverterOptionsBuilder();

        private TypeConverterOptionsBuilder()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TypeConverterOptions Build()
        {
            return new TypeConverterOptions(
                nullValueBehavior,
                defaultValueBehavior,
                clrConverters,
                diaConverters,
                ignoredProperties);
        }
    }
}
