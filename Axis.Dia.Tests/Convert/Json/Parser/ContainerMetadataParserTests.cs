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
    public class ContainerMetadataParserTests
    {
        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ContainerMetadataParser
                .Parse(null, new ParserContext()));

            Assert.ThrowsException<ArgumentException>(() => ContainerMetadataParser
                .Parse(CSTNode.Of("a", "b"), default));

            Assert.ThrowsException<ArgumentException>(() => ContainerMetadataParser.Parse(
                CSTNode.Of("a", "b"), new ParserContext()));


            var cst = ParserUtil.ParseTokens(
                "container-metadata",
                "[#0x4f; ann1;ann2;'abc:xyz';]");

            var result = ContainerMetadataParser.Parse(cst, new Dia.Convert.Json.ParserContext());
            Assert.IsTrue(result.IsDataResult());
            var info = result.Resolve();
            Assert.AreEqual(0x4f, info.AddressIndex);
            Assert.AreEqual(3, info.Annotations.Length);
            Assert.AreEqual("ann1", info.Annotations[0]);
            Assert.AreEqual("ann2", info.Annotations[1]);
            Assert.AreEqual("abc:xyz", info.Annotations[2]);


            cst = ParserUtil.ParseTokens(
                "container-metadata",
                "[#0xabc;]");

            result = ContainerMetadataParser.Parse(cst, new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            info = result.Resolve();
            Assert.AreEqual(0xabc, info.AddressIndex);
            Assert.AreEqual(0, info.Annotations.Length);

        }

        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentException>(() => ContainerMetadataParser
                .Serialize(default, new()));

            Assert.ThrowsException<ArgumentException>(() => ContainerMetadataParser
                .Serialize(new(), default));

            var metadata = new ContainerMetadataParser.ContainerMetadata(12, Array.Empty<Annotation>());
            var result = ContainerMetadataParser.Serialize(metadata, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("[#0xc;]", result.Resolve());

            metadata = new ContainerMetadataParser.ContainerMetadata(null, ArrayUtil.Of<Annotation>("abc", "def"));
            result = ContainerMetadataParser.Serialize(metadata, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("[abc;def;]", result.Resolve());
        }

        [TestMethod]
        public void Grammar_Tests()
        {
            var result = GrammarUtil.Grammar.GetRecognizer("container-metadata").Recognize("[#0x123;]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("container-metadata").Recognize("[stuff;]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("container-metadata").Recognize("[#0x111; stuff; 'other stuff';myStuff;]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("container-metadata").Recognize("[stuff; #0x111; stuff; 'other stuff';myStuff;]");
            Assert.IsTrue(result is not SuccessResult);

        }
    }
}
