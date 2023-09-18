using Axis.Dia.Contracts;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Type
{
    //public readonly struct TypeConverterContext
    //{
    //    /// <summary>
    //    /// The Clr converter map used during this conversion session
    //    /// </summary>
    //    private readonly Dictionary<(DiaType DiaType, System.Type ClrType), IClrConverter> clrConverterMap;

    //    /// <summary>
    //    /// The Dia converter map used during this conversion session
    //    /// </summary>
    //    private readonly Dictionary<System.Type, IDiaConverter> diaConverterMap;

    //    /// <summary>
    //    /// The options instance
    //    /// </summary>
    //    public TypeConverterOptions Options { get; }

    //    /// <summary>
    //    /// The current path within the object graph
    //    /// </summary>
    //    public ObjectPath Path { get; }

    //    /// <summary>
    //    /// Construct that manages custom data that converters may inject into the context. The manager and it's
    //    /// contents are available to all Converters.
    //    /// </summary>
    //    public MetadataMap MetadataManager { get; }

    //    /// <summary>
    //    /// The current object-graph depth
    //    /// </summary>
    //    public int Depth => Path.Depth;

    //    /// <summary>
    //    /// The object-tracker instance used for the current serialization/deserialization session.
    //    /// </summary>
    //    internal ObjectTracker ObjectTracker { get; }


    //    internal TypeConverterContext(
    //        TypeConverterOptions options,
    //        ObjectPath objectPath,
    //        ObjectTracker? tracker = null,
    //        MetadataMap? metadataManager = null,
    //        Dictionary<(DiaType, System.Type), IClrConverter>? clrConverterMap = null,
    //        Dictionary<System.Type, IDiaConverter>? diaConverterMap = null)
    //    {
    //        Options = options.ThrowIfDefault(new ArgumentException($"default value not accepted for '{typeof(TypeConverterOptions).FullName}'"));
    //        Path = objectPath.ThrowIfDefault(new ArgumentException($"default value not accepted for '{typeof(ObjectPath).FullName}'"));
    //        ObjectTracker = tracker ?? new ObjectTracker();
    //        MetadataManager = metadataManager ?? new MetadataMap();
    //        this.clrConverterMap = clrConverterMap ?? new();
    //        this.diaConverterMap = diaConverterMap ?? new();
    //    }


    //    #region ClrConverter
    //    internal IClrConverter ClrConverterFor<TClrType>(
    //        DiaType diaType,
    //        IEnumerable<IClrConverter> defaultClrConverters)
    //        => ClrConverterFor(diaType, typeof(TClrType), defaultClrConverters);


    //    internal IClrConverter ClrConverterFor(
    //        DiaType diaType,
    //        System.Type destinationType,
    //        IEnumerable<IClrConverter> defaultClrConverters)
    //    {
    //        var options = Options;
    //        return clrConverterMap.GetOrAdd((DiaType: diaType, ClrType: destinationType), tuple =>
    //        {
    //            return options.ClrConverters
    //                .Concat(defaultClrConverters)
    //                .Where(converter => converter.CanConvert(tuple.DiaType, tuple.ClrType))
    //                .FirstOrThrow(new InvalidOperationException(
    //                    "No Clr converter found for the given types. Source: "
    //                    + $"'{tuple.DiaType}', Destination: '{tuple.ClrType.FullName}'"));
    //        });
    //    }
    //    #endregion

    //    #region DiaConverter
    //    internal IDiaConverter DiaConverterFor<TClrType>(
    //        IEnumerable<IDiaConverter> defaultDiaConverters)
    //        => DiaConverterFor(typeof(TClrType), defaultDiaConverters);


    //    internal IDiaConverter DiaConverterFor(
    //        System.Type sourceType,
    //        IEnumerable<IDiaConverter> defaultDiaConverters)
    //    {
    //        var options = Options;
    //        return diaConverterMap.GetOrAdd(sourceType, type =>
    //        {
    //            return options.DiaConverters
    //                .Concat(defaultDiaConverters)
    //                .Where(converter => converter.CanConvert(type))
    //                .FirstOrThrow(new InvalidOperationException(
    //                    $"No Dia converter found for the given type. Source: '{sourceType.FullName}'"));
    //        });
    //    }
    //    #endregion

    //    #region API
    //    private TypeConverterContext Next(System.Type type, string objectNodeId)
    //        => new TypeConverterContext(
    //            Options,
    //            Path.Next(type, objectNodeId),
    //            ObjectTracker,
    //            MetadataManager,
    //            clrConverterMap,
    //            diaConverterMap);

    //    /// <summary>
    //    /// Called by converters, when properties of a map/record, or items of a list are being deserialized/converted to clr
    //    /// </summary>
    //    /// <param name="sourceInstance">The source dia instance</param>
    //    /// <param name="destinationType">The destination clr type</param>
    //    /// <param name="objectNodeId">The name of the property or index of the list from which this object is referenced</param>
    //    /// <returns>The result of the conversion</returns>
    //    public IResult<object?> ToClr(
    //        IDiaValue sourceInstance,
    //        System.Type destinationType,
    //        string objectNodeId)
    //        => TypeConverter.ToClr(sourceInstance, destinationType, Next(destinationType, objectNodeId));

    //    /// <summary>
    //    /// Called by converters, when properties of a map/record, or items of a list are being serialized/converted to dia
    //    /// </summary>
    //    /// <param name="sourceType">The source clr type</param>
    //    /// <param name="sourceInstance">The source clr instance</param>
    //    /// <param name="objectNodeId">The name of the property or index of the list from which this object is referenced</param>
    //    /// <returns>The result of the conversion</returns>
    //    public IResult<IDiaValue> ToDia(
    //        System.Type sourceType,
    //        object? sourceInstance,
    //        string objectNodeId)
    //        => TypeConverter.ToDia(sourceType, sourceInstance, Next(sourceType, objectNodeId));
    //    #endregion
    //}

}
