using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Dia.Typhon.Dia
{
    public class ConverterManager
    {
        private readonly Options options;
        private readonly Dictionary<TypeInfo, IDiaConverter?> converterCache = new();

        public ImmutableArray<IDiaConverter> DefaultConverters { get; } = ImmutableArray.Create<IDiaConverter>(
            SimpleTypeConverter.DefaultInstance,
            EnumConverter.DefaultInstance,
            SequenceConverter.DefaultInstance,
            RecordConverter.DefaultInstance);

        public ImmutableArray<IDiaConverter> CustomConverters => options.CustomConverters;

        internal ConverterManager(Options options)
        {
            ArgumentNullException.ThrowIfNull(options);
            this.options = options;
        }

        public bool TryGetConverter(TypeInfo typeInfo, out IDiaConverter? converter)
        {
            converter = converterCache.GetOrAdd(typeInfo, ti =>
            {
                return CustomConverters.FirstOrDefault(cc => cc.CanConvert(typeInfo))
                        ?? DefaultConverters.FirstOrDefault(cc => cc.CanConvert(typeInfo));
            });

            return converter is not null;
        }
    }
}
