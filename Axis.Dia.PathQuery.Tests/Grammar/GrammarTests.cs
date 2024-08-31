using Axis.Dia.PathQuery.Grammar;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;
using Axis.Pulsar.Core.Utils;
using static Axis.Pulsar.Core.CST.ISymbolNode;

namespace Axis.Dia.PathQuery.Tests.Grammar
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void A_ImportGrammar_Tests()
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
                "## stuff for the comment", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("## stuff for the comment", node.Tokens);

            parsed = prod.TryRecognize(
                "## stuff for the comment\r", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("## stuff for the comment", node.Tokens);

            parsed = prod.TryRecognize(
                "## stuff for the comment\n", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("## stuff for the comment", node.Tokens);
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
## some line comment
## other line comment after line-break
/* block
ish
 -ish comment
        with multiple
lines
*/


            ##another line comment
";
            parsed = prod.TryRecognize(
                str, "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>(str, node.Tokens);
        }
        #endregion

        #region Type Qualifier Filter
        [TestMethod]
        public void TypeQualifierFilter_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["type-matcher"];

            var parsed = prod.TryRecognize(
                "$boolean", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("$boolean", node.Tokens);

            parsed = prod.TryRecognize(
                "$boolean $Int", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("$boolean $Int", node.Tokens);

            parsed = prod.TryRecognize(
                "$boolean $Int $integer", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual(2, node.As<INodeContainer>().Nodes.Length);
            Assert.AreEqual<Tokens>("$boolean $Int", node.Tokens);

            parsed = prod.TryRecognize(
                "$boolean $int $rec $blob $sym", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual(5, node.As<INodeContainer>().Nodes.Length);
        }
        #endregion

        #region Index Range Filter
        [TestMethod]
        public void IndexRangeFilter_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["index-range-matcher"];

            var parsed = prod.TryRecognize(
                "#3", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("#3", node.Tokens);

            parsed = prod.TryRecognize(
                "#..", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("#..", node.Tokens);

            parsed = prod.TryRecognize(
                "#1..", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("#1..", node.Tokens);

            parsed = prod.TryRecognize(
                "#^1..", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("#^1..", node.Tokens);

            parsed = prod.TryRecognize(
                "#..1", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("#..1", node.Tokens);

            parsed = prod.TryRecognize(
                "#..^1", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("#..^1", node.Tokens);

            parsed = prod.TryRecognize(
                "#0..^1", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("#0..^1", node.Tokens);
        }
        #endregion

        #region Regular Expression
        [TestMethod]
        public void RegularExpression_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["regular-expression"];

            var parsed = prod.TryRecognize(
                "`abcd[0-9a-z]+`", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("`abcd[0-9a-z]+`", node.Tokens);
        }
        #endregion

        #region Wildcard Expression
        [TestMethod]
        public void WildcardExpression_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["wildcard-expression"];

            var parsed = prod.TryRecognize(
                "abcd", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("abcd", node.Tokens);

            parsed = prod.TryRecognize(
                "abcd{*}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("abcd{*}", node.Tokens);

            parsed = prod.TryRecognize(
                "a{1,+}bcd(a_bc.d(12)3){*}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("a{1,+}bcd(a_bc.d(12)3){*}", node.Tokens);

            parsed = prod.TryRecognize(
                "abc\\_", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("abc\\_", node.Tokens);
        }

        [TestMethod]
        public void Cardinality_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["cardinality"];

            var parsed = prod.TryRecognize(
                "{*}", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("{*}", node.Tokens);

            parsed = prod.TryRecognize(
                "{/* */\n\n+}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{/* */\n\n+}", node.Tokens);

            parsed = prod.TryRecognize(
                "{  ?\t}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{  ?\t}", node.Tokens);

            parsed = prod.TryRecognize(
                "{1}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{1}", node.Tokens);

            parsed = prod.TryRecognize(
                "{1, 4}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{1, 4}", node.Tokens);

            parsed = prod.TryRecognize(
                "{1,+}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{1,+}", node.Tokens);

            parsed = prod.TryRecognize(
                "{21,+}", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("{21,+}", node.Tokens);
        }
        #endregion

        #region Attribute Filter
        [TestMethod]
        public void AttributeFilter_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["attribute-matcher"];

            var parsed = prod.TryRecognize(
                "@abcd;", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("@abcd;", node.Tokens);

            parsed = prod.TryRecognize(
                "@abcd:`efg`; @flag;", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("@abcd:`efg`; @flag;", node.Tokens);
        }
        #endregion

        #region Property Filter
        [TestMethod]
        public void PropertyFilter_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["property-name-matcher"];

            var parsed = prod.TryRecognize(
                ":abcd", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>(":abcd", node.Tokens);

            parsed = prod.TryRecognize(
                ":`abc`", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>(":`abc`", node.Tokens);

            parsed = prod.TryRecognize(
                ":`abc` @flag;", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>(":`abc` @flag;", node.Tokens);
        }
        #endregion

        #region Segment
        [TestMethod]
        public void Segment_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["segment"];

            var parsed = prod.TryRecognize(
                "/:abcd", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("/:abcd", node.Tokens);

            parsed = prod.TryRecognize(
                "/! @flag;", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("/! @flag;", node.Tokens);
        }
        #endregion

        #region Path
        [TestMethod]
        public void Path_Tests()
        {
            var prod = GrammarUtil.LanguageContext.Grammar["path"];

            var parsed = prod.TryRecognize(
                "/:abcd /@flag;", "path", GrammarUtil.LanguageContext, out var result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out ISymbolNode node));
            Assert.AreEqual<Tokens>("/:abcd /@flag;", node.Tokens);

            parsed = prod.TryRecognize(
                "/!:bleh\n\n/#..", "path", GrammarUtil.LanguageContext, out result);
            Assert.IsTrue(parsed);
            Assert.IsTrue(result.Is(out node));
            Assert.AreEqual<Tokens>("/!:bleh\n\n/#..", node.Tokens);
        }
        #endregion
    }
}
