using Axis.Dia.Core.Types;
using Axis.Dia.TypeConverter.Clr;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Collections;
using System.Collections.Immutable;

namespace Axis.Dia.TypeConverter.Tests.Clr
{
    [TestClass]
    public class SequenceConverterTests
    {
        [TestMethod]
        public void CanConvert()
        {
            var converter = new SequenceConverter();

            Assert.IsFalse(converter.CanConvert(Core.DiaType.Int, typeof(object).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Sequence, typeof(object).ToTypeInfo()));

            Assert.IsTrue(converter.CanConvert(Core.DiaType.Sequence, typeof(int[]).ToTypeInfo()));
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Sequence, typeof(ImmutableArray<int>).ToTypeInfo()));
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Sequence, typeof(ImmutableList<int>).ToTypeInfo()));
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Sequence, typeof(ICollection<int>).ToTypeInfo()));
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Sequence, typeof(ISet<int>).ToTypeInfo()));
        }

        #region Array
        [TestMethod]
        public void IsArray()
        {
            Assert.IsFalse(SequenceConverter.IsArray(typeof(object).ToTypeInfo()));
            Assert.IsTrue(SequenceConverter.IsArray(typeof(object[]).ToTypeInfo()));
        }

        [TestMethod]
        public void ToArray_Generic()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToArray<int>(
                Sequence.Of(4, 5, 6),
                typeof(int[]).ToTypeInfo(),
                context);
            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(ArrayUtil.Of(4, 5, 6), result);
        }

        [TestMethod]
        public void ToArray()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToArray(
                Sequence.Of(4, 5, 6),
                typeof(int[]).ToTypeInfo(),
                context);
            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(ArrayUtil.Of(4, 5, 6), result);
        }
        #endregion

        #region ImmutableArray
        [TestMethod]
        public void IsImmutableArray()
        {
            Assert.IsFalse(SequenceConverter.IsImmutableArray(typeof(object).ToTypeInfo()));
            Assert.IsTrue(SequenceConverter.IsImmutableArray(typeof(ImmutableArray<object>).ToTypeInfo()));
        }

        [TestMethod]
        public void ToImmutableArray_Generic()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToImmutableArray<int>(
                Sequence.Of(4, 5, 6),
                typeof(ImmutableArray<int>).ToTypeInfo(),
                context);
            Assert.AreEqual(3, result.Length);
            CollectionAssert.AreEqual(ArrayUtil.Of(4, 5, 6), result);
        }

        [TestMethod]
        public void ToImmutableArray()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToImmutableArray(
                Sequence.Of(4, 5, 6),
                typeof(ImmutableArray<int>).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out ImmutableArray<int> array));
            Assert.AreEqual(3, array.Length);
            CollectionAssert.AreEqual(ArrayUtil.Of(4, 5, 6), array);
        }
        #endregion

        #region ImmutableList
        [TestMethod]
        public void IsImmutableList()
        {
            Assert.IsFalse(SequenceConverter.IsImmutableList(typeof(object).ToTypeInfo()));
            Assert.IsTrue(SequenceConverter.IsImmutableList(typeof(ImmutableList<object>).ToTypeInfo()));
        }

        [TestMethod]
        public void ToImmutableList_Generic()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToImmutableList<int>(
                Sequence.Of(4, 5, 6),
                typeof(ImmutableList<int>).ToTypeInfo(),
                context);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEqual(ArrayUtil.Of(4, 5, 6), result);
        }

        [TestMethod]
        public void ToImmutableList()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToImmutableList(
                Sequence.Of(4, 5, 6),
                typeof(ImmutableList<int>).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out ImmutableList<int> array));
            Assert.AreEqual(3, array.Count);
            CollectionAssert.AreEqual(ArrayUtil.Of(4, 5, 6), array);
        }
        #endregion

        #region Collection
        [TestMethod]
        public void IsCollection()
        {
            Assert.IsFalse(SequenceConverter.IsCollection(typeof(object).ToTypeInfo()));
            Assert.IsTrue(SequenceConverter.IsCollection(typeof(ICollection<object>).ToTypeInfo()));
        }

        [TestMethod]
        public void IsEnumerable()
        {
            Assert.IsFalse(SequenceConverter.IsEnumerable(typeof(object).ToTypeInfo()));
            Assert.IsTrue(SequenceConverter.IsEnumerable(typeof(IEnumerable<object>).ToTypeInfo()));
        }

        [TestMethod]
        public void NewCollection()
        {
            var collection = SequenceConverter.NewCollection<int>(
                typeof(List<int>));
            Assert.IsInstanceOfType<List<int>>(collection);

            collection = SequenceConverter.NewCollection<int>(
                typeof(LinkedList<int>));
            Assert.IsInstanceOfType<LinkedList<int>>(collection);

            collection = SequenceConverter.NewCollection<int>(
                typeof(FakeCollection<int>));
            Assert.IsInstanceOfType<FakeCollection<int>>(collection);

            collection = SequenceConverter.NewCollection<int>(
                typeof(IEnumerable<int>));
            Assert.IsInstanceOfType<List<int>>(collection);

            collection = SequenceConverter.NewCollection<int>(
                typeof(ICollection<int>));
            Assert.IsInstanceOfType<List<int>>(collection);

            collection = SequenceConverter.NewCollection<int>(
                typeof(IList<int>));
            Assert.IsInstanceOfType<List<int>>(collection);

            collection = SequenceConverter.NewCollection<int>(
                typeof(IReadOnlyCollection<int>));
            Assert.IsInstanceOfType<List<int>>(collection);

            collection = SequenceConverter.NewCollection<int>(
                typeof(IReadOnlyList<int>));
            Assert.IsInstanceOfType<List<int>>(collection);

            Assert.ThrowsException<InvalidOperationException>(
                () => SequenceConverter.NewCollection<int>(typeof(FakeCollection2<int>)));
        }

        [TestMethod]
        public void ToCollection_Generic()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToCollection<int>(
                Sequence.Of(4, 5, 6),
                typeof(ICollection<int>).ToTypeInfo(),
                context);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(ArrayUtil.Of(4, 5, 6), result));
        }

        [TestMethod]
        public void ToCollection()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToCollection(
                Sequence.Of(4, 5, 6),
                typeof(ICollection<int>).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out ICollection<int> array));
            Assert.AreEqual(3, array.Count);
            Assert.IsTrue(Enumerable.SequenceEqual(ArrayUtil.Of(4, 5, 6), array));
        }
        #endregion

        #region Set
        [TestMethod]
        public void IsSet()
        {
            Assert.IsFalse(SequenceConverter.IsSet(typeof(object).ToTypeInfo()));
            Assert.IsTrue(SequenceConverter.IsSet(typeof(ISet<object>).ToTypeInfo()));
        }

        [TestMethod]
        public void NewSet()
        {
            var collection = SequenceConverter.NewSet<int>(
                typeof(HashSet<int>));
            Assert.IsInstanceOfType<HashSet<int>>(collection);

            collection = SequenceConverter.NewSet<int>(
                typeof(FakeSet<int>));
            Assert.IsInstanceOfType<FakeSet<int>>(collection);

            collection = SequenceConverter.NewSet<int>(
                typeof(ISet<int>));
            Assert.IsInstanceOfType<HashSet<int>>(collection);

            Assert.ThrowsException<InvalidOperationException>(
                () => SequenceConverter.NewSet<int>(typeof(FakeSet2<int>)));
        }

        [TestMethod]
        public void ToSet_Generic()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToSet<int>(
                Sequence.Of(4, 5, 6),
                typeof(ISet<int>).ToTypeInfo(),
                context);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(ArrayUtil.Of(4, 5, 6).ToHashSet().SetEquals(result));
        }

        [TestMethod]
        public void ToSet()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);

            var result = SequenceConverter.ToSet(
                Sequence.Of(4, 5, 6),
                typeof(ISet<int>).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out ISet<int> set));
            Assert.AreEqual(3, set.Count);
            Assert.IsTrue(ArrayUtil.Of(4, 5, 6).ToHashSet().SetEquals(set));
        }
        #endregion

        [TestMethod]
        public void ToClr_Tests()
        {
            var options = Options
                .NewBuilder()
                .Build();
            var context = new ConverterContext(options);
            var converter = new SequenceConverter();
            var seq = Sequence.Of(1, 2, 3, 4, 5);
            var values = seq.Select(v => (int)v.AsInteger().Value!.Value);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToClr(seq, typeof(Guid).ToTypeInfo(), context));

            var result = converter.ToClr(seq, typeof(int[]).ToTypeInfo(), context);
            Assert.IsTrue(result.Is(out int[] arr));
            Assert.AreEqual(5, arr.Length);
            Assert.IsTrue(values.SequenceEqual(arr));

            context = new ConverterContext(options);
            result = converter.ToClr(seq, typeof(ImmutableArray<int>).ToTypeInfo(), context);
            Assert.IsTrue(result.Is(out ImmutableArray<int> imarr));
            Assert.AreEqual(5, imarr.Length);
            Assert.IsTrue(values.SequenceEqual(imarr));

            context = new ConverterContext(options);
            result = converter.ToClr(seq, typeof(ImmutableList<int>).ToTypeInfo(), context);
            Assert.IsTrue(result.Is(out ImmutableList<int> imlist));
            Assert.AreEqual(5, imlist.Count);
            Assert.IsTrue(values.SequenceEqual(imlist));

            context = new ConverterContext(options);
            result = converter.ToClr(seq, typeof(HashSet<int>).ToTypeInfo(), context);
            Assert.IsTrue(result.Is(out ISet<int> set));
            Assert.AreEqual(5, set.Count);
            Assert.IsTrue(values.ToHashSet().SetEquals(set));

            context = new ConverterContext(options);
            result = converter.ToClr(seq, typeof(IList<int>).ToTypeInfo(), context);
            Assert.IsTrue(result.Is(out List<int> list));
            Assert.AreEqual(5, list.Count);
            Assert.IsTrue(values.SequenceEqual(list));
        }


        #region nested types
        internal class FakeCollection<T> : ICollection<T>
        {
            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        internal class FakeCollection2<T> : ICollection<T>
        {
            public FakeCollection2(int dummy) { }
            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        internal class FakeSet<T> : ISet<T>
        {
            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public bool Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void UnionWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            void ICollection<T>.Add(T item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        internal class FakeSet2<T> : ISet<T>
        {
            public FakeSet2(int dummy) { }

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public bool Add(T item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void UnionWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            void ICollection<T>.Add(T item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
