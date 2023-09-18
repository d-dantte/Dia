using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type;
using Axis.Dia.Convert.Type.Converters;
using Axis.Dia.Convert.Type.Exceptions;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Tests.Convert.Type.Converters
{
    [TestClass]
    public class RecordTypeConverterTests
    {
        private static readonly RecordTypeConverter converter = new RecordTypeConverter();

        [TestMethod]
        public void CanConvertToClr_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(DiaType.Record, null));

            (DiaType diaType, System.Type clrType, bool success)[] args = new[]
            {
                (DiaType.Record, typeof(Obj1), true),
                (DiaType.Record, typeof(Obj2), true),
                (DiaType.Record, typeof(Obj3), true),
                (DiaType.Record, typeof(Obj4), true),

                (DiaType.List, typeof(Obj1), false),
                (DiaType.Bool, typeof(Obj2), false),
                (DiaType.Decimal, typeof(Obj3), false),
                (DiaType.Blob, typeof(Obj4), false),


                (DiaType.Record, typeof(Dictionary<string, int>), true),
                (DiaType.Record, typeof(Dictionary<string, int>), true),
                (DiaType.Record, typeof(Dictionary<string, int>), true),
                (DiaType.Record, typeof(Dictionary<string, int>), true),

                (DiaType.List, typeof(Dictionary<string, int>), false),
                (DiaType.Bool, typeof(Dictionary<string, int>), false),
                (DiaType.Decimal, typeof(Dictionary<string, int>), false),
                (DiaType.Blob, typeof(Dictionary<string, int>), false),


                (DiaType.Record, typeof(IDictionary<string, int>), true),
                (DiaType.Record, typeof(IDictionary<string, int>), true),
                (DiaType.Record, typeof(IDictionary<string, int>), true),
                (DiaType.Record, typeof(IDictionary<string, int>), true),

                (DiaType.List, typeof(IDictionary<string, int>), false),
                (DiaType.Bool, typeof(IDictionary<string, int>), false),
                (DiaType.Decimal, typeof(IDictionary<string, int>), false),
                (DiaType.Blob, typeof(IDictionary<string, int>), false),


                (DiaType.Record, typeof(IDictionary<Guid, int>), false),
                (DiaType.Record, typeof(IDictionary<Guid, int>), false),
                (DiaType.Record, typeof(IDictionary<Guid, int>), false),
                (DiaType.Record, typeof(IDictionary<Guid, int>), false),

                (DiaType.List, typeof(IDictionary<Guid, int>), false),
                (DiaType.Bool, typeof(IDictionary<Guid, int>), false),
                (DiaType.Decimal, typeof(IDictionary<Guid, int>), false),
                (DiaType.Blob, typeof(IDictionary<Guid, int>), false),
            };

            foreach (var (diaType, clrType, success) in args)
            {
                var result = converter.CanConvert(diaType, clrType);
                Assert.AreEqual(success, result);
            }
        }

        [TestMethod]
        public void CanConvertToDia_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(DiaType.Record, null));

            (System.Type clrType, bool success)[] args = new[]
            {
                (typeof(Obj1), true),
                (typeof(Obj2), true),
                (typeof(Obj3), true),
                (typeof(Obj4), true),

                (typeof(Dictionary<string, int>), true),
                (typeof(IDictionary<string, int>), true),

                (typeof(Dictionary<Guid, int>), false),
                (typeof(IDictionary<Guid, int>), false)
            };

            foreach (var (clrType, success) in args)
            {
                var result = converter.CanConvert(clrType);
                Assert.AreEqual(success, result);
            }
        }

        [TestMethod]
        public void ConvertToClr_Tests()
        {
            var options = Dia.Convert.Type.Clr.ConverterOptionsBuilder.NewBuilder().Build();

            #region Argument exceptions
            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToClr(
                    null,
                    typeof(Obj4),
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(typeof(Obj4)))));

            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToClr(
                    RecordValue.Null(),
                    null,
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(typeof(Obj1)))));

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => converter.ToClr(
                    ListValue.Null(),
                    typeof(Obj1),
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(typeof(Obj1)))));

            var result = converter.ToClr(
                RecordValue.Null(),
                typeof(Dictionary<Guid, int>),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Dictionary<Guid, int>))));
            Assert.IsTrue(result.IsErrorResult());
            Assert.IsInstanceOfType<IncompatibleClrConversionException>(result.AsError().Cause().InnerException);
            #endregion

            result = converter.ToClr(
                RecordValue.Null(),
                typeof(Obj1),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj1))));
            Assert.IsTrue(result.IsDataResult());
            Assert.IsNull(result.AsData().Resolve());

            var record1 = new RecordValue
            {
                ["Name"] = "First Last name",
                ["description", "relentless"] = "Some annotated descirption comes here",
                ["D_ob"] = DateTimeOffset.Now
            };

            var record2 = new RecordValue
            {
                ["Name"] = "First Last name",
                ["description", "relentless"] = "Some annotated descirption comes here"
            };

            var record3 = new RecordValue
            {
                ["first_record"] = record2,
                ["ref_of_record1"] = record2.ToRef()
            };

            record3["self_ref"] = record3.ToRef();

            Assert.IsTrue(record3.TryNormalizeReferences(out var unlinkedReferences));

            #region Maps
            result = converter.ToClr(
                record2,
                typeof(IDictionary<string, string>),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(IDictionary<string, string>))));
            Assert.IsTrue(result.IsDataResult());
            var dictionary = result.Map(r => (IDictionary<string, string>)r!).Resolve();
            Assert.AreEqual(2, dictionary.Count);
            Assert.IsTrue(dictionary.ContainsKey(record2.Value![0].Key.Value!));
            Assert.IsTrue(dictionary.ContainsKey(record2.Value![1].Key.Value!));

            result = converter.ToClr(
                RecordValue.Null(),
                typeof(IDictionary<string, string>),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(IDictionary<string, string>))));
            Assert.IsTrue(result.IsDataResult());
            dictionary = result.Map(r => (IDictionary<string, string>)r!).Resolve();
            Assert.IsNull(dictionary);

            result = converter.ToClr(
                record1,
                typeof(IDictionary<string, string>),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(IDictionary<string, string>))));
            Assert.IsTrue(result.IsErrorResult());

            result = converter.ToClr(
                record3,
                typeof(IDictionary<string, IDictionary<string, string>>),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(IDictionary<string, IDictionary<string, string>>))));
            Assert.IsTrue(result.IsDataResult());
            #endregion

            #region objects
            result = converter.ToClr(
                RecordValue.Null(),
                typeof(Obj1),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj1))));
            Assert.IsTrue(result.IsDataResult());
            Assert.IsNull(result.Resolve());

            result = converter.ToClr(
                RecordValue.Null(),
                typeof(Obj4),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj4))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(default(Obj4), result.Resolve());

            // obj1
            result = converter.ToClr(
                record1,
                typeof(Obj1),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj1))));
            Assert.IsTrue(result.IsDataResult());
            var obj1 = result.Map(obj => (Obj1)obj!).Resolve();
            Assert.IsInstanceOfType<Obj1>(obj1);
            Assert.IsTrue(
                record1.TryGet<StringValue>("Name", out var svalue)
                && svalue.Value!.Equals(obj1.Name));
            Assert.IsTrue(
                record1.TryGet("description", out svalue)
                && svalue.Value!.Equals(obj1.Description));
            Assert.IsTrue(
                record1.TryGet<InstantValue>("D_ob", out var ivalue)
                && ivalue.Value!.Equals(obj1.Dob));

            // obj2
            result = converter.ToClr(
                record1,
                typeof(Obj2),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj2))));
            Assert.IsTrue(result.IsDataResult());
            var obj2 = result.Map(obj => (Obj2)obj!).Resolve();
            Assert.IsInstanceOfType<Obj2>(obj2);
            Assert.IsTrue(
                record1.TryGet("Name", out svalue)
                && svalue.Value!.Equals(obj2._name));
            Assert.IsTrue(
                record1.TryGet("D_ob", out ivalue)
                && ivalue.Value!.Equals(obj2.Dob));

            // obj3
            result = converter.ToClr(
                record1,
                typeof(Obj3),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj3))));
            Assert.IsTrue(result.IsDataResult());
            var obj3 = result.Map(obj => (Obj3)obj!).Resolve();
            Assert.IsInstanceOfType<Obj3>(obj3);
            Assert.IsTrue(
                record1.TryGet("Name", out svalue)
                && svalue.Value!.Equals(obj3.Name));
            Assert.IsTrue(
                record1.TryGet("description", out svalue)
                && svalue.Value!.Equals(obj3.Description));
            Assert.IsTrue(
                record1.TryGet("D_ob", out ivalue)
                && ivalue.Value!.Equals(obj3.Dob));

            // obj4
            result = converter.ToClr(
                record1,
                typeof(Obj4),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj1))));
            Assert.IsTrue(result.IsDataResult());
            var obj4 = result.Map(obj => (Obj4)obj!).Resolve();
            Console.WriteLine(obj4);
            Assert.IsInstanceOfType<Obj4>(obj4);
            Assert.IsTrue(
                record1.TryGet("Name", out svalue)
                && svalue.Value!.Equals(obj4.Name));
            Assert.IsTrue(
                record1.TryGet("description", out svalue)
                && svalue.Value!.Equals(obj4.Description));
            Assert.IsTrue(
                record1.TryGet("D_ob", out ivalue)
                && ivalue.Value!.Equals(obj4.Dob));
            #endregion
        }

        [TestMethod]
        public void ConvertToClr_Tests2()
        {
            var options = Dia.Convert.Type.Clr.ConverterOptionsBuilder.NewBuilder().Build();
            var record1 = new RecordValue
            {
                ["abc"] = 4
            };

            record1["self"] = record1.ToRef();
            
            var result = converter.ToClr(
                record1,
                typeof(Obj5),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj5))));

            Assert.IsTrue(result.IsDataResult());
            var obj = result.Resolve().As<Obj5>();

            var record2 = new RecordValue();
            record2["self"] = record2.ToRef();

            result = TypeConverter.ToClr(
                record2,
                typeof(IDictionary<string, IDictionary<string, int>>));

            Assert.IsTrue(result.IsDataResult());
            
        }

        [TestMethod]
        public void ConvertToDia_Tests()
        {
            var options = Dia.Convert.Type.Dia.ConverterOptionsBuilder.NewBuilder().Build();

            #region Argument exceptions
            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToDia(
                    null,
                    new object(),
                    new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(typeof(Obj4)))));

            var result = converter.ToDia(
                typeof(Dictionary<Guid, int>),
                null,
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(Dictionary<Guid, int>))));
            Assert.IsTrue(result.IsErrorResult());
            Assert.IsInstanceOfType<UnknownClrSourceTypeException>(result.AsError().Cause().InnerException);

            result = converter.ToDia(
                typeof(Dictionary<string, int>),
                new Obj1(),
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(Dictionary<Guid, int>))));
            Assert.IsTrue(result.IsErrorResult());
            Assert.IsInstanceOfType<TypeMismatchException>(result.AsError().Cause().InnerException);

            result = converter.ToDia(
                typeof(Dictionary<string, int>),
                null,
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(Dictionary<Guid, int>))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(RecordValue.Null(), result.Resolve());
            #endregion

            #region Maps
            var obj3 = new Obj3();
            var map = new Dictionary<string, Obj3?>
            {
                ["first"] = null,
                ["second"] = obj3,
                ["third"] = new Obj3()
                {
                    Name = "name",
                    Description = null,
                    Dob = DateTimeOffset.Now
                },
                ["fourth"] = obj3
            };

            result = converter.ToDia(map.GetType(), map, new Dia.Convert.Type.Dia.ConverterContext(
                options,
                new ObjectPath(map.GetType())));
            Assert.IsTrue(result.IsDataResult());
            #endregion
        }

        public class Obj1
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTimeOffset Dob { get; set; }
        }

        public class Obj2
        {
            public string? _name;
            public string? Name { set => _name = value; }
            public string? Description { get => null; }
            public DateTimeOffset Dob { get; set; }
        }

        public class Obj3
        {
            public Obj3()
            { }

            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTimeOffset Dob { get; set; }
        }

        public struct Obj4
        {
            public Obj4(string name)
            {
                Name = name;
            }

            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTimeOffset Dob { get; set; }

            public override string ToString()
            {
                return $"[Name: '{Name}', Desc: '{Description}', Dob: '{Dob}']";
            }
        }

        public class Obj5
        {
            public int Abc { get; set; }

            public Obj5? Self { get; set; }
        }

    }
}
