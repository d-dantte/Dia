using Axis.Dia.Contracts;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Dia.Utils;

namespace Axis.Dia.Convert.Type.Dia
{
    public readonly struct ConverterContext: IDefaultValueProvider<ConverterContext>
    {
        /// <summary>
        /// The Dia converter map used during this conversion session
        /// </summary>
        private readonly Dictionary<System.Type, IDiaConverter> converterMap;

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
        /// The object-tracker instance used for the current serialization/deserialization session.
        /// </summary>
        internal ObjectTracker ObjectTracker { get; }

        #region DefaultValueProvider

        public bool IsDefault => default(ConverterContext).Equals(this);

        public static ConverterContext Default => default;
        #endregion


        internal ConverterContext(
            ConverterOptions options,
            ObjectPath objectPath,
            ObjectTracker? tracker = null,
            MetadataMap? metadataManager = null,
            Dictionary<System.Type, IDiaConverter>? converterMap = null)
        {
            Options = options.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(options)}: default"));
            Path = objectPath.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(options)}: default"));
            ObjectTracker = tracker ?? new();
            MetadataManager = metadataManager ?? new();
            categoryMap = new();
            this.converterMap = converterMap ?? new();
        }


        #region DiaConverter
        internal IDiaConverter DiaConverterFor<TClrType>(
            IEnumerable<IDiaConverter> defaultDiaConverters)
            => DiaConverterFor(typeof(TClrType), defaultDiaConverters);


        internal IDiaConverter DiaConverterFor(
            System.Type sourceType,
            IEnumerable<IDiaConverter> defaultDiaConverters)
        {
            var options = Options;
            var _this = this;
            return converterMap.GetOrAdd(sourceType, type =>
            {
                return options.DiaConverters
                    .Concat(defaultDiaConverters)
                    .Where(converter => converter.CanConvert(type, _this.GetTypeCategory(type)))
                    .FirstOrThrow(() => new InvalidOperationException(
                        $"No Dia converter found for the given type. Source: '{sourceType.FullName}'"));
            });
        }
        #endregion

        #region API
        private ConverterContext Next(
            System.Type type,
            string objectNodeId)
            => new(
                Options,
                Path.Next(type, objectNodeId),
                ObjectTracker,
                MetadataManager,
                converterMap);

        /// <summary>
        /// Called by converters, when properties of a map/record, or items of a list are being serialized/converted to dia
        /// </summary>
        /// <param name="sourceType">The source clr type</param>
        /// <param name="sourceInstance">The source clr instance</param>
        /// <param name="objectNodeId">The name of the property or index of the list from which this object is referenced</param>
        /// <returns>The result of the conversion</returns>
        public IResult<IDiaValue> ToDia(
            System.Type sourceType,
            object? sourceInstance,
            string objectNodeId)
            => TypeConverter.ToDia(sourceType, sourceInstance, Next(sourceType, objectNodeId));

        public TypeCategory GetTypeCategory(
            System.Type type)
            => categoryMap.GetOrAdd(type, _t => _t.GetTypeCategory());
        #endregion
    }
}
