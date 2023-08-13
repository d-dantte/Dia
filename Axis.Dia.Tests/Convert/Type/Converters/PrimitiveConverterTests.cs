using Axis.Dia.Convert.Type.Converters;
using Axis.Luna.Common.Numerics;
using System.Numerics;
using Axis.Dia.Types;
using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type;
using Axis.Luna.Extensions;
using Axis.Luna.Common.Results;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Axis.Dia.Tests.Convert.Type.Converters
{
    [TestClass]
    public class PrimitiveConverterTests
    {
        private static readonly PrimitiveConverter converter = new PrimitiveConverter();

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
                (typeof(string), "stuff")
            };

            var nullableBase = typeof(Nullable<>);

            foreach (var type in types)
            {
                var result = converter.CanConvert(type.Item1);
                Assert.IsTrue(result);

                if (type.Item1 != typeof(string))
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
                (typeof(string), diaString)
            };

            var nullableBase = typeof(Nullable<>);

            foreach (var type in types)
            {
                var result = converter.CanConvert(type.diaValue.Type, type.clrType);
                Assert.IsTrue(result);

                if (type.Item1 != typeof(string))
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
            #endregion

            (System.Type type, IDiaValue dia, IDiaValue nullDia)[] types = new[]
            {
                (typeof(bool), boolValue, nboolValue),
                (typeof(byte), intValue, nintValue),
                (typeof(sbyte), intValue, nintValue),
                (typeof(short), intValue, nintValue),
                (typeof(ushort), intValue, nintValue),
                (typeof(int), intValue, nintValue),
                (typeof(uint), intValue, nintValue),
                (typeof(long), intValue, nintValue),
                (typeof(ulong), intValue, nintValue),
                (typeof(BigInteger), intValue, nintValue),
                (typeof(Half), decimalValue, ndecimalValue),
                (typeof(float), decimalValue, ndecimalValue),
                (typeof(double), decimalValue, ndecimalValue),
                (typeof(decimal), decimalValue, ndecimalValue),
                (typeof(BigDecimal), decimalValue, ndecimalValue),
                (typeof(DateTime), timestampValue, ntimestampValue),
                (typeof(DateTimeOffset), timestampValue, ntimestampValue)
            };

            var context = new TypeConverterContext(TypeConverterOptionsBuilder.NewBuilder().Build());

            Assert.ThrowsException<ArgumentNullException>(() => converter.ToClr(intValue, null, context));
            Assert.ThrowsException<ArgumentNullException>(() => converter.ToClr(null, typeof(int), context));

            foreach (var info in types)
            {
                IResult<object?> result;

                var ntype = nullableBase.MakeGenericType(info.type);

                // primitive/ion
                result = converter.ToClr(info.dia, info.type, context);
                Assert.IsNotNull(result);
                Assert.AreEqual(info.type, result.Resolve()?.GetType());

                // primitive/null-ion-primitive
                result = converter.ToClr(info.nullDia, info.type, context);
                Assert.IsNull(result.Resolve());

                // nullable-primitive/ion-primitive
                result = converter.ToClr(info.dia, ntype, context);
                Assert.IsNotNull(result.Resolve());
                Assert.AreEqual(info.type, result.Resolve()?.GetType());

                // nullable-primitive/null-ion-primitive
                result = converter.ToClr(info.nullDia, ntype, context);
                Assert.IsNull(result.Resolve());
            }

            // sstring
            var result2 = converter.ToClr(stringValue, typeof(string), context);
            Assert.AreEqual(stringValue.As<StringValue>().Value, result2.Resolve());

            result2 = converter.ToClr(nstringValue, typeof(string), context);
            Assert.IsNull(result2.Resolve());
        }


        [TestMethod]
        public void ToDia_Tests()
        {
            (System.Type type, System.Type ntype, DiaType diaType, object value)[] args = new[]
            {
                (typeof(byte), typeof(byte?), DiaType.Int, (object)(byte)5),
                (typeof(sbyte), typeof(sbyte?), DiaType.Int, (object)(sbyte)5),
                (typeof(short), typeof(short?), DiaType.Int, (object)5),
                (typeof(short), typeof(short?), DiaType.Int, (object)5),
                (typeof(ushort), typeof(ushort?), DiaType.Int, (object)5),
                (typeof(int), typeof(int?), DiaType.Int, (object)5),
                (typeof(uint), typeof(uint?), DiaType.Int, (object)5u),
                (typeof(long), typeof(long?), DiaType.Int, (object)5L),
                (typeof(ulong), typeof(ulong?), DiaType.Int, (object)5ul),
                (typeof(BigInteger), typeof(BigInteger?), DiaType.Int, (object)new BigInteger(5)),
                (typeof(Half), typeof(Half?), DiaType.Decimal, (object)(Half)5.0),
                (typeof(float), typeof(float?), DiaType.Decimal, (object)5.0),
                (typeof(double), typeof(double?), DiaType.Decimal, (object)5.0),
                (typeof(decimal), typeof(decimal?), DiaType.Decimal, (object)5.0m),
                (typeof(BigDecimal), typeof(BigDecimal?), DiaType.Decimal, (object)new BigDecimal(5.0m)),
                (typeof(bool), typeof(bool?), DiaType.Bool, (object)false),
                (typeof(DateTime), typeof(DateTime?), DiaType.Instant, (object)DateTime.Now),
                (typeof(DateTimeOffset), typeof(DateTimeOffset?), DiaType.Instant, (object)DateTimeOffset.Now)
            };

            var options = new TypeConverterContext(TypeConverterOptionsBuilder.NewBuilder().Build());

            Assert.ThrowsException<ArgumentNullException>(() => converter.ToDia(null, null, options));

            foreach (var info in args)
            {
                // clr-type/value
                var result = converter.ToDia(info.type, info.value, options);
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

                // nullable-clr-type/value
                result = converter.ToDia(info.ntype, info.value, options);
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
                result = converter.ToDia(info.type, null, options);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Resolve().Type, info.diaType);
                Assert.IsTrue(result.Resolve().IsNull);

                // nullable-clr-type/null
                result = converter.ToDia(info.ntype, null, options);
                Assert.IsNotNull(result);
                Assert.AreEqual(result.Resolve().Type, info.diaType);
                Assert.IsTrue(result.Resolve().IsNull);

                // clr-type/invalid-value
                Assert.ThrowsException<ArgumentException>(() => converter.ToDia(info.type, new object(), options));

                // nullable-clr-type/invalid-value
                Assert.ThrowsException<ArgumentException>(() => converter.ToDia(info.ntype, new object(), options).Resolve());
            }
        }
    }
}
