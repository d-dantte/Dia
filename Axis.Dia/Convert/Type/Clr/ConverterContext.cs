using Axis.Dia.Contracts;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Type.Clr
{
    public readonly struct ConverterContext: IDefaultValueProvider<ConverterContext>
    {
        /// <summary>
        /// The Dia converter map used during this conversion session
        /// </summary>
        private readonly Dictionary<(DiaType DiaType, System.Type ClrType), IClrConverter> converterMap;

        /// <summary>
        /// The category map used to speed-up type category retrieval
        /// </summary>
        private readonly Dictionary<System.Type, TypeCategory> categoryMap;

        /// <summary>
        /// The options instance
        /// </summary>
        public ConverterOptions Options { get; }

        /// <summary>
        /// The current path within the object graph
        /// </summary>
        public ObjectPath Path { get; }

        /// <summary>
        /// Construct that manages custom data that converters may inject into the context. The manager and it's
        /// contents are available to all Converters.
        /// </summary>
        public MetadataMap MetadataManager { get; }

        /// <summary>
        /// The current object-graph depth
        /// </summary>
        public int Depth => Path.Depth;

        /// <summary>
        /// The reference-tracker instance used for the current conversion session.
        /// </summary>
        public ReferenceTracker ReferenceTracker { get; }

        #region DefaultValueProvider

        public bool IsDefault => default(ConverterContext).Equals(this);

        public static ConverterContext Default => default;
        #endregion


        internal ConverterContext(
            ConverterOptions options,
            ObjectPath objectPath,
            ReferenceTracker? tracker = null,
            MetadataMap? metadataManager = null,
            Dictionary<(DiaType DiaType, System.Type ClrType), IClrConverter>? converterMap = null)
        {
            Options = options.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(options)}: default"));
            Path = objectPath.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(objectPath)}: default"));
            ReferenceTracker = tracker ?? new ReferenceTracker();
            MetadataManager = metadataManager ?? new MetadataMap();
            categoryMap = new();
            this.converterMap = converterMap ?? new();
        }


        #region DiaConverter
        internal IClrConverter ClrConverterFor<TClrType>(
            DiaType diaType,
            IEnumerable<IClrConverter> defaultClrConverters)
            => ClrConverterFor(diaType, typeof(TClrType), defaultClrConverters);


        internal IClrConverter ClrConverterFor(
            DiaType diaType,
            System.Type destinationType,
            IEnumerable<IClrConverter> defaultClrConverters)
        {
            var options = Options;
            var _this = this;
            return converterMap.GetOrAdd((DiaType: diaType, ClrType: destinationType), tuple =>
            {
                return options.ClrConverters
                    .Concat(defaultClrConverters)
                    .Where(converter => converter.CanConvert(
                        tuple.DiaType,
                        tuple.ClrType,
                        _this.GetTypeCategory(tuple.ClrType)))
                    .FirstOrThrow(new InvalidOperationException(
                        "No Clr converter found for the given types. Source: "
                        + $"'{tuple.DiaType}', Destination: '{tuple.ClrType.FullName}'"));
            });
        }
        #endregion

        #region API
        private ConverterContext Next(System.Type type, string objectNodeName)
            => new(
                Options,
                Path.Next(type, objectNodeName),
                ReferenceTracker,
                MetadataManager,
                converterMap);

        /// <summary>
        /// Called by converters, when properties of a map/record, or items of a list are being deserialized/converted to clr
        /// </summary>
        /// <param name="sourceInstance">The source dia instance</param>
        /// <param name="destinationType">The destination clr type</param>
        /// <param name="objectNodeName">The name of the property or index of the list from which this object is referenced</param>
        /// <returns>The result of the conversion</returns>
        public IResult<object?> ToClr(
            IDiaValue sourceInstance,
            System.Type destinationType,
            string objectNodeName)
            => TypeConverter.ToClr(sourceInstance, destinationType, Next(destinationType, objectNodeName));

        public TypeCategory GetTypeCategory(
            System.Type type)
            => categoryMap.GetOrAdd(type, _t => _t.GetTypeCategory());
        #endregion
    }
}
