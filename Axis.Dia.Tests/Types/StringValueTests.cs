using Axis.Dia.Contracts;
using Axis.Dia.Types;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class StringValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = new StringValue();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new StringValue(Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new StringValue(null, "first", "second");

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region non-null value
            value = new StringValue("bleh");

            Assert.AreEqual("bleh", value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new StringValue("bleh bleh", Annotation.Of("first", "second"));

            Assert.AreEqual("bleh bleh", value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new StringValue("things", "first", "second");

            Assert.AreEqual("things", value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = "stuff";
            Assert.AreEqual("stuff", value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = "stuff".ToCharArray();
            Assert.AreEqual("stuff", value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = new Span<char>("stuff".ToCharArray());
            Assert.AreEqual("stuff", value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = default(StringValue);
            Assert.AreEqual(DiaType.String, value.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = StringValue.Of(null);
            Assert.IsNull(value.Value);

            value = "the quick fox";
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = StringValue.Of(null);
            Assert.IsTrue(value.IsNull);

            value = StringValue.Of(null, Annotation.Of("bleh"));
            Assert.IsTrue(value.IsNull);

            value = "didn't jump";
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new StringValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = default;
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new StringValue(null, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new StringValue(null, "stuff", default));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            StringValue value = default;
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            value = StringValue.Of("nore was it brown", Annotation.Of("bleh"));
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = StringValue.Null();
            Assert.AreEqual(default, value);

            value = StringValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(default(StringValue).ValueEquals(value));
            Assert.IsTrue(default(StringValue).ValueEquals(value));
            Assert.AreEqual(default(StringValue).Value, value.Value);
            Assert.AreNotEqual(default(StringValue).Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            StringValue first = "worry";
            StringValue second = "fascination";
            StringValue third = StringValue.Of("worry", Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            StringValue first = "worry";
            StringValue second = "fascination";
            StringValue third = StringValue.Of("worry", Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsFalse(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }
    }
}
