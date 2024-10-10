using Axis.Dia.Contracts;
using Axis.Dia.Types;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class BoolValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = new BoolValue();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new BoolValue(Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new BoolValue(null as bool?, "first", "second");

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region non-null value
            value = new BoolValue(true);

            Assert.AreEqual(true, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new BoolValue(false, Annotation.Of("first", "second"));

            Assert.AreEqual(false, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new BoolValue(false, "first", "second");

            Assert.AreEqual(false, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = true;

            Assert.AreEqual(true, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = default(BoolValue);
            Assert.AreEqual(DiaType.Bool, value.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = BoolValue.Of(null);
            Assert.IsNull(value.Value);

            value = true;
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = BoolValue.Of(null);
            Assert.IsTrue(value.IsNull);

            value = BoolValue.Of(null, Annotation.Of("bleh"));
            Assert.IsTrue(value.IsNull);

            value = true;
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new BoolValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = default;
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new BoolValue(null, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new BoolValue(null as bool?, "stuff", default));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            BoolValue value = default;
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            value = BoolValue.Of(false, Annotation.Of("bleh"));
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = BoolValue.Null();
            Assert.AreEqual(default, value);

            value = BoolValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(default(BoolValue).ValueEquals(value));
            Assert.IsTrue(default(BoolValue).ValueEquals(value));
            Assert.AreEqual(default(BoolValue).Value, value.Value);
            Assert.AreNotEqual(default(BoolValue).Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            BoolValue first = true;
            BoolValue second = false;
            BoolValue third = BoolValue.Of(true, Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            BoolValue first = true;
            BoolValue second = false;
            BoolValue third = BoolValue.Of(true, Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsFalse(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }
    }
}
