using static Axis.Dia.Axon.Serializers.Options;

namespace Axis.Dia.Axon.Serializers
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

        public DurationOptions Durations { get; }

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
            DurationOptions durations,
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
            Durations = durations ?? new();
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
            Decimal,
            Binary,
            Hex
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
            Inline,
            Verbatim
        }

        public enum DecimalNotation
        {
            Scientific,
            NonScientific
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

        public class BlobOptions
        {
            /// <summary>
            /// When <see cref="Options.Indentation"/> is not <see cref="IndentationStyle.None"/>, this value indicates how many characters
            /// will exist on a single line before carrying over to another line.
            /// <para/>
            /// This value cannot be less than 0
            /// </summary>
            public ushort SingleLineCharacterCount { get; init; } = 100;
        }

        public class BoolOptions
        {
        }

        public class IntOptions
        {
            public IntegerStyle Style { get; init; } = IntegerStyle.Decimal;

            /// <summary>
            /// Indicates if a digit separator should be used or not. Note that with <see cref="Options.IntegerStyle.Binary"/>,
            /// digit separators are ALWAYS used, regardless of this value.
            /// </summary>
            public bool UseDigitSeparator { get; init; } = false;
        }

        public class DecimalOptions
        {
            public DecimalNotation Notation { get; init; } = DecimalNotation.Scientific;
        }

        public class DurationOptions
        {
        }

        public class TimestampOptions
        {
            public TimestampPrecision Precision { get; init; }

            public bool IncludeTimezone { get; init; } = true;
        }

        public class StringOptions
        {
            public StringStyle Style { get; init; }

            /// <summary>
            /// Indicates if the string should be broken into multiple lines, and at what character count the break should occur.
            /// A null value indicates use of singleline, while non-null values indicate the character count at which the text is
            /// broken into new lines.
            /// </summary>
            public ushort? LineThreshold { get; init; } = null;
        }

        public class SymbolOptions
        {
            /// <summary>
            /// Indicates if the string should be broken into multiple lines, and at what character count the break should occur.
            /// A null value indicates use of singleline, while non-null values indicate the character count at which the text is
            /// broken into new lines.
            /// </summary>
            public ushort? LineThreshold { get; init; } = null;
        }

        public class SequenceOptions
        {
            public bool UseMultiline { get; init; }
        }

        public class RecordOptions
        {
            public bool UseMultiline { get; init; }

            public bool AlwaysQuotePropertyName { get; init; }
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
        private ushort _attributeOptionsSingleLineAttributeCount = 7;

        public OptionsBuilder WithAttributeOptions(ushort singleLineAttributeCount)
        {
            _attributeOptionsSingleLineAttributeCount = singleLineAttributeCount;
            return this;
        }
        #endregion

        #region Blobs
        private ushort _blobSinglelineCharacterCount = 100;

        public OptionsBuilder WithBlobSinglelineCharacterCount(ushort singleLineCharacterCount)
        {
            if (singleLineCharacterCount == 0)
                throw new ArgumentException($"Invalid {nameof(singleLineCharacterCount)}: 0 is forbidden");

            _blobSinglelineCharacterCount = singleLineCharacterCount;
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

        public OptionsBuilder WithIntegerStyle(Options.IntegerStyle style)
        {
            if (!Enum.IsDefined(style))
                throw new ArgumentOutOfRangeException(nameof(style));

            _intIntegerStyle = style;
            return this;
        }

        public OptionsBuilder WithIntegerDigitSeparator(bool useDigitSeparator)
        {
            _intUseDigitSeparator = useDigitSeparator;
            return this;
        }
        #endregion

        #region Decimals
        private DecimalNotation _decimalNotation;

        public OptionsBuilder WithDecimalNotation(DecimalNotation notation)
        {
            if (!Enum.IsDefined(notation))
                throw new ArgumentOutOfRangeException(nameof(notation));

            _decimalNotation = notation;
            return this;
        }
        #endregion

        #region String
        private StringStyle _stringStyle;
        private ushort? _stringLineThreshold;

        public OptionsBuilder WithStringStyle(StringStyle style)
        {
            if (!Enum.IsDefined(style))
                throw new ArgumentOutOfRangeException(nameof(style));

            _stringStyle = style;
            return this;
        }

        public OptionsBuilder WithStringLineThreshold(ushort? lineThreshold = 70)
        {
            if (lineThreshold == 0)
                throw new ArgumentException($"Invalid {nameof(lineThreshold)}: 0 is forbidden");

            _stringLineThreshold = lineThreshold;
            return this;
        }

        #endregion

        #region Symbol
        private ushort? _symbolLineThreshold;

        public OptionsBuilder WithSymbolLineThreshold(ushort? lineThreshold = 70)
        {
            if (lineThreshold == 0)
                throw new ArgumentException($"Invalid {nameof(lineThreshold)}: 0 is forbidden");

            _symbolLineThreshold = lineThreshold;
            return this;
        }

        #endregion

        #region Timestamp
        private bool _timestampIncludeTimezone = true;
        private TimestampPrecision _timestampPrecision;

        public OptionsBuilder WithTimestampTimezone(bool includeTimezone)
        {
            _timestampIncludeTimezone = includeTimezone;
            return this;
        }

        public OptionsBuilder WithTimestampPrecision(TimestampPrecision timestampPrecision)
        {
            if (!Enum.IsDefined(timestampPrecision))
                throw new ArgumentOutOfRangeException(nameof(timestampPrecision));

            _timestampPrecision = timestampPrecision;
            return this;
        }

        #endregion

        #region Sequences
        private bool _sequenceUseMultiline = false;

        public OptionsBuilder WithSequenceUseMultiline(bool useMultiline)
        {
            _sequenceUseMultiline = useMultiline;
            return this;
        }
        #endregion

        #region Record
        private bool _recordUseMultiline = false;
        private bool _recordAlwaysQuotePropertyName = false;

        public OptionsBuilder WithRecordUseMultiline(bool useMultiline)
        {
            _recordUseMultiline = useMultiline;
            return this;
        }

        public OptionsBuilder WithRecordAlwaysQuotePropertyName(bool alwaysQuotePropertyName)
        {
            _recordAlwaysQuotePropertyName = alwaysQuotePropertyName;
            return this;
        }
        #endregion

        public Options Build()
        {
            return new Options(
                new AttributeOptions
                {
                    SingleLineAttributeCount = _attributeOptionsSingleLineAttributeCount
                },
                new BlobOptions
                {
                    SingleLineCharacterCount = _blobSinglelineCharacterCount
                },
                new BoolOptions(),
                new IntOptions
                {
                    Style = _intIntegerStyle,
                    UseDigitSeparator = _intUseDigitSeparator
                },
                new DecimalOptions
                {
                    Notation = _decimalNotation
                },
                new DurationOptions(),
                new TimestampOptions
                {
                    IncludeTimezone = _timestampIncludeTimezone,
                    Precision = _timestampPrecision
                },
                new StringOptions
                {
                    Style = _stringStyle,
                    LineThreshold = _stringLineThreshold
                },
                new SymbolOptions
                {
                    LineThreshold = _symbolLineThreshold
                },
                new SequenceOptions
                {
                    UseMultiline = _sequenceUseMultiline
                },
                new RecordOptions
                {
                    AlwaysQuotePropertyName = _recordAlwaysQuotePropertyName,
                    UseMultiline = _recordUseMultiline
                },
                _newline,
                _indentationStyle);
        }
    }
}
