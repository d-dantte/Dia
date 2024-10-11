using Axis.Dia.Axon.Lang;
using Axis.Pulsar.Core.CST;
using Axis.Pulsar.Core.Utils;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class AxonGrammar
    {
        [TestMethod]
        public void ImportLanguage_Tests()
        {
            var langCxt = GrammarUtil.LanguageContext;
            Assert.IsNotNull(langCxt);
        }

        #region Comments
        [TestMethod]
        public void LineComment_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["line-comment"];

            var parsed = prod.TryRecognize(
                "// stuff for the comment", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("// stuff for the comment", node.Tokens);

            parsed = prod.TryRecognize(
                "// stuff for the comment\r", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("// stuff for the comment", node.Tokens);

            parsed = prod.TryRecognize(
                "// stuff for the comment\n", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("// stuff for the comment", node.Tokens);
        }

        [TestMethod]
        public void BlockComment_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["block-comment"];

            var parsed = prod.TryRecognize(
                "/* stuff for the comment */", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("/* stuff for the comment */", node.Tokens);

            parsed = prod.TryRecognize(
                "/* stuff\nfor\rthe\r\ncomment\n\r */", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("/* stuff\nfor\rthe\r\ncomment\n\r */", node.Tokens);
        }
        #endregion

        #region Spaces
        [TestMethod]
        public void BlockSpace_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["block-space"];

            var parsed = prod.TryRecognize(
                "\v\t\r\n\t\t\r\v", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("\v\t\r\n\t\t\r\v", node.Tokens);

            var str = @"
// some line comment
// other line comment after line-break
/* block
ish
 -ish comment
        with multiple
lines
*/


            //another line comment
";
            parsed = prod.TryRecognize(
                str, "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>(str, node.Tokens);
        }
        #endregion

        #region Attribute
        [TestMethod]
        public void AttributeList_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["attribute-list"];

            var parsed = prod.TryRecognize(
                "@att;", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("@att;", node.Tokens);

            parsed = prod.TryRecognize(
                "@att;@att2; @att.3:value;", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@att;@att2; @att.3:value;", node.Tokens);
            var nodes = node
                .FindNodes("attribute")
                .ToArray();
            Assert.AreEqual(3, nodes.Length);
            nodes = node
                .FindNodes("attribute/attribute-flag|attribute-kvp")
                .ToArray();
            Assert.AreEqual(3, nodes.Length);
        }

        [TestMethod]
        public void AttributeName_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["attribute-name"];

            var parsed = prod.TryRecognize(
                "@att", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("@att", node.Tokens);

            parsed = prod.TryRecognize(
                "@att.bleh.jjj", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@att.bleh.jjj", node.Tokens);
        }
        #endregion

        #region Bool
        [TestMethod]
        public void Bool_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-bool"];

            var parsed = prod.TryRecognize(
                "true", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("true", node.Tokens);

            parsed = prod.TryRecognize(
                "TRue", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("TRue", node.Tokens);

            parsed = prod.TryRecognize(
                "false", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("false", node.Tokens);

            parsed = prod.TryRecognize(
                "FALSE", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("FALSE", node.Tokens);

            parsed = prod.TryRecognize(
                "null.bool", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.bool", node.Tokens);

            parsed = prod.TryRecognize(
                "@never-winter;null.bool", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@never-winter;null.bool", node.Tokens);

            parsed = prod.TryRecognize(
                "@never-winter:yes; null.bool", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@never-winter:yes; null.bool", node.Tokens);
        }
        #endregion

        #region Int
        [TestMethod]
        public void Int_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-int"];

            #region Regular
            var parsed = prod.TryRecognize(
                "45432454", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("45432454", node.Tokens);

            parsed = prod.TryRecognize(
                "-3432", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-3432", node.Tokens);

            parsed = prod.TryRecognize(
                "-0", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-0", node.Tokens);

            parsed = prod.TryRecognize(
                "-1_000_000", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-1_000_000", node.Tokens);
            #endregion

            #region Hex
            parsed = prod.TryRecognize(
                "0x0", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("0x0", node.Tokens);

            parsed = prod.TryRecognize(
                "-0x1c", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-0x1c", node.Tokens);

            parsed = prod.TryRecognize(
                "-0x1c_ee_2f", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-0x1c_ee_2f", node.Tokens);
            #endregion

            #region Binary
            parsed = prod.TryRecognize(
                "0b0", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("0b0", node.Tokens);

            parsed = prod.TryRecognize(
                "0b0001_1101_0000_1010", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("0b0001_1101_0000_1010", node.Tokens);

            parsed = prod.TryRecognize(
                "-0b0001_1101_0000_1010", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-0b0001_1101_0000_1010", node.Tokens);
            #endregion

            parsed = prod.TryRecognize(
                "null.int", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.int", node.Tokens);
        }
        #endregion

        #region Decimal
        [TestMethod]
        public void Decimal_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-decimal"];

            #region Regular
            var parsed = prod.TryRecognize(
                "45432454.655", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("45432454.655", node.Tokens);

            parsed = prod.TryRecognize(
                "-0.000_54", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-0.000_54", node.Tokens);
            #endregion

            #region Scientific
            parsed = prod.TryRecognize(
                "0E0", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("0E0", node.Tokens);

            parsed = prod.TryRecognize(
                "-0E0", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-0E0", node.Tokens);

            parsed = prod.TryRecognize(
                "-0.0E0", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("-0.0E0", node.Tokens);

            parsed = prod.TryRecognize(
                "54E-1", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("54E-1", node.Tokens);

            parsed = prod.TryRecognize(
                "0.54e-12", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("0.54e-12", node.Tokens);
            #endregion

            parsed = prod.TryRecognize(
                "null.decimal", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.decimal", node.Tokens);
        }
        #endregion

        #region Timestamp
        [TestMethod]
        public void Timestamp_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-timestamp"];

            var parsed = prod.TryRecognize(
                "'T 2007-11-30 21:05:04'", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("'T 2007-11-30 21:05:04'", node.Tokens);

            parsed = prod.TryRecognize(
                "'T 2022 Z'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'T 2022 Z'", node.Tokens);

            parsed = prod.TryRecognize(
                "'t 2022-12 -02:00'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'t 2022-12 -02:00'", node.Tokens);

            parsed = prod.TryRecognize(
                "'TS 2022-12-31'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'TS 2022-12-31'", node.Tokens);

            parsed = prod.TryRecognize(
                "'T 2022-12-31 19'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'T 2022-12-31 19'", node.Tokens);

            parsed = prod.TryRecognize(
                "'TimeStamp 2022-12-31 19:49'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'TimeStamp 2022-12-31 19:49'", node.Tokens);

            parsed = prod.TryRecognize(
                "'T 2022-12-31 19:49:03'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'T 2022-12-31 19:49:03'", node.Tokens);

            parsed = prod.TryRecognize(
                "'T 2022-12-31 19:49:03.554410'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'T 2022-12-31 19:49:03.554410'", node.Tokens);

            parsed = prod.TryRecognize(
                "null.timestamp", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.timestamp", node.Tokens);
        }
        #endregion

        #region Duration
        [TestMethod]
        public void Duration_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-duration"];

            var parsed = prod.TryRecognize(
                "'d 233.00:00:00'", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("'d 233.00:00:00'", node.Tokens);

            parsed = prod.TryRecognize(
                "'D 00:04:00'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'D 00:04:00'", node.Tokens);

            parsed = prod.TryRecognize(
                "@flag; 'duration 2.00:59:59.554443'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@flag; 'duration 2.00:59:59.554443'", node.Tokens);

            parsed = prod.TryRecognize(
                "null.duration", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.duration", node.Tokens);
        }
        #endregion

        #region Strings
        [TestMethod]
        public void String_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-string"];

            var parsed = prod.TryRecognize(
                "\"\"", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("\"\"", node.Tokens);

            parsed = prod.TryRecognize(
                "\"\\\"\"", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("\"\\\"\"", node.Tokens);

            parsed = prod.TryRecognize(
                "\"ab\\x02ccd\"", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("\"ab\\x02ccd\"", node.Tokens);

            parsed = prod.TryRecognize(
                "\"ab\\x02ccd\" + \"123\"", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("\"ab\\x02ccd\" + \"123\"", node.Tokens);

            parsed = prod.TryRecognize(
                "@att; null.string", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@att; null.string", node.Tokens);

            parsed = prod.TryRecognize(
                "@att; `abcd\nxyz\r\nafter the new line\nother stuff`", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@att; `abcd\nxyz\r\nafter the new line\nother stuff`", node.Tokens);
        }
        #endregion

        #region Symbols
        [TestMethod]
        public void Symbol_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-symbol"];

            var parsed = prod.TryRecognize(
                "'s '", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("'s '", node.Tokens);

            parsed = prod.TryRecognize(
                "'s \\''", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'s \\''", node.Tokens);

            parsed = prod.TryRecognize(
                "'sym ab\\x02ccd'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'sym ab\\x02ccd'", node.Tokens);

            parsed = prod.TryRecognize(
                "'Symbol ab\\x02ccd' + '123'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'Symbol ab\\x02ccd' + '123'", node.Tokens);

            parsed = prod.TryRecognize(
                "null.symbol", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.symbol", node.Tokens);
        }
        #endregion

        #region Blob
        [TestMethod]
        public void Blob_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-blob"];

            var parsed = prod.TryRecognize(
                "'b AQID'", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("'b AQID'", node.Tokens);

            parsed = prod.TryRecognize(
                "'B AQID'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'B AQID'", node.Tokens);

            parsed = prod.TryRecognize(
                "'Blob AQ' + 'ID'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'Blob AQ' + 'ID'", node.Tokens);

            parsed = prod.TryRecognize(
                "null.blob", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.blob", node.Tokens);
        }
        #endregion

        #region Sequence
        [TestMethod]
        public void Sequence_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-sequence"];

            var parsed = prod.TryRecognize(
                "null.sequence", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("null.sequence", node.Tokens);

            parsed = prod.TryRecognize(
                "[]", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("[]", node.Tokens);

            parsed = prod.TryRecognize(
                "[ 23.54, true, @abc; 's bleh' ]", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("[ 23.54, true, @abc; 's bleh' ]", node.Tokens); ;

            parsed = prod.TryRecognize(
                "[ 23.54, @me:you;[ 4, 3, 1, null.duration] ]", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("[ 23.54, @me:you;[ 4, 3, 1, null.duration] ]", node.Tokens);
        }
        #endregion

        #region Record
        [TestMethod]
        public void Record_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-record"];

            var parsed = prod.TryRecognize(
                "null.record", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("null.record", node.Tokens);

            parsed = prod.TryRecognize(
                "{}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{}", node.Tokens);

            parsed = prod.TryRecognize(
                "{prop.name: [ 23.54, true, @abc; 's bleh' ]}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{prop.name: [ 23.54, true, @abc; 's bleh' ]}", node.Tokens);

            parsed = prod.TryRecognize(
                "{prop.name: [ 23.54 ], @att; prop-name-with.att: null.bool}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{prop.name: [ 23.54 ], @att; prop-name-with.att: null.bool}", node.Tokens);

            parsed = prod.TryRecognize(
                "{prop.1: @bleh;[ 23.54 ], \"prop.2\": {}, prop.3: 'R:Record 0x00f'}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{prop.1: @bleh;[ 23.54 ], \"prop.2\": {}, prop.3: 'R:Record 0x00f'}", node.Tokens);
        }
        #endregion

        #region Ref/Hash
        [TestMethod]
        public void Hash_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-hash"];

            var parsed = prod.TryRecognize(
                "#456c;", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("#456c;", node.Tokens);
        }
        [TestMethod]
        public void Ref_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-ref"];

            var parsed = prod.TryRecognize(
                "'Ref:Record 0x456c'", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("'Ref:Record 0x456c'", node.Tokens);
        }
        #endregion

        #region Values
        [TestMethod]
        public void Values_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia-value"];

            var parsed = prod.TryRecognize(
                "null.bool", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("null.bool", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-bool").ToArray().Length);

            parsed = prod.TryRecognize(
                "true", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("true", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-bool").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.decimal", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.decimal", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-decimal").ToArray().Length);

            parsed = prod.TryRecognize(
                "0E0", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("0E0", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-decimal").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.int", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.int", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-int").ToArray().Length);

            parsed = prod.TryRecognize(
                "1_000_234", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("1_000_234", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-int").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.duration", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.duration", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-duration").ToArray().Length);

            parsed = prod.TryRecognize(
                "'D 2.00:00:00'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'D 2.00:00:00'", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-duration").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.timestamp", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.timestamp", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-timestamp").ToArray().Length);

            parsed = prod.TryRecognize(
                "'T 2024'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'T 2024'", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-timestamp").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.string", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.string", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-string").ToArray().Length);

            parsed = prod.TryRecognize(
                "\"\"", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("\"\"", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-string").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.symbol", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.symbol", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-symbol").ToArray().Length);

            parsed = prod.TryRecognize(
                "'s bl'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'s bl'", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-symbol").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.blob", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.blob", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-blob").ToArray().Length);

            parsed = prod.TryRecognize(
                "'b AQID'", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("'b AQID'", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-blob").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.sequence", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.sequence", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-sequence").ToArray().Length);

            parsed = prod.TryRecognize(
                "@tueh;[]", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@tueh;[]", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-sequence").ToArray().Length);


            parsed = prod.TryRecognize(
                "null.record", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.record", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-record").ToArray().Length);

            parsed = prod.TryRecognize(
                "@tueh;{}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@tueh;{}", node.Tokens);
            Assert.AreEqual(1, node.FindNodes("dia-record").ToArray().Length);
        }
        #endregion

        #region Root
        [TestMethod]
        public void Root_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["dia"];

            var parsed = prod.TryRecognize(
                "null.record", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("null.record", node.Tokens);

            parsed = prod.TryRecognize(
                "{}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{}", node.Tokens);

            parsed = prod.TryRecognize(
                "null.sequence", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("null.sequence", node.Tokens);

            parsed = prod.TryRecognize(
                "[]", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("[]", node.Tokens);
        }
        #endregion
    }
}
