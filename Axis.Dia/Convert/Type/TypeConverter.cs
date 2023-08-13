namespace Axis.Dia.Convert.Type
{
    public static class TypeConverter
    {
        //private static readonly ConcurrentDictionary<System.Type, ObjectConverter> ObjectConverters = new ConcurrentDictionary<Type, ObjectConverter>();
        //internal static readonly List<IConverter> SingletonDefaultTypeConverters = new List<IConverter>
        //{
        //    new EnumConverter(), // enum come before primitive because some enum types will pass the IsIntegral(...) test
        //    new PrimitiveConverter(),
        //    new MapConverter(),
        //    new ComplexMapConverter(),
        //    new CollectionConverter(),
        //    //new ComplexCollectionProfile()
        //};

        //public static IIonValue ToIon<T>(T value, ConversionOptions? options = null)
        //{
        //    if (value is null)
        //        return new IonNull();

        //    return ToIon(typeof(T), value, options);
        //}

        //public static T? ToClr<T>(IIonValue ion, ConversionOptions? options = null)
        //{
        //    if (ion is null)
        //        throw new ArgumentNullException(nameof(ion));

        //    var obj = ToClr(typeof(T), ion, options);
        //    if (obj is null)
        //        return default;
        //    return (T)obj;
        //}

        //public static IIonValue ToIon(Type sourceType, object? value, ConversionOptions? options = null)
        //{
        //    return ToIon(sourceType, value, new ConversionContext(options ?? new ConversionOptions()));
        //}

        //public static object? ToClr(Type destinationType, IIonValue ion, ConversionOptions? options = null)
        //{
        //    return ToClr(destinationType, ion, new ConversionContext(options ?? new ConversionOptions()));
        //}

        //internal static IIonValue ToIon(Type sourceType, object? instance, ConversionContext context)
        //{
        //    sourceType.ValidateCongruenceWith(instance?.GetType());

        //    var converter = context.Options.IonConverters
        //        .Concat(SingletonDefaultTypeConverters)
        //        .Where(profile => profile.CanConvert(sourceType, instance))
        //        .FirstOrDefault();

        //    if (converter is null)
        //    {
        //        converter = TypeCategory.Object == ConversionUtils.CategoryOf(sourceType)
        //            ? GetObjectConverterFor(sourceType)
        //            : throw new InvalidOperationException($"Could not find a converter for type: {sourceType}");
        //    }
        //    return converter.ToIon(sourceType, instance, context.Next());
        //}

        //internal static object? ToClr(Type destinationType, IIonValue ion, ConversionContext context)
        //{
        //    if (destinationType is null)
        //        throw new ArgumentNullException(nameof(destinationType));

        //    if (ion is null)
        //        throw new ArgumentNullException(nameof(ion));

        //    if (ion is IonNull)
        //        return null;

        //    var converter = context.Options.ClrConverters
        //        .Concat(SingletonDefaultTypeConverters)
        //        .Where(profile => profile.CanConvert(destinationType, ion))
        //        .FirstOrDefault();

        //    if (converter is null)
        //    {
        //        converter = TypeCategory.Object == ConversionUtils.CategoryOf(destinationType)
        //            ? GetObjectConverterFor(destinationType)
        //            : throw new InvalidOperationException($"Could not find a converter for type: {destinationType}, ion-type: {ion.Type}");
        //    }

        //    return converter.ToClr(destinationType, ion, context.Next());
        //}

        //private static ObjectConverter GetObjectConverterFor(Type type)
        //    => ObjectConverters.GetOrAdd(type, _type => new ObjectConverter(_type));

    }
}
