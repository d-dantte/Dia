using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type;
using Axis.Dia.Convert.Type.Converters;
using Axis.Dia.Convert.Type.Exceptions;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Axis.Dia.Tests.Convert.Type.Converters
{
    [TestClass]
    public class CollectionTypeConverterTests
    {
        private static readonly CollectionTypeConverter converter = new CollectionTypeConverter();

        [TestMethod]
        public void CanConvertToClrTests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(DiaType.List, null));

            Enum.GetValues<DiaType>()
                .Where(diaType => !DiaType.List.Equals(diaType))
                .ForAll(diaType =>
                {
                    var canConvert = converter.CanConvert(diaType, typeof(List<int>));
                    Assert.IsFalse(canConvert);
                });


            // array
            var canConvert = converter.CanConvert(DiaType.List, typeof(int[]));
            Assert.IsTrue(canConvert);

            // common collection interfaces and concrete collections
            var collections = new List<System.Type>
            {
                typeof(IEnumerable<int>),
                typeof(ICollection<int>),
                typeof(IList<int>),
                typeof(ISet<int>),
                typeof(IReadOnlyCollection<int>),
                typeof(List<int>),
                typeof(ConcurrentBag<int>),
                typeof(Queue<int>),
                typeof(Stack<int>)
            };

            collections.ForEach(i =>
            {
                var canConvert = converter.CanConvert(DiaType.List, i);
                Assert.IsTrue(canConvert);
            });
        }

        [TestMethod]
        public void CanConvertToDiaTests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(null));

            Assert.IsFalse(converter.CanConvert(typeof(object)));

            Assert.IsTrue(converter.CanConvert(typeof(int[])));
            Assert.IsTrue(converter.CanConvert(typeof(IEnumerable<int>)));
            Assert.IsTrue(converter.CanConvert(typeof(IList<int>)));
            Assert.IsTrue(converter.CanConvert(typeof(IReadOnlyCollection<int>)));
            Assert.IsTrue(converter.CanConvert(typeof(IImmutableList<int>)));
            Assert.IsTrue(converter.CanConvert(typeof(IImmutableSet<int>)));
            Assert.IsTrue(converter.CanConvert(typeof(ICollection<int>)));
            Assert.IsTrue(converter.CanConvert(typeof(List<int>)));
            Assert.IsTrue(converter.CanConvert(typeof(Stack<int>)));
        }

        [TestMethod]
        public void ConvertToClrTests()
        {
            var options = Dia.Convert.Type.Clr.ConverterOptionsBuilder.NewBuilder().Build();

            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToClr(
                    null,
                    typeof(IEnumerable<int>),
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(typeof(IEnumerable<int>)))));

            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToClr(
                    ListValue.Null(),
                    null,
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(typeof(IEnumerable<int>)))));

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => converter.ToClr(
                    IntValue.Null(),
                    typeof(IEnumerable<int>),
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(typeof(IEnumerable<int>)))));

            var result = converter.ToClr(
                ListValue.Null(),
                typeof(object),
                new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(typeof(object))));
            Assert.IsTrue(result.IsErrorResult());
            var error = result.AsError();
            Assert.IsInstanceOfType<IncompatibleClrConversionException>(error.Cause().InnerException);

            IDiaValue diaList = new ListValue { 1, 2, 3 };
            IDiaValue nullDiaList = ListValue.Null();

            (IDiaValue sourceValue, System.Type destinationType, bool success)[] testParams = new[]
            {
                // array
                (diaList, typeof(int[]), true),
                (nullDiaList, typeof(int[]), true),

                // immutable array
                (diaList, typeof(ImmutableArray<int>), true),
                (nullDiaList, typeof(ImmutableArray<int>), true),

                // immutable list
                (diaList, typeof(ImmutableList<int>), true),
                (nullDiaList, typeof(ImmutableList<int>), true),
                (diaList, typeof(IImmutableList<int>), true),
                (nullDiaList, typeof(IImmutableList<int>), true),

                // list
                (diaList, typeof(List<int>), true),
                (nullDiaList, typeof(List<int>), true),
                (diaList, typeof(IEnumerable<int>), true),
                (nullDiaList, typeof(IEnumerable<int>), true),

                // set
                (diaList, typeof(HashSet<int>), true),
                (nullDiaList, typeof(HashSet<int>), true),
                (diaList, typeof(ISet<int>), true),
                (nullDiaList, typeof(ISet<int>), true),

                // arbitrary
                (diaList, typeof(LinkedList<int>), true),
                (diaList, typeof(NonDefaultConstructorList), false)
            };

            testParams.ForAll(args =>
            {
                var result = converter.ToClr(
                    args.sourceValue,
                    args.destinationType,
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(args.destinationType)));
                var succeeded = result.IsDataResult();
                Assert.AreEqual(args.success, succeeded);

                if (args.success)
                {
                    var converted = result.Resolve().As<ICollection<int>>();

                    if (!args.sourceValue.IsNull)
                    {
                        Assert.IsInstanceOfType(converted, args.destinationType);
                        Assert.AreEqual(args.sourceValue.As<ListValue>().Count, converted.Count);
                    }

                    else Assert.IsNull(converted);
                }
            });
        }

        [TestMethod]
        public void ConvertToDiaTests()
        {
            var options = Dia.Convert.Type.Dia.ConverterOptionsBuilder.NewBuilder().Build();

            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToDia(
                    null,
                    Array.Empty<int>(),
                    new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(typeof(IEnumerable<int>)))));

            var result = converter.ToDia(
                typeof(ISet<int>),
                Array.Empty<int>(),
                new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(typeof(ISet<int>))));
            Assert.IsTrue(result.IsErrorResult());
            var error = result.AsError();
            Assert.IsInstanceOfType<TypeMismatchException>(error.Cause().InnerException);

            result = converter.ToDia(
                typeof(object),
                Array.Empty<int>(),
                new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(typeof(object))));
            Assert.IsTrue(result.IsErrorResult());
            error = result.AsError();
            Assert.IsInstanceOfType<UnknownClrSourceTypeException>(error.Cause().InnerException);

            result = converter.ToDia(
                typeof(ISet<int>),
                null,
                new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(typeof(ISet<int>))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(ListValue.Null(), result.Resolve());

            result = converter.ToDia(
                typeof(IEnumerable<int>),
                new[] { 1, 2, 3, 4, 5 },
                new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(typeof(IEnumerable<int>))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(new ListValue { 1, 2, 3, 4, 5 }, result.Resolve());
        }

        #region nested types
        internal class NonDefaultConstructorList : List<int>
        {
            public NonDefaultConstructorList(ICollection<int> items)
            {
                AddRange(items);
            }
        }
        #endregion

    }
}
