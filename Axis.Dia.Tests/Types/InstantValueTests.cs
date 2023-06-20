using Axis.Dia.Contracts;
using Axis.Dia.Types;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class InstantValueTests
    {
        private static readonly DateTimeOffset First = DateTimeOffset.Parse("2020-01-20 18:32:47.1234");
        private static readonly DateTimeOffset Second = DateTimeOffset.Parse("1993-06-30");
        private static readonly DateTimeOffset Third = DateTimeOffset.Parse("2051-09-21 00:00:12.03221");

        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = new InstantValue();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new InstantValue(Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new InstantValue(null as DateTimeOffset?, "first", "second");

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region non-null value
            value = new InstantValue(First);

            Assert.AreEqual(First, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new InstantValue(Second, Annotation.Of("first", "second"));

            Assert.AreEqual(Second, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new InstantValue(Second, "first", "second");

            Assert.AreEqual(Second, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = First;
            Assert.AreEqual(First, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = First;
            Assert.AreEqual(First, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = TimeSpan.FromHours(7.54);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = default(InstantValue);
            Assert.AreEqual(DiaType.Instant, value.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = InstantValue.Of(null);
            Assert.IsNull(value.Value);

            value = First;
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = InstantValue.Of(null);
            Assert.IsTrue(value.IsNull);

            value = InstantValue.Of(null, Annotation.Of("bleh"));
            Assert.IsTrue(value.IsNull);

            value = Third;
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new InstantValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = default;
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new InstantValue(null, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new InstantValue(null as DateTimeOffset?, "stuff", null));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            InstantValue value = default;
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            value = InstantValue.Of(Third, Annotation.Of("bleh"));
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = InstantValue.Null();
            Assert.AreEqual(default, value);

            value = InstantValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(default(InstantValue).Equals(value));
            Assert.IsTrue(default(InstantValue).ValueEquals(value));
            Assert.AreEqual(default(InstantValue).Value, value.Value);
            Assert.AreNotEqual(default(InstantValue).Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            InstantValue first = First;
            InstantValue second = Second;
            InstantValue third = InstantValue.Of(First, Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            InstantValue first = First;
            InstantValue second = Second;
            InstantValue third = InstantValue.Of(First, Annotation.Of("annotated"));

            Assert.IsTrue(first.Equals(first));
            Assert.IsFalse(first.Equals(third));
            Assert.IsFalse(first.Equals(second));
        }
    }
}
