namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class Options
    {
        public IndentationStyle Indentation { get; }

        public string NewLine { get; }

        public AttributeOptions Attributes { get; }

        public BlobOptions Blobs { get; }

        public BoolOptions Bools { get; }

        public IntOptions Ints { get; }

        public DecimalOptions Decimals { get; }

        public TimestampOptions Timestamps { get; }

        public StringOptions Strings { get; }

        public SymbolOptions Symbols { get; }

        public SequenceOptions Sequences { get; }

        public RecordOptions Records { get; }

        public Options(
            AttributeOptions attributes,
            BlobOptions blobs,
            BoolOptions bools,
            IntOptions ints,
            DecimalOptions decimals,
            TimestampOptions timestamps,
            StringOptions strings,
            SymbolOptions symbols,
            SequenceOptions sequences,
            RecordOptions records,
            string newLine,
            IndentationStyle indentation = IndentationStyle.None)
        {
            Indentation = indentation;
            NewLine = newLine;
            Attributes = attributes ?? new();
            Blobs = blobs ?? new();
            Bools = bools ?? new();
            Ints = ints ?? new();
            Timestamps = timestamps ?? new();
            Strings = strings ?? new();
            Symbols = symbols ?? new();
            Sequences = sequences ?? new();
            Records = records ?? new();
            Decimals = decimals ?? new();
        }

        public static OptionsBuilder Builder() => new();


        #region Nested types
        public enum IndentationStyle
        {
            None = 0,
            Tabs,
            Spaces
        }

        public enum IntegerStyle
        {
            Binary,
            Hex,
            Decimal
        }

        public enum TimestampPrecision
        {
            Ticks,
            Seconds,
            Minutes,
            Hours,
            Day,
            Month,
            Year
        }

        public enum StringStyle
        {
            Verbatim,
            Inline
        }

        /// <summary>
        /// Marks options for value types that support the Canonical form.
        /// <para/>
        /// Note: this name (Canonical) should be changed
        /// </summary>
        public interface ICanonical
        {
            /// <summary>
            /// Indicates if the canonical form should be used
            /// </summary>
            bool UseCanonicalForm { get; init; }
        }


        public class AttributeOptions
        {
            /// <summary>
            /// When <see cref="Options.Indentation"/> is not <see cref="IndentationStyle.None"/>, this value indicates how many attributes
            /// will exist on a single line before carrying over to another line.
            /// <para/>
            /// This value cannot be less than 0.
            /// </summary>
            public ushort SingleLineAttributeCount { get; init; } = 7;
        }

        public class BlobOptions: ICanonical
        {
            /// <summary>
            /// When <see cref="Options.Indentation"/> is not <see cref="IndentationStyle.None"/>, this value indicates how many characters
            /// will exist on a single line before carrying over to another line.
            /// <para/>
            /// This value cannot be less than 0
            /// </summary>
            public ushort SingleLineCharacterCount { get; init; } = 70;

            public bool UseCanonicalForm { get; init; }
        }

        public class BoolOptions : ICanonical
        {
            public bool UseCanonicalForm { get; init; }
        }

        public class IntOptions : ICanonical
        {
            public IntegerStyle Style { get; init; } = IntegerStyle.Decimal;

            /// <summary>
            /// Indicates if a digit separator should be used or not. Note that with <see cref="Options.IntegerStyle.Binary"/>,
            /// digit separators are ALWAYS used, regardless of this value.
            /// </summary>
            public bool UseDigitSeparator { get; init; } = false;

            public bool UseCanonicalForm { get; init; }
        }

        public class DecimalOptions : ICanonical
        {
            public bool UseCanonicalForm { get; init; }
        }

        public class TimestampOptions
        {
            public TimestampPrecision Precision { get; }

            public bool IncludeTimezone { get; }
        }

        public class StringOptions
        {
            public StringStyle Style { get; }

            /// <summary>
            /// While in <see cref="StringStyle.Inline"/> mode, determines if the string is broken into multiple concatenated
            /// inline strings, each placed on a new line.
            /// </summary>
            public bool UseMultiline { get; }

            /// <summary>
            /// Character count after which the string is broken up into an extra line
            /// </summary>
            public ushort MultilineThreshold { get; }
        }

        public class SymbolOptions: ICanonical
        {
            public bool UseMultiline { get; }

            /// <summary>
            /// Character count after which the string is broken up into an extra line
            /// </summary>
            public ushort MultilineThreshold { get; }

            /// <summary>
            /// 
            /// </summary>
            public bool UseCanonicalForm { get; init; }
        }

        public class SequenceOptions
        {
            public bool UseMultiline { get; }
        }

        public class RecordOptions
        {
            public bool UseMultiline { get; }

            public bool AlwaysQuotePropertyName { get; }
        }
        #endregion
    }

    public class OptionsBuilder
    {
        #region Global
        public string _newline = Environment.NewLine;
        public Options.IndentationStyle _indentationStyle = Options.IndentationStyle.None;

        public OptionsBuilder WithNewLine(string newline)
        {
            ArgumentException.ThrowIfNullOrEmpty(newline);
            _newline = newline;
            return this;
        }

        public OptionsBuilder WithIndentationStyle(Options.IndentationStyle indentationStyle)
        {
            if (!Enum.IsDefined(indentationStyle))
                throw new ArgumentOutOfRangeException(nameof(indentationStyle));

            _indentationStyle = indentationStyle;
            return this;
        }
        #endregion

        #region Attributes
        private ushort _attributeOptionsSingleLineAttributeCount;

        public OptionsBuilder WithAttributeOptions(ushort singleLineAttributeCount)
        {
            _attributeOptionsSingleLineAttributeCount = singleLineAttributeCount;
            return this;
        }
        #endregion

        #region Blobs
        private ushort _blobSinglelineCharacterCount;
        private bool _blobUseCanonicalForm;

        public OptionsBuilder WithBlobSinglelineCharacterCount(ushort singleLineAttributeCount)
        {
            _blobSinglelineCharacterCount = singleLineAttributeCount;
            return this;
        }

        public OptionsBuilder WithBlobCanonicalForm(bool useCanonicalForm)
        {
            _blobUseCanonicalForm = useCanonicalForm;
            return this;
        }
        #endregion

        #region Bools
        private bool _boolUseCanonicalForm;

        public OptionsBuilder WithBoolCanonicalForm(bool useCanonicalForm)
        {
            _boolUseCanonicalForm = useCanonicalForm;
            return this;
        }
        #endregion

        #region Ints
        private bool _intUseDigitSeparator;
        private Options.IntegerStyle _intIntegerStyle;
        private bool _intUseCanonicalForm;

        public OptionsBuilder WithIntegerStyle(Options.IntegerStyle style)
        {
            _intIntegerStyle = style;
            return this;
        }

        public OptionsBuilder WithIntegerDigitSeparator(bool useDigitSeparator)
        {
            _intUseDigitSeparator = useDigitSeparator;
            return this;
        }

        public OptionsBuilder WithIntegerCanonicalForm(bool useCanonicalForm)
        {
            this._intUseCanonicalForm = useCanonicalForm;
            return this;
        }
        #endregion

        #region Decimals
        private bool _decimalUseCanonicalForm;

        public OptionsBuilder WithDecimalCanonicalForm(bool useCanonicalForm)
        {
            _decimalUseCanonicalForm = useCanonicalForm;
            return this;
        }
        #endregion


        public Options Build()
        {
            return new Options(
                new Options.AttributeOptions
                {
                    SingleLineAttributeCount = _attributeOptionsSingleLineAttributeCount
                },
                new Options.BlobOptions
                {
                    SingleLineCharacterCount = _blobSinglelineCharacterCount,
                    UseCanonicalForm = _blobUseCanonicalForm
                },
                new Options.BoolOptions
                {
                    UseCanonicalForm = _boolUseCanonicalForm
                },
                new Options.IntOptions
                {
                    Style = _intIntegerStyle,
                    UseDigitSeparator = _intUseDigitSeparator,
                    UseCanonicalForm = _intUseCanonicalForm
                },
                new Options.DecimalOptions
                {
                    UseCanonicalForm = _decimalUseCanonicalForm
                },
                new Options.TimestampOptions(),
                new Options.StringOptions(),
                new Options.SymbolOptions(),
                new Options.SequenceOptions(),
                new Options.RecordOptions(),
                _newline,
                _indentationStyle);
        }
    }
}
