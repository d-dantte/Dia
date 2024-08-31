using System.Collections.Immutable;

namespace Axis.Dia.TypeConverter.Dia
{
    public class Options
    {
        public ImmutableArray<IDiaConverter> CustomConverters { get; }

        private Options(
            params IDiaConverter[] customConverters)
        {
            CustomConverters = customConverters.ToImmutableArray();
        }

        public static Builder NewBuilder() => new Builder();


        public class Builder
        {
            private readonly List<IDiaConverter> customConverters = new();

            internal Builder() { }

            public Builder AppendConverter(IDiaConverter converter)
            {
                ArgumentNullException.ThrowIfNull(converter);
                customConverters.Add(converter);

                return this;
            }

            public Options Build()
            {
                return new Options([.. customConverters]);
            }
        }
    }
}
