using System.Collections.Immutable;

namespace Axis.Dia.TypeConverter.Dia
{
    public class Options
    {
        public ImmutableDictionary<string, object> CustomOptions { get; }

        public ImmutableArray<IDiaConverter> CustomConverters { get; }

        private Options(
            IDictionary<string, object> customOptions,
            params IDiaConverter[] customConverters)
        {
            ArgumentNullException.ThrowIfNull(customOptions);

            CustomConverters = customConverters.ToImmutableArray();
            CustomOptions = customOptions.ToImmutableDictionary();
        }

        public static Builder NewBuilder() => new Builder();


        public class Builder
        {
            private readonly List<IDiaConverter> customConverters = new();

            internal Builder() { }

            #region Custom Converters
            public Builder AppendConverter(IDiaConverter converter)
            {
                ArgumentNullException.ThrowIfNull(converter);
                customConverters.Add(converter);

                return this;
            }
            #endregion

            #region Custom Options
            public IDictionary<string, object> CustomOptions { get; } = new Dictionary<string, object>();
            #endregion

            public Options Build()
            {
                return new Options(CustomOptions, [.. customConverters]);
            }
        }
    }
}
