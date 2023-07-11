using Axis.Dia.IO.Text.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Tests.IO.Text.Parsers
{
    [TestClass]
    public class AnnotationParserTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            var annotations = Annotation.Of(
                "first",
                "second:bleh bleh",
                "abra\u20eadevo");
            var text = AnnotationParser.Serialize(annotations[0]);
            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("first::", text.Resolve());

            text = AnnotationParser.Serialize(annotations[1]);
            Assert.IsTrue(text.IsDataResult());

            text = AnnotationParser.Serialize(annotations[2]);
            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("'abra\\u20eadevo'::", text.Resolve());

            text = AnnotationParser.Serialize(annotations);
            Assert.IsTrue(text.IsDataResult());
            Assert.AreEqual("first::'second:bleh bleh'::'abra\\u20eadevo'::", text.Resolve());
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var annotations = Annotation.Of(
                "first",
                "second:bleh bleh",
                "abra\u20eadevo");
            var text = AnnotationParser.Serialize(annotations);
            var result = text.Bind(t => AnnotationParser.Parse(t, null));
            Assert.IsTrue(result.IsDataResult());
            var annotationsResult = result.Resolve();
            Assert.AreEqual(3, annotationsResult.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(annotations, annotationsResult));
        }
    }
}
