using System.Collections.Immutable;

namespace Axis.Dia.TypeConverter.Clr
{
    public class Options
    {
        public ImmutableDictionary<string, object> CustomOptions { get; }

        public ImmutableArray<IClrConverter> CustomConverters { get; }

        public Record Records { get; }

        private Options(
            Record record,
            IDictionary<string, object> customOptions,
            params IClrConverter[] customConverters)
        {
            ArgumentNullException.ThrowIfNull(record);
            ArgumentNullException.ThrowIfNull(customOptions);

            Records = record;
            CustomConverters = customConverters.ToImmutableArray();
            CustomOptions = customOptions.ToImmutableDictionary();
        }

        public static Builder NewBuilder() => new();


        #region Nested Types
        public class Builder
        {
            #region Custom Converters
            private readonly List<IClrConverter> customConverters = new();

            public Builder AppendConverter(IClrConverter converter)
            {
                ArgumentNullException.ThrowIfNull(converter);
                customConverters.Add(converter);

                return this;
            }
            #endregion

            #region Custom Options
            public IDictionary<string, object> CustomOptions { get; } = new Dictionary<string, object>();
            #endregion

            #region Records
            private bool records_IncludePropertyAttributesInMapKey = false;
            private bool records_IgnoreNulls = true;
            private Func<PropertyNameSource, string, string> records_NameNormalizer = RecordConverter.NormalizePropertyName;

            public Builder WithIncludePropertyAttributesInMapKeyForRecords(bool value)
            {
                records_IncludePropertyAttributesInMapKey = value;
                return this;
            }

            public Builder WithIgnoreNullsForRecords(bool value)
            {
                records_IgnoreNulls = value;
                return this;
            }

            public Builder WithPropertyNameNormalizerForRecords(Func<PropertyNameSource, string, string> nameNormalizer)
            {
                ArgumentNullException.ThrowIfNull(nameNormalizer);

                records_NameNormalizer = nameNormalizer;
                return this;
            }
            #endregion

            internal Builder() { }

            public Options Build()
            {
                return new Options(
                    new Record
                    {
                        IncludePropertyAttributesInMapKey = this.records_IncludePropertyAttributesInMapKey,
                        IgnoreNulls = this.records_IgnoreNulls,
                        PropertyNameNormalizer = this.records_NameNormalizer
                    },
                    CustomOptions,
                    [.. customConverters]);
            }
        }

        public enum PropertyNameSource
        {
            Dia,
            Clr
        }

        public record Record
        {
            public bool IncludePropertyAttributesInMapKey { get; internal set; }

            public bool IgnoreNulls { get; internal set; }

            public Func<PropertyNameSource, string, string> PropertyNameNormalizer { get; internal set; }

            internal Record()
            {
                PropertyNameNormalizer = null!;
            }
        }
        #endregion
    }
}
