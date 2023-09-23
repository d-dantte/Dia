using Axis.Luna.Common;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Json
{
    public readonly struct SerializerOptions
    {
        /// <summary>
        /// TODO
        /// </summary>
        public DecimalOptions Decimals { get; } = new DecimalOptions();

        /// <summary>
        /// TODO
        /// </summary>
        public TimestampOptions Timestamps { get; } = new TimestampOptions();

        /// <summary>
        /// TODO
        /// </summary>
        public ListOptions Lists { get; } = new ListOptions();

        /// <summary>
        /// TODO
        /// </summary>
        public RecordOptions Records { get; } = new RecordOptions();

        /// <summary>
        /// TODO
        /// </summary>
        public IndentationStyles IndentationStyle { get; }

        internal SerializerOptions(
            DecimalOptions decimalOptions,
            TimestampOptions timestampOptions,
            ListOptions listOptions,
            RecordOptions recordOptions,
            IndentationStyles indentationStyles)
        {
            Decimals = decimalOptions;
            Timestamps = timestampOptions;
            Lists = listOptions;
            Records = recordOptions;
            IndentationStyle = indentationStyles;
        }


        #region nested enums
        public enum IndentationStyles
        {
            Spaces,
            Tabs,
            None
        }

        public enum Case
        {
            Lowercase,
            Uppercase,
            Titlecase
        }

        public enum TimestampPrecision
        {
            MilliSecond,
            Second,
            Minute,
            Day,
            Month,
            Year
        }

        public enum TextLineStyle
        {
            Singleline,
            Multiline
        }
        #endregion

        #region nested types

        public readonly struct DecimalOptions
            : IDefaultValueProvider<DecimalOptions>
        {
            #region Default
            public bool IsDefault => Default.Equals(this);

            public static DecimalOptions Default => default;
            #endregion

            public bool UseExponentNotation { get; }

            public int MaxPrecision { get; }

            public DecimalOptions(bool useExponentNotation, ushort maxPrecision)
            {
                UseExponentNotation = useExponentNotation;
                MaxPrecision = maxPrecision;
            }
        }

        public readonly struct TimestampOptions
            : IDefaultValueProvider<TimestampPrecision>
        {
            #region Default
            public bool IsDefault => Default.Equals(this);

            public static TimestampPrecision Default => default;
            #endregion

            public TimestampPrecision TimestampPrecision { get; }

            public TimestampOptions(TimestampPrecision precision)
            {
                TimestampPrecision = precision;
            }
        }

        public readonly struct ListOptions
            : IDefaultValueProvider<ListOptions>
        {
            #region Default
            public bool IsDefault => Default.Equals(this);

            public static ListOptions Default => default;
            #endregion

            public bool UseMultipleLines { get; }

            public ListOptions(bool useMultipleLines)
            {
                UseMultipleLines = useMultipleLines;
            }
        }

        public readonly struct RecordOptions
            : IDefaultValueProvider<RecordOptions>
        {
            #region Default
            public bool IsDefault => Default.Equals(this);

            public static RecordOptions Default => default;
            #endregion

            public bool UseMultipleLines { get; }

            public RecordOptions(bool useMultipleLines)
            {
                UseMultipleLines= useMultipleLines;
            }
        }
        #endregion
    }

    public class SerializerOptionsBuilder
    {
        public SerializerOptions.DecimalOptions decimalOptions;
        public SerializerOptions.TimestampOptions timestampOptions;
        public SerializerOptions.ListOptions listOptions;
        public SerializerOptions.RecordOptions recordOptions;
        public SerializerOptions.IndentationStyles indentationStyle;

        public static SerializerOptionsBuilder NewBuilder() => new();

        public static SerializerOptionsBuilder FromBuilder(SerializerOptionsBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            return new SerializerOptionsBuilder
            {
                decimalOptions = builder.decimalOptions,
                timestampOptions = builder.timestampOptions,
                listOptions = builder.listOptions,
                recordOptions = builder.recordOptions,
                indentationStyle = builder.indentationStyle
            };
        }

        public static SerializerOptionsBuilder FromOptions(SerializerOptions options)
        {
            options.ThrowIfDefault(new ArgumentException($"Invalid {nameof(options)} instance"));

            return new SerializerOptionsBuilder
            {
                decimalOptions = options.Decimals,
                timestampOptions = options.Timestamps,
                listOptions = options.Lists,
                recordOptions = options.Records,
                indentationStyle = options.IndentationStyle
            };
        }

        private SerializerOptionsBuilder()
        {
            decimalOptions = new SerializerOptions.DecimalOptions(false, 9);
            timestampOptions = new SerializerOptions.TimestampOptions(SerializerOptions.TimestampPrecision.MilliSecond);
            listOptions = new SerializerOptions.ListOptions(true);
            recordOptions = new SerializerOptions.RecordOptions(true);
            indentationStyle = SerializerOptions.IndentationStyles.Spaces;
        }

        #region API
        public SerializerOptionsBuilder WithDecimalOptions(
            bool useExponentNotation,
            ushort maxPrecision)
        {
            decimalOptions = new SerializerOptions.DecimalOptions(useExponentNotation, maxPrecision);
            return this;
        }

        public SerializerOptionsBuilder WithTimestampOptions(SerializerOptions.TimestampPrecision precision)
        {
            timestampOptions = new SerializerOptions.TimestampOptions(precision);
            return this;
        }

        public SerializerOptionsBuilder WithListOptions(bool useMultipleLines)
        {
            listOptions = new SerializerOptions.ListOptions(useMultipleLines);
            return this;
        }

        public SerializerOptionsBuilder WithRecordOptions(bool useMultipleLines)
        {
            recordOptions = new SerializerOptions.RecordOptions(useMultipleLines);
            return this;
        }

        public SerializerOptionsBuilder WithIndentationStyle(SerializerOptions.IndentationStyles indentationStyle)
        {
            this.indentationStyle = indentationStyle;
            return this;
        }

        public SerializerOptions Build()
        {
            return new SerializerOptions(
                decimalOptions,
                timestampOptions,
                listOptions,
                recordOptions,
                indentationStyle);
        }
        #endregion
    }
}
