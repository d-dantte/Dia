using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type.Clr;
using Axis.Dia.Convert.Type.Converters;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Convert.Type
{
    public static class TypeConverter
    {
        private static readonly List<IClrConverter> DefaultClrConverters = new();
        private static readonly List<IDiaConverter> DefaultDiaConverters = new();


        static TypeConverter()
        {
            var simpleTypeConverter = new SimpleTypeConverter();
            DefaultClrConverters.Add(simpleTypeConverter);
            DefaultDiaConverters.Add(simpleTypeConverter);

            var enumTypeConverter = new EnumTypeConverter();
            DefaultClrConverters.Add(enumTypeConverter);
            DefaultDiaConverters.Add(enumTypeConverter);

            var collectionTypeConverter = new CollectionTypeConverter();
            DefaultClrConverters.Add(collectionTypeConverter);
            DefaultDiaConverters.Add(collectionTypeConverter);

            var recordTypeConverter = new RecordTypeConverter();
            DefaultClrConverters.Add(recordTypeConverter);
            DefaultDiaConverters.Add(recordTypeConverter);

            // others come here
        }

        #region ToClr

        public static IResult<object?> ToClr(
            IDiaValue sourceInstance,
            System.Type destinationType,
            Clr.ConverterOptions options = default)
        {
            var _options = options.IsDefault
                ? Clr.ConverterOptionsBuilder.NewBuilder().Build()
                : options;

            var context = new Clr.ConverterContext(_options, new ObjectPath(destinationType));

            return ToClr(sourceInstance, destinationType,  context);
        }

        internal static IResult<object?> ToClr(
            IDiaValue sourceInstance,
            System.Type destinationType,
            Clr.ConverterContext context)
        {
            if (sourceInstance is null)
                throw new ArgumentNullException(nameof(sourceInstance));

            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            if (context.IsDefault)
                throw new ArgumentException($"Invalid (default) value supplied for '{nameof(context)}'");

            var linkedRef = sourceInstance switch
            {
                IDiaReference v => v,

                BoolValue v => ReferenceValue.Of(v),
                IntValue v => ReferenceValue.Of(v),
                DecimalValue v => ReferenceValue.Of(v),
                InstantValue v => ReferenceValue.Of(v),
                BlobValue v => ReferenceValue.Of(v),
                ClobValue v => ReferenceValue.Of(v),
                StringValue v => ReferenceValue.Of(v),
                SymbolValue v => ReferenceValue.Of(v),
                ListValue v => ReferenceValue.Of(v),
                RecordValue v => ReferenceValue.Of(v),
                _ => throw new ArgumentException($"Invalid '{nameof(sourceInstance)}' value: '{sourceInstance}'")
            };

            try
            {
                if (context.ReferenceTracker.IsTracked(linkedRef, out var info))
                {
                    if (info is TrackingInfo.Value valueInfo)
                        return Result.Of(valueInfo.TrackedValue);

                    else if (info is TrackingInfo.Placeholder placeholderInfo)
                        return Result.Of<object?>(new InvalidOperationException(
                            $"Cyclic reference resolution detected for reference: {linkedRef.ValueAddress}"));

                    else return Result.Of<object?>(new InvalidOperationException(
                        $"Invalid TrackingIinfo found: '{info?.GetType().FullName ?? "null"}'"));
                }

                var converter = context.ClrConverterFor(
                    linkedRef.Value!.Type,
                    destinationType,
                    DefaultClrConverters);

                return context.ReferenceTracker.TrackConversion(
                    linkedRef,
                    () => converter.ToClr(linkedRef.Value!, destinationType, context));
            }
            catch(Exception e)
            {
                return Result.Of<object?>(e);
            }
        }

        #endregion

        #region ToDia

        /// <summary>
        /// Converts the given <paramref name="sourceInstance"/> into a <see cref="IDiaValue"/> instance.
        /// <para/>
        /// This method first checks if the given <paramref name="sourceInstance"/> is tracked, returning the mapped <see cref="ReferenceValue"/>
        /// instance if it is. Next, it will attempt to track the <paramref name="sourceInstance"/> before calling on the converter,
        /// assuming the <paramref name="sourceInstance"/> has a <see cref="TypeCategory"/> of <see cref="TypeCategory.Collection"/>,
        /// <see cref="TypeCategory.Map"/>, or <see cref="TypeCategory.Record"/>.
        /// </summary>
        /// <param name="sourceType">The source type - present here in case the <paramref name="sourceInstance"/> is null</param>
        /// <param name="sourceInstance">The source instance</param>
        /// <param name="options">The converter options</param>
        /// <returns>The conversion result</returns>
        public static IResult<IDiaValue> ToDia(
            System.Type sourceType,
            object? sourceInstance,
            Dia.ConverterOptions options = default)
        {
            var _options = options.IsDefault
                ? Dia.ConverterOptionsBuilder.NewBuilder().Build()
                : options;

            var context = new Dia.ConverterContext(_options, new ObjectPath(sourceType));

            return ToDia(sourceType, sourceInstance, context);
        }

        internal static IResult<IDiaValue> ToDia(
            System.Type sourceType,
            object? sourceInstance,
            Dia.ConverterContext context)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceInstance));

            if (context.IsDefault)
                throw new ArgumentException($"Invalid (default) value supplied for '{nameof(context)}'");

            var isTrackableCategory = IsTrackableCategory(sourceType.GetTypeCategory());
            IDiaReference? _ref = null;

            // check if the object is being tracked, returning a reference if it is
            if (sourceInstance is not null
                && isTrackableCategory
                && context.ObjectTracker.IsTracked(sourceInstance, out _ref))
                return Result.Of<IDiaValue>(_ref!);

            try
            {
                var converter = context.DiaConverterFor(
                    sourceType,
                    DefaultDiaConverters);

                // track the object
                if (sourceInstance is not null
                    && isTrackableCategory
                    && !context.ObjectTracker.TryTrack(sourceInstance, out _ref))
                    return Result.Of<IDiaValue>(new InvalidOperationException(
                        $"Failed to track object: '{sourceInstance}'"));

                return converter
                    .ToDia(sourceType, sourceInstance, context)
                    .Map(diaValue => _ref is null
                        ? diaValue
                        : diaValue switch
                        {
                            // link the ref to a relocated value, and return the boxed version.
                            BoolValue v => _ref.LinkValue(v).Value!,
                            IntValue v => _ref.LinkValue(v).Value!,
                            DecimalValue v => _ref.LinkValue(v).Value!,
                            InstantValue v => _ref.LinkValue(v).Value!,
                            BlobValue v => _ref.LinkValue(v).Value!,
                            ClobValue v => _ref.LinkValue(v).Value!,
                            StringValue v => _ref.LinkValue(v).Value!,
                            SymbolValue v => _ref.LinkValue(v).Value!,
                            ListValue v => _ref.LinkValue(v).Value!,
                            RecordValue v => _ref.LinkValue(v).Value!,
                            _ => throw new InvalidOperationException($"Invalid addressable type: {diaValue}")
                        });
            }
            catch (Exception ex)
            {
                return Result.Of<IDiaValue>(ex);
            }
        }

        #endregion

        private static bool IsTrackableCategory(TypeCategory category)
        {
            return
                TypeCategory.Map.Equals(category)
                || TypeCategory.Record.Equals(category)
                || TypeCategory.Collection.Equals(category);
        }
    }
}
