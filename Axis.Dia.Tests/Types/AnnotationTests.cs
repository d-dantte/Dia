using Axis.Dia.Types;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class AnnotationTests
    {
        [TestMethod]
        public void TryGetAttributeTest()
        {
            var value = "key:value";
            var annotation = Annotation.Of(value);
            Assert.AreEqual(value, annotation.Symbol.Value);
            Assert.IsTrue(annotation.IsAttribute);
            Assert.IsTrue(annotation.TryGetAttribute(out var attribute));
            Assert.AreEqual("key", attribute.Key);
            Assert.AreEqual("value", attribute.Value);
        }
    }
}
