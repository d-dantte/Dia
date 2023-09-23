using static Axis.Dia.Convert.Axon.SerializerOptions;

namespace Axis.Dia.Convert.Axon
{
    public readonly struct SerializerOptions
    {
        /// <summary>
        /// TODO
        /// </summary>
        public IntOptions Ints { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public BoolOptions Bools { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public DecimalOptions Decimals { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public TimestampOptions Timestamps { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public StringOptions Strings { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public BlobOptions Blobs { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public ListOptions Lists { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public RecordOptions Records { get; }

        /// <summary>
        /// TODO
        /// </summary>
        public IndentationStyles IndentationStyle { get; }

        internal SerializerOptions(
            BoolOptions boolOptions,
            IntOptions intOptions,
            DecimalOptions decimalOptions,
            TimestampOptions timestampOptions,
            StringOptions stringOptions,
            BlobOptions blobOptions,
            ListOptions listOptions,
            RecordOptions recordOptions,
            IndentationStyles indentationStyle)
        {
            ArgumentNullException.ThrowIfNull(boolOptions);
            ArgumentNullException.ThrowIfNull(intOptions);
            ArgumentNullException.ThrowIfNull(decimalOptions);
            ArgumentNullException.ThrowIfNull(timestampOptions);
            ArgumentNullException.ThrowIfNull(stringOptions);
            ArgumentNullException.ThrowIfNull(blobOptions);
            ArgumentNullException.ThrowIfNull(listOptions);
            ArgumentNullException.ThrowIfNull(recordOptions);

            Bools = boolOptions with { };
            Ints = intOptions with { };
            Decimals = decimalOptions with { };
            Timestamps = timestampOptions with { };
            Strings = stringOptions with { };
            Blobs = blobOptions with { };
            Lists = listOptions with { };
            Records = recordOptions with { };
            IndentationStyle = indentationStyle;
        }


        #region nested enums
        public enum IndentationStyles
        {
            None,
            Tabs,
            Spaces
        }

        public enum IntFormat
        {
            Decimal,
            BigBinary,
            SmallBinary,
            BigHex,
            SmallHex
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

        public record IntOptions
        {
            public IntFormat NumberFormat { get; set; }

            public bool UseDigitSeparator { get; set; }
        }

        public record BoolOptions
        {
            public Case ValueCase { get; set; }
        }

        public record DecimalOptions
        {
            public bool UseExponentNotation { get; set; }

            public int MaxPrecision { get; set; } = 64;
        }

        public record TimestampOptions
        {
            public TimestampPrecision TimestampPrecision { get; set; }
        }

        public record StringOptions
        {
            private ushort maxLineLength = 150;

            /// <summary>
            /// Text line style.
            /// </summary>
            public TextLineStyle LineStyle { get; set; }

            /// <summary>
            /// Maximum number of text characters in each line. Words that cause a line to exceed this
            /// boundary are placed on a new line. If a single word is longer than the boundary, it is placed
            /// on it's own line.
            /// </summary>
            public ushort MaxLineLength
            {
                get => maxLineLength;
                set => maxLineLength = value == 0
                    ? throw new ArgumentException("Maximum line length cannot be 0")
                    : value;
            }

            /// <summary>
            /// Indicates that text in <see cref="TextLineStyle.Multiline"/> mode should be aligned
            /// </summary>
            public bool IsTextAligned { get; set; }

            /// <summary>
            /// Number of spaces to use when aligning text in <see cref="TextLineStyle.Multiline"/> mode.
            /// </summary>
            public ushort AlignmentIndentation { get; set; } = 8;
        }

        public record BlobOptions
        {
            private ushort maxLineLength = 150;

            /// <summary>
            /// Text line style.
            /// </summary>
            public TextLineStyle LineStyle { get; set; }

            /// <summary>
            /// Maximum number of text characters in each line. Words that cause a line to exceed this
            /// boundary are placed on a new line. If a single word is longer than the boundary, it is placed
            /// on it's own line.
            /// </summary>
            public ushort MaxLineLength
            {
                get => maxLineLength;
                set => maxLineLength = value == 0
                    ? throw new ArgumentException("Maximum line length cannot be 0")
                    : value;
            }
        }

        public record ListOptions
        {
            public bool UseMultipleLines { get; set; }
        }

        public record RecordOptions
        {
            public bool UseMultipleLines { get; set; }

            public bool UseQuotedIdentifierPropertyNames { get; set; }
        }
        #endregion
    }

    public class SerializerOptionsBuilder
    {
        private IntOptions ints;
        private BoolOptions bools;
        private DecimalOptions decimals;
        private TimestampOptions timestamps;
        private StringOptions strings;
        private BlobOptions blobs;
        private ListOptions lists;
        private RecordOptions records;
        private IndentationStyles indentationStyle;

        private SerializerOptionsBuilder()
        {
            ints = new IntOptions();
            bools = new BoolOptions();
            decimals = new DecimalOptions();
            timestamps = new TimestampOptions();
            strings = new StringOptions();
            blobs = new BlobOptions();
            lists = new ListOptions();
            records = new RecordOptions();
        }

        public static SerializerOptionsBuilder NewBuilder() => new();

        public SerializerOptionsBuilder WithIntOptions(IntOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            ints = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithDecimalOptions(DecimalOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            decimals = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithInstantOptions(TimestampOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            timestamps = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithBoolOptions(BoolOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            bools = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithStringOptions(StringOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            strings = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithBlobOptions(BlobOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            blobs = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithListOptions(ListOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            lists = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithRecordOptions(RecordOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            records = options with { };
            return this;
        }

        public SerializerOptionsBuilder WithIndentationStyle(IndentationStyles indentationStyle)
        {
            this.indentationStyle = indentationStyle;
            return this;
        }

        public SerializerOptionsBuilder FromBuilder(SerializerOptionsBuilder builder)
        {
            return this
                .WithBoolOptions(builder.bools)
                .WithIntOptions(builder.ints)
                .WithDecimalOptions(builder.decimals)
                .WithInstantOptions(builder.timestamps)
                .WithStringOptions(builder.strings)
                .WithBlobOptions(builder.blobs)
                .WithListOptions(builder.lists)
                .WithRecordOptions(builder.records)
                .WithIndentationStyle(builder.indentationStyle);
        }

        public SerializerOptions Build()
        {
            return new SerializerOptions(
                bools, ints, decimals,
                timestamps, strings, blobs,
                lists, records, indentationStyle);
        }
    }
}
