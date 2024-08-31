using System.Collections.Immutable;

namespace Axis.Dia.TypeConverter.Clr
{
    public class Options
    {
        public ImmutableArray<IClrConverter> CustomConverters { get; }

        private Options(
            params IClrConverter[] customConverters)
        {
            CustomConverters = customConverters.ToImmutableArray();
        }

        public static Builder NewBuilder() => new();


        #region Nested Types
        public class Builder
        {
            private readonly List<IClrConverter> customConverters = new();

            internal Builder() { }

            public Builder AppendConverter(IClrConverter converter)
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
        #endregion
    }
}
