using Axis.Dia.Contracts;
using Axis.Dia.Types;
using System.Numerics;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class IntValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = new IntValue();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new IntValue(Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new IntValue(null as BigInteger?, "first", "second");

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region non-null value
            value = new IntValue(2345);

            Assert.AreEqual(2345, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new IntValue(98765, Annotation.Of("first", "second"));

            Assert.AreEqual(98765, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new IntValue(-45678900000, "first", "second");

            Assert.AreEqual(-45678900000, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = (BigInteger)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (ulong)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (long)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (uint)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (int)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (ushort)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (short)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (byte)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = (sbyte)7;
            Assert.AreEqual(7, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = default(IntValue);
            Assert.AreEqual(DiaType.Int, value.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = IntValue.Of(null);
            Assert.IsNull(value.Value);

            value = 8765;
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = IntValue.Of(null);
            Assert.IsTrue(value.IsNull);

            value = IntValue.Of(null, Annotation.Of("bleh"));
            Assert.IsTrue(value.IsNull);

            value = 12345432345432;
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new IntValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = default;
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new IntValue(null, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new IntValue(null as int?, "stuff", null));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            IntValue value = default;
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            value = IntValue.Of(1234567890, Annotation.Of("bleh"));
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = IntValue.Null();
            Assert.AreEqual(default, value);

            value = IntValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(default(IntValue).Equals(value));
            Assert.IsTrue(default(IntValue).ValueEquals(value));
            Assert.AreEqual(default(IntValue).Value, value.Value);
            Assert.AreNotEqual(default(IntValue).Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            IntValue first = 123456789;
            IntValue second = 987654321;
            IntValue third = IntValue.Of(123456789, Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            IntValue first = 123456789;
            IntValue second = 987654321;
            IntValue third = IntValue.Of(123456789, Annotation.Of("annotated"));

            Assert.IsTrue(first.Equals(first));
            Assert.IsFalse(first.Equals(third));
            Assert.IsFalse(first.Equals(second));
        }
    }
}
