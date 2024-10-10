using Axis.Dia.Core;
using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Dia.Typhon.Clr
{
    public class ConverterManager
    {
        private readonly Options options;
        private readonly Dictionary<TypeInfo, IClrConverter?> converterCache = new();

        public ImmutableArray<IClrConverter> DefaultConverters { get; } = ImmutableArray.Create<IClrConverter>(
            SimpleTypeConverter.DefaultInstance,
            EnumConverter.DefaultInstance);
            //,SequenceConverter.DefaultInstance,
            //RecordConverter.DefaultInstance);

        public ImmutableArray<IClrConverter> CustomConverters => options.CustomConverters;

        internal ConverterManager(Options options)
        {
            ArgumentNullException.ThrowIfNull(options);
            this.options = options;
        }

        public bool TryGetConverter(DiaType sourceType, TypeInfo destinationTypeInfo, out IClrConverter? converter)
        {
            converter = converterCache.GetOrAdd(destinationTypeInfo, ti =>
            {
                return CustomConverters.FirstOrDefault(cc => cc.CanConvert(sourceType, destinationTypeInfo))
                        ?? DefaultConverters.FirstOrDefault(cc => cc.CanConvert(sourceType, destinationTypeInfo));
            });

            return converter is not null;
        }
    }
}
