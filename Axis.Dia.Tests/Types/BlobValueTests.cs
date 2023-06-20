using Axis.Dia.Contracts;
using Axis.Dia.Types;
using System.Text;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class BlobValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = new BlobValue();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new BlobValue(Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new BlobValue(null as byte[], "first", "second");

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            var bytes = Encoding.ASCII.GetBytes("the value");
            #region non-null value
            value = new BlobValue(bytes);

            Assert.IsTrue(Enumerable.SequenceEqual(bytes, value.Value!));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new BlobValue(bytes, Annotation.Of("first", "second"));

            Assert.IsTrue(Enumerable.SequenceEqual(bytes, value.Value!));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new BlobValue(bytes, "first", "second");

            Assert.IsTrue(Enumerable.SequenceEqual(bytes, value.Value!));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = bytes;

            Assert.IsTrue(Enumerable.SequenceEqual(bytes, value.Value!));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new Span<byte>(bytes);

            Assert.IsTrue(Enumerable.SequenceEqual(bytes, value.Value!));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = default(BlobValue);
            Assert.AreEqual(DiaType.Blob, value.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = BlobValue.Of(null);
            Assert.IsNull(value.Value);

            value = Encoding.ASCII.GetBytes("something");
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = BlobValue.Of(null);
            Assert.IsTrue(value.IsNull);

            value = BlobValue.Of(null, Annotation.Of("bleh"));
            Assert.IsTrue(value.IsNull);

            value = new byte[0];
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new BlobValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = default;
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new BlobValue(null as byte[], null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new BlobValue(null as byte[], "stuff", null));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            BlobValue value = default;
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            var bytes = Encoding.UTF8.GetBytes("stuff");
            value = BlobValue.Of(bytes, Annotation.Of("bleh"));
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(value.Value!, copied.Value!));
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = BlobValue.Null();
            Assert.AreEqual(default, value);

            value = BlobValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(default(BlobValue).Equals(value));
            Assert.IsTrue(default(BlobValue).ValueEquals(value));
            Assert.AreEqual(default(BlobValue).Value, value.Value);
            Assert.AreNotEqual(default(BlobValue).Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            var something = Encoding.UTF8.GetBytes("something");
            var otherThing = Encoding.UTF8.GetBytes("other thing");
            BlobValue first = something;
            BlobValue second = otherThing;
            BlobValue third = BlobValue.Of(something, Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var something = Encoding.UTF8.GetBytes("something");
            var otherThing = Encoding.UTF8.GetBytes("other thing");
            BlobValue first = something;
            BlobValue second = otherThing;
            BlobValue third = BlobValue.Of(something, Annotation.Of("annotated"));

            Assert.IsTrue(first.Equals(first));
            Assert.IsFalse(first.Equals(third));
            Assert.IsFalse(first.Equals(second));
        }
    }
}
