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
        }

        public record TimestampOptions
        {
            public TimestampPrecision TimestampPrecision { get; set; }
        }

        public record StringOptions
        {
            private ushort lineBreakPoint = 100;

            public TextLineStyle LineStyle { get; set; }

            public ushort LineBreakPoint
            {
                get => lineBreakPoint;
                set => lineBreakPoint = value == 0
                    ? throw new ArgumentException("Line break cannot be 0")
                    : value;
            }
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
