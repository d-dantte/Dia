using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Binary.Serializers
{
    [TestClass]
    public class AnnotationSerializerTests
    {

        [TestMethod]
        public void Serialize_Tests()
        {
            var defaultValue = Annotation.Default;
            var result = AnnotationSerializer.Serialize(defaultValue);
            Assert.IsTrue(result.IsErrorResult());

            Annotation someValue = Annotation.Of("the-value");
            result = AnnotationSerializer.Serialize(someValue);
            Assert.IsTrue(result.IsDataResult());
            var data = result.Resolve();
            Assert.AreEqual(20, data.Length);
            Assert.AreEqual(1, data[0]); // the annotation count
            Assert.AreEqual(9, data[1]);   // symbol char count (varbyte)

            var someValue2 = Annotation.Of("the-other-value");
            result = AnnotationSerializer.Serialize(someValue, someValue2);
            Assert.IsTrue(result.IsDataResult());
            data = result.Resolve();
            Assert.AreEqual(51, data.Length);
            Assert.AreEqual(2, data[0]); // the annotation count
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var someValues = Annotation.Of("the-value", "the-annotation", "the-other-annotation");
            var bytes = AnnotationSerializer
                .Serialize(someValues)
                .Resolve();
            var result = AnnotationSerializer.Deserialize(new MemoryStream(bytes));
            Assert.IsTrue(result.IsDataResult());
            var resultValues = result.Resolve();
            Assert.IsTrue(Enumerable.SequenceEqual(someValues, resultValues));
        }
    }
}
