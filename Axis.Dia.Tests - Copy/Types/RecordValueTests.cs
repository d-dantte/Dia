using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Utils;

namespace Axis.Dia.Tests.Types
{
    using Property = KeyValuePair<SymbolValue, IDiaValue>;

    [TestClass]
    public class RecordValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null value
            var value = RecordValue.Null();

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new RecordValue(null as IEnumerable<Property>, Annotation.Of("first", "second"));

            Assert.AreEqual(null, value.Value);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region non-null value
            value = new RecordValue { ["key"] = "value" };
            Assert.IsNotNull(value.Value);
            Assert.AreEqual(1, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);


            value = new RecordValue(Annotation.Of("first", "second"))
            { 
                ["first"] = "something",
                ["second"] = "else"
            };

            Assert.IsNotNull(value.Value);
            Assert.AreEqual(2, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);


            value = new RecordValue("first", "second");

            Assert.IsNotNull(value.Value);
            Assert.AreEqual(0, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);
            #endregion

            #region implicits
            value = ArrayUtil.Of(
                KeyValuePair.Create(SymbolValue.Of("key"), DecimalValue.Of(654) as IDiaValue));

            Assert.IsNotNull(value.Value);
            Assert.AreEqual(1, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            value = ArrayUtil.Of(
                (SymbolValue.Of("key"), DecimalValue.Of(654) as IDiaValue));

            Assert.IsNotNull(value.Value);
            Assert.AreEqual(1, value.Value!.Length);
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var value = RecordValue.Null();
            Assert.AreEqual(DiaType.Record, value!.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var value = RecordValue.Of(ArrayUtil.Of<Annotation>(), null as Property[]);
            Assert.IsNull(value.Value);

            value = ArrayUtil.Of(
                (SymbolValue.Of("key"), DecimalValue.Of(654) as IDiaValue));
            Assert.IsNotNull(value.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var value = RecordValue.Null();
            Assert.IsTrue(value.IsNull);

            value = RecordValue.Of(ArrayUtil.Of<Annotation>("bleh"), null as Property[]);
            Assert.IsTrue(value.IsNull);

            value = ArrayUtil.Of(
                (SymbolValue.Of("key"), DecimalValue.Of(654) as IDiaValue));
            Assert.IsFalse(value.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var value = new RecordValue(null as IEnumerable<Property>, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(2, value.Annotations.Length);

            value = RecordValue.Null();
            Assert.IsNotNull(value.Annotations);
            Assert.AreEqual(0, value.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new RecordValue(null as IEnumerable<Property>, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new RecordValue(null as IEnumerable<Property>, "stuff", default));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            RecordValue value = RecordValue.Null();
            var copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(value.Value, copied.Value);

            value = new RecordValue("bleh")
            {
                ["key1"] = StringValue.Of("meta"),
                ["key2"] = ClobValue.Of("critics")
            };
            copied = value.DeepCopy();

            Assert.AreEqual(value, copied);
            Assert.AreEqual(value.Annotations.Length, copied.Annotations.Length);
            Assert.IsTrue(Extensions.NullOrTrue(value.Value, copied.Value, Enumerable.SequenceEqual));
        }

        [TestMethod]
        public void Null_Tests()
        {
            var value = RecordValue.Null();
            Assert.IsNull(value.Value);

            value = RecordValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(RecordValue.Null().Equals(value));
            Assert.IsTrue(RecordValue.Null().ValueEquals(value));
            Assert.AreEqual(RecordValue.Null().Value, value.Value);
            Assert.AreNotEqual(RecordValue.Null().Annotations.Length, value.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            RecordValue first = ArrayUtil.Of(
                (SymbolValue.Of("key"), DecimalValue.Of(654) as IDiaValue));

            RecordValue second = ArrayUtil.Of(
                (SymbolValue.Of("key"), IntValue.Of(654) as IDiaValue));

            RecordValue third = ArrayUtil.Of(
                (SymbolValue.Of("key"), DecimalValue.Of(654) as IDiaValue));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            RecordValue first = ArrayUtil.Of(
                (SymbolValue.Of("key"), DecimalValue.Of(654) as IDiaValue));

            RecordValue second = ArrayUtil.Of(
                (SymbolValue.Of("key"), IntValue.Of(654) as IDiaValue));

            RecordValue third = ArrayUtil.Of(
                (SymbolValue.Of("key", "annotated"), DecimalValue.Of(654) as IDiaValue));

            Assert.IsTrue(first.Equals(first));
            Assert.IsFalse(first.Equals(third));
            Assert.IsFalse(first.Equals(second));
        }

        [TestMethod]
        public void EmptyTests()
        {
            var value = RecordValue.Empty();
            Assert.AreEqual(0, value.Value!.Length);
        }

        [TestMethod]
        public void ValueWrapperIndexer_Tests()
        {
            var value = RecordValue.Null();
            Assert.ThrowsException<InvalidOperationException>(() => value[null, Array.Empty<Annotation>()] = "stuff");

            value = RecordValue.Empty();
            Assert.ThrowsException<ArgumentNullException>(() => value[null, Array.Empty<Annotation>()] = "stuff");
            Assert.ThrowsException<ArgumentNullException>(() => value["stuff", null] = "stuff");

            value["key"] = "first value";
            Assert.AreEqual(1, value.Count);

            value["key2", "help"] = "second value";
            Assert.AreEqual(2, value.Count);

            var key = value.Keys!.FirstOrDefault(k => k.Annotations.Any(ann => ann.Text == "help"));
            Assert.IsNotNull(key);
        }

        [TestMethod]
        public void Count_Tests()
        {
            var value = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };
            Assert.AreEqual(3, value.Count);
        }

        [TestMethod]
        public void ContainsKey_Test()
        {
            Assert.ThrowsException<InvalidOperationException>(() => RecordValue.Null().ContainsKey("bleh"));
            Assert.ThrowsException<InvalidOperationException>(() => RecordValue.Null().ContainsKey(SymbolValue.Of("bleh")));

            var record = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };

            Assert.IsTrue(record.ContainsKey("1"));
            Assert.IsFalse(record.ContainsKey(SymbolValue.Of("1")));
            Assert.IsFalse(record.ContainsKey(SymbolValue.Of("1", "nein")));
            Assert.IsTrue(record.ContainsKey(SymbolValue.Of("1", "no")));
            Assert.IsTrue(record.ContainsKey("3"));
            Assert.IsTrue(record.ContainsKey(SymbolValue.Of("3")));
        }

        [TestMethod]
        public void Keys_Test()
        {
            Assert.IsNull(RecordValue.Null().Keys);
            Assert.AreEqual(0, RecordValue.Empty().Keys!.Length);

            var record = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };

            var keys = record.Keys!;
            Assert.AreEqual(3, keys.Length);
        }

        [TestMethod]
        public void Values_Test()
        {
            Assert.IsNull(RecordValue.Null().Values);
            Assert.AreEqual(0, RecordValue.Empty().Values!.Length);

            var record = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };

            var values = record.Values!;
            Assert.AreEqual(3, values.Length);
        }

        [TestMethod]
        public void GetOrAdd_Test()
        {
            Assert.ThrowsException<InvalidOperationException>(() => RecordValue.Null().GetOrAdd("bleh", null));
            var record = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };

            Assert.ThrowsException<ArgumentException>(
                () => record.GetOrAdd(SymbolValue.Null(), _ => null));

            Assert.ThrowsException<ArgumentNullException>(
                () => record.GetOrAdd(SymbolValue.Of("stuff"), null));

            var result = record.GetOrAdd(
                SymbolValue.Of("1", "not no"), 
                _ =>
                {
                    Assert.Fail();
                    return IntValue.Of(3);
                });

            Assert.AreEqual(true, ((BoolValue)result).Value);
            Assert.IsTrue(record.TryGetKeySymbol("1", out var key));
            Assert.AreEqual("no", key.Annotations[0].Text);

            result = record.GetOrAdd(
                SymbolValue.Of("4", "not no"),
                _ =>
                {
                    return IntValue.Of(3);
                });
            Assert.AreEqual(IntValue.Of(3), result);
            Assert.IsTrue(record.TryGetKeySymbol("4", out key));
            Assert.AreEqual("not no", key.Annotations[0].Text);
        }

        [TestMethod]
        public void TryAdd_Test()
        {
            Assert.ThrowsException<InvalidOperationException>(() => RecordValue.Null().TryAdd("bleh", IntValue.Of(5)));
            var record = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };

            Assert.ThrowsException<ArgumentException>(
                () => record.TryAdd(SymbolValue.Null(), null));

            Assert.ThrowsException<ArgumentNullException>(
                () => record.TryAdd(SymbolValue.Of("stuff"), null));

            Assert.IsTrue(record.TryAdd(SymbolValue.Of("new.key"), SymbolValue.Of("new.value")));
            Assert.AreEqual(4, record.Count);
            Assert.IsTrue(record.TryGetKeySymbol("new.key", out _));
        }

        [TestMethod]
        public void TryRemove_Test()
        {
            Assert.ThrowsException<InvalidOperationException>(() => RecordValue.Null().TryRemove("bleh", out _));
            var record = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };

            Assert.ThrowsException<ArgumentException>(
                () => record.TryRemove(SymbolValue.Null(), out _));

            Assert.IsFalse(record.TryRemove(SymbolValue.Of("new.key"), out var value));
            Assert.IsNull(value);
            Assert.AreEqual(3, record.Count);

            Assert.IsTrue(record.TryRemove(SymbolValue.Of("1"), out value));
            Assert.IsNotNull(value);
            Assert.AreEqual(2, record.Count);
        }

        [TestMethod]
        public void TryGet_Test()
        {
            Assert.ThrowsException<InvalidOperationException>(() => RecordValue.Null().TryGet("bleh", out _));
            var record = new RecordValue
            {
                [SymbolValue.Of("1", "no")] = BoolValue.Of(true),
                ["2", "yes"] = false,
                ["3"] = DateTimeOffset.Now
            };

            Assert.ThrowsException<ArgumentException>(
                () => record.TryGet(SymbolValue.Null(), out _));

            Assert.IsFalse(record.TryGet(SymbolValue.Of("new.key"), out var value));
            Assert.IsNull(value);

            Assert.IsTrue(record.TryRemove(SymbolValue.Of("1"), out value));
            Assert.IsNotNull(value);
        }
    }
}
