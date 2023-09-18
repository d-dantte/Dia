using Axis.Dia.Convert.Type.Converters;
using Axis.Luna.Common.Numerics;
using System.Numerics;
using Axis.Dia.Types;
using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type;
using Axis.Luna.Common.Results;
using System.Collections.Immutable;
using Axis.Dia.Convert.Type.Exceptions;
using static Axis.Dia.Tests.Convert.Type.Converters.RecordTypeConverterTests;

namespace Axis.Dia.Tests.Convert.Type.Converters
{
    [TestClass]
    public class SimpleTypeConverterTests
    {
        private static readonly SimpleTypeConverter converter = new SimpleTypeConverter();

        [TestMethod]
        public void CanConvertToDia_Tests()
        {
            var types = new[]
            {
                (typeof(byte), (object)(byte)4),
                (typeof(sbyte), (sbyte)4),
                (typeof(short), (short)4),
                (typeof(ushort), (ushort)4),
                (typeof(int), 4),
                (typeof(uint), (uint)4),
                (typeof(long), 4L),
                (typeof(ulong), 4ul),
                (typeof(float), 4f),
                (typeof(double), 4.0),
                (typeof(Half), (Half)4.0),
                (typeof(decimal), 4m),
                (typeof(bool), false),
                (typeof(DateTime), DateTime.Now),
                (typeof(DateTimeOffset), DateTimeOffset.Now),
                (typeof(BigInteger), new BigInteger(4)),
                (typeof(BigDecimal), new BigDecimal(4)),
                (typeof(string), "stuff"),
                (typeof(byte[]), Array.Empty<byte>()),
                (typeof(ImmutableArray<byte>), ImmutableArray.Create<byte>()),
                (typeof(ImmutableList<byte>), ImmutableList.Create<byte>()),
                (typeof(List<byte>), new List<byte>())
            };

            var nullableBase = typeof(Nullable<>);

            foreach (var type in types)
            {
                var result = converter.CanConvert(type.Item1);
                Assert.IsTrue(result);

                if (type.Item1.IsValueType)
                {
                    var nresult = converter.CanConvert(nullableBase.MakeGenericType(type.Item1));
                    Assert.IsTrue(nresult);
                }
            }

            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(null));
        }

        [TestMethod]
        public void CanConvertToClr_Tests()
        {
            var diaInt = new IntValue(4);
            var diaDecimal = new DecimalValue(4);
            var diaBool = new BoolValue(false);
            var diaTimestamp = new InstantValue(DateTimeOffset.Now);
            var diaString = new StringValue("stuff");
            var diaBlob = new BlobValue(Array.Empty<byte>());
            (System.Type clrType, IDiaValue diaValue)[] types = new[]
            {
                (typeof(byte), (IDiaValue)diaInt),
                (typeof(sbyte), diaInt),
                (typeof(short), diaInt),
                (typeof(ushort), diaInt),
                (typeof(int), diaInt),
                (typeof(uint), diaInt),
                (typeof(long), diaInt),
                (typeof(ulong), diaInt),
                (typeof(Half), diaDecimal),
                (typeof(float), diaDecimal),
                (typeof(double), diaDecimal),
                (typeof(decimal), diaDecimal),
                (typeof(bool), diaBool),
                (typeof(DateTime), diaTimestamp),
                (typeof(DateTimeOffset), diaTimestamp),
                (typeof(BigInteger), diaInt),
                (typeof(BigDecimal), diaDecimal),
                (typeof(string), diaString),
                (typeof(byte[]), diaBlob),
                (typeof(ImmutableArray<byte>), diaBlob),
                (typeof(ImmutableList<byte>), diaBlob),
                (typeof(List<byte>), diaBlob)
            };

            var nullableBase = typeof(Nullable<>);

            foreach (var type in types)
            {
                var result = converter.CanConvert(type.diaValue.Type, type.clrType);
                Assert.IsTrue(result);

                if (type.Item1.IsValueType)
                {
                    var nresult = converter.CanConvert(type.diaValue.Type, nullableBase.MakeGenericType(type.clrType));
                    Assert.IsTrue(nresult);
                }
            }

            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(DiaType.String, null));
        }

        [TestMethod]
        public void ToClr_Tests()
        {
            var nullableBase = typeof(Nullable<>);

            #region Types
            // bool
            IDiaValue boolValue = new BoolValue(true);
            IDiaValue nboolValue = IDiaValue.NullOf(DiaType.Bool);

            // int
            IDiaValue intValue = new IntValue(4);
            IDiaValue nintValue = IDiaValue.NullOf(DiaType.Int);

            // decimal
            IDiaValue decimalValue = new DecimalValue(4);
            IDiaValue ndecimalValue = IDiaValue.NullOf(DiaType.Decimal);

            // timestamp
            IDiaValue timestampValue = new InstantValue(DateTime.Now);
            IDiaValue ntimestampValue = IDiaValue.NullOf(DiaType.Instant);

            // string
            IDiaValue stringValue = new StringValue("stuff");
            IDiaValue nstringValue = IDiaValue.NullOf(DiaType.String);

            // blob
            IDiaValue blobValue = new BlobValue(new byte[] { 0,1,2,3,4,5,6,7,8,9});
            IDiaValue nblobValue = BlobValue.Null();
            #endregion

            (System.Type type, IDiaValue dia, IDiaValue nullDia, IDiaValue incompatibleDia)[] types = new[]
            {
                (typeof(bool), boolValue, nboolValue, stringValue),
                (typeof(byte), intValue, nintValue, stringValue),
                (typeof(sbyte), intValue, nintValue, stringValue),
                (typeof(short), intValue, nintValue, stringValue),
                (typeof(ushort), intValue, nintValue, stringValue),
                (typeof(int), intValue, nintValue, stringValue),
                (typeof(uint), intValue, nintValue, stringValue),
                (typeof(long), intValue, nintValue, stringValue),
                (typeof(ulong), intValue, nintValue, stringValue),
                (typeof(BigInteger), intValue, nintValue, stringValue),
                (typeof(Half), decimalValue, ndecimalValue, stringValue),
                (typeof(float), decimalValue, ndecimalValue, stringValue),
                (typeof(double), decimalValue, ndecimalValue, stringValue),
                (typeof(decimal), decimalValue, ndecimalValue, stringValue),
                (typeof(BigDecimal), decimalValue, ndecimalValue, stringValue),
                (typeof(DateTime), timestampValue, ntimestampValue, stringValue),
                (typeof(DateTimeOffset), timestampValue, ntimestampValue, stringValue),
                (typeof(byte[]), blobValue, nblobValue, stringValue),
                (typeof(ImmutableArray<byte>), blobValue, nblobValue, stringValue),
                (typeof(ImmutableList<byte>), blobValue, nblobValue, stringValue),
                (typeof(List<byte>), blobValue , nblobValue, stringValue),
                (typeof(string), stringValue , nstringValue, boolValue)
            };

            var options = Dia.Convert.Type.Clr.ConverterOptionsBuilder.NewBuilder().Build();

            Assert.ThrowsException<ArgumentNullException>(() => converter.ToClr(
                intValue,
                null,
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(Obj1)))));
            Assert.ThrowsException<ArgumentNullException>(() => converter.ToClr(
                null,
                typeof(int),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(int)))));

            foreach (var info in types)
            {
                IResult<object?> result;

                // primitive/ion
                result = converter.ToClr(info.dia, info.type,
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(info.type)));
                Assert.IsNotNull(result);
                Assert.AreEqual(info.type, result.Resolve()?.GetType());

                // primitive/null-dia-primitive
                result = converter.ToClr(info.nullDia, info.type,
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(info.type)));
                Assert.IsNull(result.Resolve());

                // incompatible error
                result = converter.ToClr(info.incompatibleDia, info.type,
                    new Dia.Convert.Type.Clr.ConverterContext(
                        options,
                        new ObjectPath(info.type)));
                Assert.IsTrue(result.IsErrorResult());
                var error = result.AsError();
                Assert.IsTrue(error.Cause().InnerException is IncompatibleClrConversionException);

                if (info.type.IsValueType)
                {
                    var ntype = nullableBase.MakeGenericType(info.type);

                    // nullable-primitive/ion-primitive
                    result = converter.ToClr(info.dia, ntype,
                        new Dia.Convert.Type.Clr.ConverterContext(
                            options,
                            new ObjectPath(ntype)));
                    Assert.IsNotNull(result.Resolve());
                    Assert.AreEqual(info.type, result.Resolve()?.GetType());

                    // nullable-primitive/null-ion-primitive
                    result = converter.ToClr(info.nullDia, ntype,
                        new Dia.Convert.Type.Clr.ConverterContext(
                            options,
                            new ObjectPath(ntype)));
                    Assert.IsNull(result.Resolve());
                }
            }
        }


        [TestMethod]
        public void ToDia_Tests()
        {
            (System.Type type, System.Type ntype, DiaType diaType, object value, object incompatible)[] args = new[]
            {
                (typeof(byte), typeof(byte?), DiaType.Int, (object)(byte)5, (object)"bleh"),
                (typeof(sbyte), typeof(sbyte?), DiaType.Int, (object)(sbyte)5, (object)"bleh"),
                (typeof(short), typeof(short?), DiaType.Int, (object)5, (object)"bleh"),
                (typeof(short), typeof(short?), DiaType.Int, (object)5, (object)"bleh"),
                (typeof(ushort), typeof(ushort?), DiaType.Int, (object)5, (object)"bleh"),
                (typeof(int), typeof(int?), DiaType.Int, (object)5, (object)"bleh"),
                (typeof(uint), typeof(uint?), DiaType.Int, (object)5u, (object)"bleh"),
                (typeof(long), typeof(long?), DiaType.Int, (object)5L, (object)"bleh"),
                (typeof(ulong), typeof(ulong?), DiaType.Int, (object)5ul, (object)"bleh"),
                (typeof(BigInteger), typeof(BigInteger?), DiaType.Int, (object)new BigInteger(5), (object)"bleh"),
                (typeof(Half), typeof(Half?), DiaType.Decimal, (object)(Half)5.0, (object)"bleh"),
                (typeof(float), typeof(float?), DiaType.Decimal, (object)5.0, (object)"bleh"),
                (typeof(double), typeof(double?), DiaType.Decimal, (object)5.0, (object)"bleh"),
                (typeof(decimal), typeof(decimal?), DiaType.Decimal, (object)5.0m, (object)"bleh"),
                (typeof(BigDecimal), typeof(BigDecimal?), DiaType.Decimal, (object)new BigDecimal(5.0m), (object)"bleh"),
                (typeof(bool), typeof(bool?), DiaType.Bool, (object)false, (object)"bleh"),
                (typeof(DateTime), typeof(DateTime?), DiaType.Instant, (object)DateTime.Now, (object)"bleh"),
                (typeof(DateTimeOffset), typeof(DateTimeOffset?), DiaType.Instant, (object)DateTimeOffset.Now, (object)"bleh"),
                (typeof(byte[]), typeof(byte[]), DiaType.Blob, (object)Array.Empty<byte>(), (object)"bleh"),
                (typeof(ImmutableArray<byte>), typeof(ImmutableArray<byte>?), DiaType.Blob, (object)ImmutableArray.Create<byte>(), (object)"bleh"),
                (typeof(ImmutableList<byte>), typeof(ImmutableArray<byte>), DiaType.Blob, (object)ImmutableList.Create<byte>(), (object)"bleh"),
                (typeof(List<byte>), typeof(ImmutableArray<byte>), DiaType.Blob, (object)new List<byte>(), (object)"bleh"),
                (typeof(string), typeof(string), DiaType.String, (object)"abcd", (object)34)
            };

            var options = Dia.Convert.Type.Dia.ConverterOptionsBuilder.NewBuilder().Build();

            Assert.ThrowsException<ArgumentNullException>(() => converter.ToDia(null, null,
                    new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(typeof(object)))));

            var result = converter.ToDia(typeof(KeyValuePair<string, string>), KeyValuePair.Create("", ""),
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(KeyValuePair<string, string>))));
            Assert.IsTrue(result.IsErrorResult());
            var errorResult = result.AsError();
            Assert.IsTrue(errorResult.Cause().InnerException is UnknownClrSourceTypeException);

            foreach (var info in args)
            {
                // clr-type/value
                result = converter.ToDia(info.type, info.value,
                    new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(info.type)));
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Resolve().Type, info.diaType);
                if (info.type == typeof(DateTime))
                {
                    var ts = ((InstantValue)result.Resolve()).Value ?? throw new ArgumentNullException();
                    Assert.AreEqual(info.value, ts.DateTime);
                }
                else
                {
                    var diaResult = result.Resolve();

                    if (diaResult is IntValue i)
                        Assert.AreEqual(i.Value, BigInteger.Parse(info.value.ToString()!));

                    if (diaResult is DecimalValue d)
                        Assert.AreEqual(d.Value, BigDecimal.Parse(info.value.ToString()!).Resolve());

                    if (diaResult is InstantValue ins)
                    {
                        if (info.value is DateTimeOffset dto)
                            Assert.AreEqual(ins.Value, dto);

                        else Assert.AreEqual(ins.Value, new DateTimeOffset((DateTime)info.value));
                    }
                }

                // clr-type/null
                result = converter.ToDia(info.type, null,
                    new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(info.type)));
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Resolve().Type, info.diaType);
                Assert.IsTrue(result.Resolve().IsNull);

                // clr-type/invalid-value
                result = converter.ToDia(info.type, info.incompatible,
                    new Dia.Convert.Type.Dia.ConverterContext(
                        options,
                        new ObjectPath(info.type)));
                Assert.IsTrue(result.IsErrorResult());
                errorResult = result.AsError();
                Assert.IsTrue(errorResult.Cause().InnerException is TypeMismatchException);


                if (info.type.IsValueType)
                {
                    // nullable-clr-type/value
                    result = converter.ToDia(info.ntype, info.value,
                        new Dia.Convert.Type.Dia.ConverterContext(
                            options,
                            new ObjectPath(info.ntype)));
                    Assert.IsNotNull(result);
                    Assert.AreEqual(result.Resolve().Type, info.diaType);
                    if (info.type == typeof(DateTime))
                    {
                        var ts = ((InstantValue)result.Resolve()).Value ?? throw new ArgumentNullException();
                        Assert.AreEqual(info.value, ts.DateTime);
                    }
                    else
                    {
                        var diaResult = result.Resolve();

                        if (diaResult is IntValue i)
                            Assert.AreEqual(i.Value, BigInteger.Parse(info.value.ToString()!));

                        if (diaResult is DecimalValue d)
                            Assert.AreEqual(d.Value, BigDecimal.Parse(info.value.ToString()!).Resolve());

                        if (diaResult is InstantValue ins)
                        {
                            if (info.value is DateTimeOffset dto)
                                Assert.AreEqual(ins.Value, dto);

                            else Assert.AreEqual(ins.Value, new DateTimeOffset((DateTime)info.value));
                        }
                    }

                    // nullable-clr-type/null
                    result = converter.ToDia(info.ntype, null,
                        new Dia.Convert.Type.Dia.ConverterContext(
                            options,
                            new ObjectPath(info.ntype)));
                    Assert.IsNotNull(result);
                    Assert.AreEqual(result.Resolve().Type, info.diaType);
                    Assert.IsTrue(result.Resolve().IsNull);

                    // nullable-clr-type/invalid-value
                    result = converter.ToDia(info.ntype, info.incompatible,
                        new Dia.Convert.Type.Dia.ConverterContext(
                            options,
                            new ObjectPath(info.ntype)));
                    Assert.IsTrue(result.IsErrorResult());
                    errorResult = result.AsError();
                    Assert.IsTrue(errorResult.Cause().InnerException is TypeMismatchException);
                }
            }
        }
    }
}
