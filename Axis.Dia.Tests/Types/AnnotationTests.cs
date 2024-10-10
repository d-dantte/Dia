using Axis.Dia.Contracts;
using Axis.Dia.Types;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class AnnotationTests
    {

        [TestMethod]
        public void Creation_Tests()
        {
            #region default annotation
            var annotation = new Annotation();

            Assert.AreEqual(null, annotation.Text);
            Assert.AreEqual(default, annotation);
            Assert.IsTrue(annotation.IsDefault);


            annotation = default;

            Assert.AreEqual(null, annotation.Text);
            Assert.AreEqual(default, annotation);
            Assert.IsTrue(annotation.IsDefault);
            #endregion

            #region non-null annotation
            annotation = new Annotation("the annotation");

            Assert.AreEqual("the annotation", annotation.Text);
            Assert.IsFalse(annotation.IsDefault);
            Assert.AreNotEqual(default, annotation);
            #endregion

            #region implicits
            annotation = "the annotation";

            Assert.AreEqual("the annotation", annotation.Text);


            annotation = "the.annotation".ToArray();

            Assert.AreEqual("the.annotation", annotation.Text);


            annotation = new Span<char>("the-annotation".ToArray());

            Assert.AreEqual("the-annotation", annotation.Text);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var annotation = default(Annotation);
            Assert.AreEqual(DiaType.Annotation, annotation.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            Assert.ThrowsException<ArgumentException>(() => Annotation.Of(null as string));

            Annotation annotation = "something";
            Assert.IsNotNull(annotation.Text);
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            Annotation annotation = default;
            var copied = annotation.DeepCopy();

            Assert.AreEqual(annotation, copied);
            Assert.AreEqual(annotation.Text, copied.Text);

            annotation = Annotation.Of("stuff");
            copied = annotation.DeepCopy();

            Assert.AreEqual(annotation, copied);
            Assert.AreEqual(annotation.Text, copied.Text);
        }

        [TestMethod]
        public void EqualsTest()
        {
            Annotation first = "something";
            Annotation second = "other something";
            Annotation third = Annotation.Of("something");

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void IsIdentifier_Tests()
        {
            var annotation = Annotation.Of("abcdef");
            Assert.IsTrue(annotation.IsIdentifier);
            Assert.IsTrue(annotation.TryGetIdentifier(out var identifier));
            Assert.AreEqual("abcdef", identifier);

            annotation = "abcd.efgh";
            Assert.IsTrue(annotation.IsIdentifier);
            Assert.IsTrue(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual("abcd.efgh", identifier);

            annotation = "_abc_d.efg_h_";
            Assert.IsTrue(annotation.IsIdentifier);
            Assert.IsTrue(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual("_abc_d.efg_h_", identifier);

            annotation = "ab.cd-efg-h";
            Assert.IsTrue(annotation.IsIdentifier);
            Assert.IsTrue(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual("ab.cd-efg-h", identifier);

            annotation = "_";
            Assert.IsTrue(annotation.IsIdentifier);
            Assert.IsTrue(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual("_", identifier);

            annotation = "2abc";
            Assert.IsFalse(annotation.IsIdentifier);
            Assert.IsFalse(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual(null, identifier);

            annotation = "abc xyz";
            Assert.IsFalse(annotation.IsIdentifier);
            Assert.IsFalse(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual(null, identifier);

            annotation = ".ab.cd-efg-h";
            Assert.IsFalse(annotation.IsIdentifier);
            Assert.IsFalse(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual(null, identifier);

            annotation = "-ab.cd-efg-h";
            Assert.IsFalse(annotation.IsIdentifier);
            Assert.IsFalse(annotation.TryGetIdentifier(out identifier));
            Assert.AreEqual(null, identifier);
        }

        [TestMethod]
        public void Attribute_Tests()
        {
            var value = "key:value";
            var annotation = Annotation.Of(value);
            Assert.AreEqual(value, annotation.Text);
            Assert.IsTrue(annotation.IsAttribute);
            Assert.IsTrue(annotation.TryGetAttribute(out var attribute));
            Assert.AreEqual("key", attribute.Key);
            Assert.AreEqual("value", attribute.Value);
        }
    }
}
