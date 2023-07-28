namespace Axis.Dia.IO.Text
{
    public class TextSerializerOptions
    {
        /// <summary>
        /// TODO
        /// </summary>
        public IntOptions Ints { get; } = new IntOptions();

        /// <summary>
        /// TODO
        /// </summary>
        public BoolOptions Bools { get; } = new BoolOptions();

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
        public StringOptions Strings { get; } = new StringOptions();

        /// <summary>
        /// TODO
        /// </summary>
        public BlobOptions Blobs { get; } = new BlobOptions();

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
        public IndentationStyles IndentationStyle { get; set; }


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

        public record BlobOptions: StringOptions
        {
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
}
