using Axis.Dia.Contracts;
using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Common.Utils;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class ValueMetadataParserTests
    {
        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ValueMetadataParser
                .Parse(null, new Dia.Convert.Json.ParserContext()));

            Assert.ThrowsException<ArgumentException>(() => ValueMetadataParser
                .Parse(CSTNode.Of("a", "b"), default));

            Assert.ThrowsException<ArgumentException>(() => ValueMetadataParser.Parse(
                CSTNode.Of("a", "b"), new Dia.Convert.Json.ParserContext()));


            var cst = ParserUtil.ParseTokens(
                "value-metadata",
                "[#0x4f;$Int; ann1;ann2;'abc:xyz';]");

            var result = ValueMetadataParser.Parse(cst, new Dia.Convert.Json.ParserContext());
            Assert.IsTrue(result.IsDataResult());
            var info = result.Resolve();
            Assert.AreEqual(DiaType.Int, info.Type);
            Assert.AreEqual(0x4f, info.AddressIndex);
            Assert.AreEqual(3, info.Annotations.Length);
            Assert.AreEqual("ann1", info.Annotations[0]);
            Assert.AreEqual("ann2", info.Annotations[1]);
            Assert.AreEqual("abc:xyz", info.Annotations[2]);


            cst = ParserUtil.ParseTokens(
                "value-metadata",
                "[$Symbol;]");

            result = ValueMetadataParser.Parse(cst, new Dia.Convert.Json.ParserContext());
            Assert.IsTrue(result.IsDataResult());
            info = result.Resolve();
            Assert.AreEqual(DiaType.Symbol, info.Type);
            Assert.AreEqual(null, info.AddressIndex);
            Assert.AreEqual(0, info.Annotations.Length);

        }

        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentException>(() => ValueMetadataParser
                .Serialize(default, new()));

            Assert.ThrowsException<ArgumentException>(() => ValueMetadataParser
                .Serialize(new(), default));

            var metadata = new ValueMetadataParser.ValueMetadata(12, DiaType.Blob, Array.Empty<Annotation>());
            var result = ValueMetadataParser.Serialize(metadata, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("[#0xc;$Blob;]", result.Resolve());

            metadata = new ValueMetadataParser.ValueMetadata(null, DiaType.Clob, ArrayUtil.Of<Annotation>("abc", "def"));
            result = ValueMetadataParser.Serialize(metadata, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("[$Clob;abc;def;]", result.Resolve());
        }

        [TestMethod]
        public void Grammar_Tests()
        {
            var result = GrammarUtil.Grammar.GetRecognizer("value-metadata").Recognize("[$Int;]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("value-metadata").Recognize("[#0xabc;$Decimal;]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("value-metadata").Recognize("[$Decimal; stuff; 'other stuff';myStuff;]");
            Assert.IsTrue(result is SuccessResult);

        }
    }
}
