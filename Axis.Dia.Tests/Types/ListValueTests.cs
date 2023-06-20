using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Utils;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class ListValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = ListValue.Null();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new ListValue(null, Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region non-null value
            value = new ListValue { "value" };
            Assert.IsNotNull(value.Value);
            Assert.AreEqual(1, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new ListValue(Annotation.Of("first", "second")) { "something", "else" };

            Assert.IsNotNull(value.Value);
            Assert.AreEqual(2, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new ListValue("first", "second");

            Assert.IsNotNull(value.Value);
            Assert.AreEqual(0, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = ArrayUtil.Of<IDiaValue>(DecimalValue.Of(654));

            Assert.IsNotNull(value.Value);
            Assert.AreEqual(1, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = ListValue.Null();
            Assert.AreEqual(DiaType.List, value!.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = ListValue.Of(ArrayUtil.Of<Annotation>(), null);
            Assert.IsNull(value.Value);

            value = ArrayUtil.Of<IDiaValue>(IntValue.Of(54));
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = ListValue.Null();
            Assert.IsTrue(value.IsNull);

            value = ListValue.Of(ArrayUtil.Of<Annotation>("bleh"), null);
            Assert.IsTrue(value.IsNull);

            value = ArrayUtil.Of<IDiaValue>(DecimalValue.Of(654));
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new ListValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = ListValue.Null();
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new ListValue(null, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new ListValue(null as IEnumerable<IDiaValue>, "stuff", null));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            ListValue value = ListValue.Null();
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            value = ListValue.Of(ArrayUtil.Of<Annotation>("bleh"), StringValue.Of("meta"), ClobValue.Of("critics"));
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.IsTrue(Extensions.NullOrTrue(value.Value, copied.Value, Enumerable.SequenceEqual));
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = ListValue.Null();
            Assert.IsNull(value.Value);

            value = ListValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(ListValue.Null().Equals(value));
            Assert.IsTrue(ListValue.Null().ValueEquals(value));
            Assert.AreEqual(ListValue.Null().Value, value.Value);
            Assert.AreNotEqual(ListValue.Null().Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            ListValue first = ArrayUtil.Of<IDiaValue>(IntValue.Of(345));
            ListValue second = ArrayUtil.Of<IDiaValue>(SymbolValue.Of("things"));
            ListValue third = ListValue.Of(ArrayUtil.Of<Annotation>("annotated"), IntValue.Of(345));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            ListValue first = ArrayUtil.Of<IDiaValue>(IntValue.Of(345));
            ListValue second = ArrayUtil.Of<IDiaValue>(SymbolValue.Of("things"));
            ListValue third = ListValue.Of(ArrayUtil.Of<Annotation>("annotated"), IntValue.Of(345));

            Assert.IsTrue(first.Equals(first));
            Assert.IsFalse(first.Equals(third));
            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void EmptyTests()
        {
            var value = ListValue.Empty();
            Assert.AreEqual(0, value.Value!.Length);
        }

        [TestMethod]
        public void AddValueWrapper_Tests()
        {
            var value = ListValue.Empty();
            Assert.AreEqual(0, value.Value!.Length);
            value.Add(new ValueWrapper(StringValue.Of("me")));
            Assert.AreEqual(1, value.Value!.Length);
            value = ListValue.Null();
            Assert.ThrowsException<InvalidOperationException>(() => value.Add(StringValue.Of("you")));
        }

        [TestMethod]
        public void GetValueWrapperEnumerator_Tests()
        {
            var value = new ListValue { "a", "b", 34.54m };
            var enumerator = value.GetEnumerator();
            Assert.IsNotNull(enumerator);
            int cnt = 0;
            for(; enumerator.MoveNext(); cnt++)
            {
                if (cnt == 0)
                {
                    Assert.AreEqual(DiaType.String, enumerator.Current.Value.Type);
                    Assert.AreEqual("a", ((StringValue)enumerator.Current.Value).Value);
                }
                if (cnt == 1)
                {
                    Assert.AreEqual(DiaType.String, enumerator.Current.Value.Type);
                    Assert.AreEqual("b", ((StringValue)enumerator.Current.Value).Value);
                }
                if (cnt == 2)
                {
                    Assert.AreEqual(DiaType.Decimal, enumerator.Current.Value.Type);
                    Assert.AreEqual(new BigDecimal(34.54m), ((DecimalValue)enumerator.Current.Value).Value);
                }
            }

            Assert.AreEqual(3, cnt);

            value = ListValue.Null();
            Assert.ThrowsException<InvalidOperationException>(value.GetEnumerator);
        }

        [TestMethod]
        public void Count_Tests()
        {
            var value = new ListValue { "a", "b", 34.54m, IntValue.Of(566) };
            Assert.AreEqual(4, value.Count());

            _ = value.AddValue(IntValue.Of(454));
            Assert.AreEqual(5, value.Count());
        }

        [TestMethod]
        public void Contains_Test()
        {
            var value = StringValue.Of("random");
            var value2 = StringValue.Of("random");
            var list = new ListValue { value };
            Assert.IsTrue(list.Contains(value));
            Assert.IsTrue(list.Contains(value2));
            list = ListValue.Null();
            Assert.ThrowsException<InvalidOperationException>(() => list.Contains(StringValue.Of("you")));
        }

        [TestMethod]
        public void Remove_Test()
        {
            var value = StringValue.Of("random");
            var value2 = StringValue.Of("random");
            var list = new ListValue { value };
            Assert.IsTrue(list.Remove(value2));
            Assert.AreEqual(0, list.Count());
            Assert.IsFalse(list.Remove(value));
            Assert.AreEqual(0, list.Count());
            list = ListValue.Null();
            Assert.ThrowsException<InvalidOperationException>(() => list.Remove(StringValue.Of("you")));
        }

        [TestMethod]
        public void RemoveAt_Test()
        {
            var value = StringValue.Of("random");
            var value2 = StringValue.Of("random 2");
            var list = new ListValue { value, value2 };
            list.RemoveAt(1);
            Assert.AreEqual(1, list.Count());
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(-12));
            list.RemoveAt(0);
            Assert.AreEqual(0, list.Count());

            list = ListValue.Null();
            Assert.ThrowsException<InvalidOperationException>(() => list.RemoveAt(0));
        }

        [TestMethod]
        public void Indexer_Test()
        {
            var value = StringValue.Of("random");
            var value2 = StringValue.Of("random 2");
            var list = new ListValue();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[0] = value);

            list.AddValue(value);
            Assert.AreEqual(1, list.Count());
            Assert.IsTrue(list.Contains(value));

            list[0] = value2;
            Assert.IsFalse(list.Contains(value));
            Assert.IsTrue(list.Contains(value2));
            Assert.AreEqual(value2, list[0]);
        }

    }
}
