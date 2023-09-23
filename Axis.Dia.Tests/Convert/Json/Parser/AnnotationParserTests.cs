using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class AnnotationParserTests
    {

        [TestMethod]
        public void SerializeTests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => AnnotationParser.Serialize(
                null,
                new SerializerContext()));

            Assert.ThrowsException<ArgumentException>(() => AnnotationParser.Serialize(
                Array.Empty<Annotation>(),
                default));

            var options = SerializerOptionsBuilder.NewBuilder().Build();
            var context = new SerializerContext(options);
            var result = AnnotationParser.Serialize(new[] { default(Annotation) }, new SerializerContext(options));
            Assert.IsTrue(result.IsErrorResult());

            result = AnnotationParser.Serialize(new[] { Annotation.Of("stuff") }, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("stuff;", result.Resolve());

            result = AnnotationParser.Serialize(Annotation.Of("stuff", "quoted stuff", "key:value"), context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("stuff;'quoted stuff';'key:value';", result.Resolve());

            result = AnnotationParser.Serialize(Array.Empty<Annotation>(), context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("", result.Resolve());
        }

        [TestMethod]
        public void ParseTests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => AnnotationParser.Parse(
                null,
                new ParserContext()));

            Assert.ThrowsException<ArgumentException>(() => AnnotationParser.Parse(
                CSTNode.Of("dummy", "stuff"),
                default));

            Assert.ThrowsException<ArgumentException>(() => AnnotationParser.Parse(
                CSTNode.Of("dummy", "stuff"),
                new ParserContext()));

            var cst = ParserUtil.ParseTokens("annotation-list", "stuff;");
            var result = AnnotationParser.Parse(cst, new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            var annotations = result.Resolve();
            Assert.AreEqual(1, annotations.Length);
            Assert.AreEqual("stuff", annotations[0].Text);

            cst = ParserUtil.ParseTokens("annotation-list", "stuff;'quoted stuff'; 'key:value';");
            result = AnnotationParser.Parse(cst, new ParserContext());
            Assert.IsTrue(result.IsDataResult());
            annotations = result.Resolve();
            Assert.AreEqual(3, annotations.Length);
            Assert.AreEqual("stuff", annotations[0].Text);
            Assert.AreEqual("quoted stuff", annotations[1].Text);
            Assert.AreEqual("key:value", annotations[2].Text);
        }

        [TestMethod]
        public void GrammarTests()
        {
            var result = ParserUtil.ParseTokens("annotation-list", "stuff;");
            Assert.IsInstanceOfType<CSTNode>(result);

            result = ParserUtil.ParseTokens("annotation-list", "stuff;'quoted stuff'; 'key:value';ann1;ann2;'abc:xyz';");
            Assert.IsInstanceOfType<CSTNode>(result);
        }
    }
}
