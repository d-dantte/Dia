using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Numerics;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class DecimalValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = new DecimalValue();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new DecimalValue(Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new DecimalValue(null as BigDecimal?, "first", "second");

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region non-null value
            value = new DecimalValue(0.5m);

            Assert.AreEqual(0.5m, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new DecimalValue(123.000000012345m, Annotation.Of("first", "second"));

            Assert.AreEqual(123.000000012345m, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new DecimalValue(-0.00000001m, "first", "second");

            Assert.AreEqual(-0.00000001m, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = 12345m;

            Assert.AreEqual(12345m, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = default(DecimalValue);
            Assert.AreEqual(DiaType.Decimal, value.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = DecimalValue.Of(null);
            Assert.IsNull(value.Value);

            value = 6.54;
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = DecimalValue.Of(null);
            Assert.IsTrue(value.IsNull);

            value = DecimalValue.Of(null, Annotation.Of("bleh"));
            Assert.IsTrue(value.IsNull);

            value = 445;
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new DecimalValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = default;
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new DecimalValue(null, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new DecimalValue(null as BigDecimal?, "stuff", default));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            DecimalValue value = default;
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            value = DecimalValue.Of(7654.456m, Annotation.Of("bleh"));
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = DecimalValue.Null();
            Assert.AreEqual(default, value);

            value = DecimalValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(default(DecimalValue).Equals(value));
            Assert.IsTrue(default(DecimalValue).ValueEquals(value));
            Assert.AreEqual(default(DecimalValue).Value, value.Value);
            Assert.AreNotEqual(default(DecimalValue).Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            DecimalValue first = 0.000000000000000000065434m;
            DecimalValue second = 1.234m;
            DecimalValue third = DecimalValue.Of(0.000000000000000000065434m, Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            DecimalValue first = 0.000000000000000000065434m;
            DecimalValue second = 1.234m;
            DecimalValue third = DecimalValue.Of(0.000000000000000000065434m, Annotation.Of("annotated"));

            Assert.IsTrue(first.Equals(first));
            Assert.IsFalse(first.Equals(third));
            Assert.IsFalse(first.Equals(second));
        }
    }
}
