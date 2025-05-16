using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;
using Axis.Dia.Json.Deserializers;
using Axis.Luna.Common;

namespace Axis.Dia.Json.Tests.Deserializers
{
    [TestClass]
    public class AttributeParserTests
    {
        #region CanonicalForm
        [TestMethod]
        public void TryParseCanonicalForm_WithInvalidParams_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => CanonicalFormParser.TryParseCanonicalType(null!, "prefix", ParseSeq, out _));

            Assert.Throws<ArgumentException>(
                () => CanonicalFormParser.TryParseCanonicalType("abcd", null!, ParseSeq, out _));

            Assert.Throws<ArgumentNullException>(
                () => CanonicalFormParser.TryParseCanonicalType("abcd", "prefix", null!, out _));
        }

        [TestMethod]
        public void TryParseCanonicalForm_WithValidNullValue_ReturnsTrue()
        {
            var result = CanonicalFormParser.TryParseCanonicalType(
                $"{CanonicalFormParser.IntPrefix}.null",
                Core.DiaType.Int,
                ParseSeq,
                out var value);

            Assert.IsTrue(result);
            Assert.IsTrue(value is DummyDiaValue);
            Assert.AreEqual<CharSequence>("null", ((DummyDiaValue)value).CharSequence);
            Assert.IsTrue(((DummyDiaValue)value).Attributes.IsDefault);
        }

        [TestMethod]
        public void TryParseCanonicalForm_WithValidNonNullValue_ReturnsTrue()
        {
            var result = CanonicalFormParser.TryParseCanonicalType(
                $"{CanonicalFormParser.IntPrefix} \t 3455",
                Core.DiaType.Int,
                ParseSeq,
                out var value);

            Assert.IsTrue(result);
            Assert.IsTrue(value is DummyDiaValue);
            Assert.AreEqual<CharSequence>("3455", ((DummyDiaValue)value).CharSequence);
            Assert.IsTrue(((DummyDiaValue)value).Attributes.IsDefault);
        }

        [TestMethod]
        public void TryParseCanonicalForm_WithValidNullValueAndAttributes_ReturnsTrue()
        {
            var result = CanonicalFormParser.TryParseCanonicalType(
                $"{CanonicalFormParser.IntPrefix}.null [@tsunami;]",
                Core.DiaType.Int,
                ParseSeq,
                out var value);

            Assert.IsTrue(result);
            Assert.IsTrue(value is DummyDiaValue);
            Assert.AreEqual<CharSequence>("null", ((DummyDiaValue)value).CharSequence);
            Assert.AreEqual(1, ((DummyDiaValue)value).Attributes.Count);
        }

        [TestMethod]
        public void TryParseCanonicalForm_WithValidNonNullValueAndAttributes_ReturnsTrue()
        {
            var result = CanonicalFormParser.TryParseCanonicalType(
                $"{CanonicalFormParser.IntPrefix}[@tsunami;] 0x3ca25",
                Core.DiaType.Int,
                ParseSeq,
                out var value);

            Assert.IsTrue(result);
            Assert.IsTrue(value is DummyDiaValue);
            Assert.AreEqual<CharSequence>("0x3ca25", ((DummyDiaValue)value).CharSequence);
            Assert.AreEqual(1, ((DummyDiaValue)value).Attributes.Count);
        }

        [TestMethod]
        public void TryParseCanonicalForm_WithInvalidPrefix_ReturnsFalse()
        {
            var result = CanonicalFormParser.TryParseCanonicalType(
                $"{CanonicalFormParser.IntPrefix} 0x3ca25",
                "#InvalidPrefix",
                ParseSeq,
                out var value);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryParseCanonicalForm_WithFailingConverter_ReturnsFalse()
        {
            var result = CanonicalFormParser.TryParseCanonicalType(
                $"{CanonicalFormParser.IntPrefix}[@tsunami;] 0x3ca25",
                Core.DiaType.Int,
                FailParseSeq,
                out var value);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(Core.DiaType.Blob, true)]
        [DataRow(Core.DiaType.Bool, true)]
        [DataRow(Core.DiaType.Decimal, true)]
        [DataRow(Core.DiaType.Duration, true)]
        [DataRow(Core.DiaType.Int, true)]
        [DataRow(Core.DiaType.Record, true)]
        [DataRow(Core.DiaType.Sequence, true)]
        [DataRow(Core.DiaType.String, true)]
        [DataRow(Core.DiaType.Symbol, true)]
        [DataRow(Core.DiaType.Timestamp, true)]
        [DataRow(Core.DiaType.Unknown, false)]
        [DataRow(Core.DiaType.Attribute, false)]
        public void ToPrefix_Tests(Core.DiaType type, bool result)
        {
            if (result)
                Assert.IsNotNull(CanonicalFormParser.ToPrefix(type));

            else Assert.Throws<ArgumentOutOfRangeException>(
                () => CanonicalFormParser.ToPrefix(type));
        }

        private bool ParseSeq((CharSequence seq, AttributeSet atts) abc, out IDiaValue v)
        {
            v = new DummyDiaValue
            {
                CharSequence = abc.seq,
                Attributes = abc.atts
            };
            return true;
        }

        private bool FailParseSeq((CharSequence seq, AttributeSet atts) abc, out IDiaValue v)
        {
            v = null!;
            return false;
        }
        #endregion

        #region Attributes
        [TestMethod]
        public void TryParseAttributes_NullReader_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                CanonicalFormParser.TryParseAttributes(null!, out _));
        }

        [TestMethod]
        public void TryParseAttributes_EmptyReader_ReturnsFalse()
        {
            var reader = new CharSequenceReader("");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsFalse(result);
            Assert.AreEqual(0, info.Attributes.Count);
            Assert.AreEqual(0, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_MissingOpeningBracket_ReturnsFalse()
        {
            var reader = new CharSequenceReader("@key:value]");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsFalse(result);
            Assert.AreEqual(0, info.Attributes.Count);
            Assert.AreEqual(0, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_EmptyAttributeSet_ReturnsTrueAndEmptySet()
        {
            var reader = new CharSequenceReader("[]");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsTrue(result);
            Assert.AreEqual(0, info.Attributes.Count);
            Assert.AreEqual(2, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_SingleAttributeNoValue_ReturnsTrue()
        {
            var reader = new CharSequenceReader("[@key;]");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsTrue(result);
            Assert.AreEqual(1, info.Attributes.Count);
            Assert.AreEqual("key", info.Attributes[0].Key);
            Assert.AreEqual(default, info.Attributes[0].Value);
            Assert.AreEqual(7, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_SingleAttributeWithValue_ReturnsTrue()
        {
            var reader = new CharSequenceReader("[@key:value;]");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsTrue(result);
            Assert.AreEqual(1, info.Attributes.Count);
            Assert.AreEqual("key", info.Attributes[0].Key);
            Assert.AreEqual("value", info.Attributes[0].Value);
            Assert.AreEqual(13, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_MultipleAttributes_ReturnsTrue()
        {
            var reader = new CharSequenceReader("[@key1:value1; @key2;@key3:value3;]");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsTrue(result);
            var atts = info.Attributes.ToArray();
            Assert.AreEqual(3, atts.Length);
            Assert.AreEqual("key1", atts[0].Key);
            Assert.AreEqual("value1", atts[0].Value);
            Assert.AreEqual("key2", atts[1].Key);
            Assert.AreEqual(default, atts[1].Value);
            Assert.AreEqual("key3", atts[2].Key);
            Assert.AreEqual("value3", atts[2].Value);
            Assert.AreEqual(35, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_MultipleAttributesWithSpaces_ReturnsTrue()
        {
            var reader = new CharSequenceReader("[@key1:value1; @key2; @key3:value3; ]");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsTrue(result);
            Assert.AreEqual(3, info.Attributes.Count);
            Assert.AreEqual(37, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_UnclosedAttributeSet_ReturnsFalse()
        {
            var reader = new CharSequenceReader("[@key:value");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsFalse(result);
            Assert.AreEqual(0, info.Attributes.Count);
            Assert.AreEqual(0, info.Count);
        }

        [TestMethod]
        public void TryParseAttributes_ExtraTextAfterClose_ReturnsFalse()
        {
            var reader = new CharSequenceReader("[@key:value]abc");
            bool result = CanonicalFormParser.TryParseAttributes(reader, out var info);
            Assert.IsFalse(result);
            Assert.AreEqual(0, info.Attributes.Count);
            Assert.AreEqual(0, info.Count);
        }
        #endregion

        #region TryParseLineSpace
        [TestMethod]
        public void TryParseLineSpace_NullReader_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                CanonicalFormParser.TryParseLineSpace(null!, out _));
        }

        [TestMethod]
        public void TryParseLineSpace_EmptyReader_ReturnsFalse()
        {
            var reader = new CharSequenceReader("");
            bool result = CanonicalFormParser.TryParseLineSpace(reader, out var lineSpace);
            Assert.IsFalse(result);
            Assert.AreEqual(default, lineSpace);
        }

        [TestMethod]
        public void TryParseLineSpace_SingleSpace_ReturnsTrue()
        {
            var reader = new CharSequenceReader(" ");
            bool result = CanonicalFormParser.TryParseLineSpace(reader, out var lineSpace);
            Assert.IsTrue(result);
            Assert.AreEqual(" ", lineSpace.ToString());
        }

        [TestMethod]
        public void TryParseLineSpace_MultipleSpaces_ReturnsTrue()
        {
            var reader = new CharSequenceReader("   ");
            bool result = CanonicalFormParser.TryParseLineSpace(reader, out var lineSpace);
            Assert.IsTrue(result);
            Assert.AreEqual("   ", lineSpace.ToString());
        }

        [TestMethod]
        public void TryParseLineSpace_SpaceAndNonSpace_ReturnsFalse()
        {
            var reader = new CharSequenceReader(" a");
            bool result = CanonicalFormParser.TryParseLineSpace(reader, out var lineSpace);
            Assert.IsTrue(result);
            Assert.AreEqual<CharSequence>(" ", lineSpace);
            Assert.AreEqual(1, reader.CurrentIndex);
        }

        [TestMethod]
        public void TryParseLineSpace_OnlyNonSpace_ReturnsFalse()
        {
            var reader = new CharSequenceReader("abc");
            bool result = CanonicalFormParser.TryParseLineSpace(reader, out var lineSpace);
            Assert.IsFalse(result);
            Assert.AreEqual(default, lineSpace);
            Assert.AreEqual(0, reader.CurrentIndex);
        }
        #endregion

        #region TryParseAttribute
        [TestMethod]
        public void TryParseAttribute_NullReader_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                CanonicalFormParser.TryParseAttribute(null!, out _));
        }

        [TestMethod]
        public void TryParseAttribute_EmptyReader_ReturnsFalse()
        {
            var reader = new CharSequenceReader("");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsFalse(result);
            Assert.AreEqual(default, attribute);
        }

        [TestMethod]
        public void TryParseAttribute_MissingAtSymbol_ReturnsFalse()
        {
            var reader = new CharSequenceReader("key:value;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsFalse(result);
            Assert.AreEqual(default, attribute);
        }

        [TestMethod]
        public void TryParseAttribute_OnlyAtSymbol_ReturnsFalse()
        {
            var reader = new CharSequenceReader("@");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsFalse(result);
            Assert.AreEqual(default, attribute);
        }

        [TestMethod]
        public void TryParseAttribute_KeyOnly_ReturnsTrue()
        {
            var reader = new CharSequenceReader("@key;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsTrue(result);
            Assert.AreEqual("key", attribute!.Value.Key);
            Assert.AreEqual(default, attribute.Value.Value);
        }

        [TestMethod]
        public void TryParseAttribute_KeyAndValue_ReturnsTrue()
        {
            var reader = new CharSequenceReader("@key:value;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsTrue(result);
            Assert.AreEqual("key", attribute!.Value.Key);
            Assert.AreEqual("value", attribute.Value.Value);
        }

        [TestMethod]
        public void TryParseAttribute_KeyAndEmptyValue_ReturnsTrue()
        {
            var reader = new CharSequenceReader("@key:;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("@key:value;", true)]
        [DataRow("@key;", true)]
        [DataRow("@key:", false)]
        [DataRow("@key:value", false)]
        public void TryParseAttribute_IncompleteInput_ReturnsFalse(string input, bool expectedResult)
        {
            var reader = new CharSequenceReader(input);
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TryParseAttribute_KeyWithValidChars_ReturnsTrue()
        {
            var reader = new CharSequenceReader("@key_1-2;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsTrue(result);
            Assert.AreEqual("key_1-2", attribute!.Value.Key);
            Assert.AreEqual(default, attribute.Value.Value);
        }

        [TestMethod]
        public void TryParseAttribute_KeyWithInvalidChars_ReturnsFalse()
        {
            var reader = new CharSequenceReader("@key+;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsFalse(result);
            Assert.AreEqual(default, attribute);
        }

        [TestMethod]
        public void TryParseAttribute_ValueWithValidChars_ReturnsTrue()
        {
            var reader = new CharSequenceReader("@key:value_1-2;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsTrue(result);
            Assert.AreEqual("key", attribute!.Value.Key);
            Assert.AreEqual("value_1-2", attribute.Value.Value);
        }

        [TestMethod]
        public void TryParseAttribute_ValueWithSemicolonEscape_ReturnsTrue()
        {
            var reader = new CharSequenceReader("@key:value\\;;");
            bool result = CanonicalFormParser.TryParseAttribute(reader, out var attribute);
            Assert.IsTrue(result);
            Assert.AreEqual("key", attribute!.Value.Key);
            Assert.AreEqual("value;", attribute.Value.Value);
        }
        #endregion

        #region Nested types
        internal class DummyDiaValue : IDiaValue
        {
            public DiaType Type => DiaType.Unknown;

            public CharSequence CharSequence { get; set; }

            public AttributeSet Attributes { get; set; }
        }
        #endregion
    }
} 
