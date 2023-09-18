using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type;
using Axis.Dia.Convert.Type.Converters;
using Axis.Dia.Convert.Type.Exceptions;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Type.Converters
{
    [TestClass]
    public class EnumConverterTests
    {
        private static readonly EnumTypeConverter converter = new EnumTypeConverter();

        [TestMethod]
        public void CanConvertToDia_Tests()
        {
            var canConvert = converter.CanConvert(typeof(SampleEnum));
            Assert.IsTrue(canConvert);

            canConvert = converter.CanConvert(typeof(object));
            Assert.IsFalse(canConvert);
            canConvert = converter.CanConvert(typeof(Array));
            Assert.IsFalse(canConvert);

            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(null));
        }

        [TestMethod]
        public void CanConvertToClr_Tests()
        {
            var canConvert = converter.CanConvert(DiaType.Symbol, typeof(SampleEnum));
            Assert.IsTrue(canConvert);

            canConvert = converter.CanConvert(DiaType.Symbol, typeof(object));
            Assert.IsFalse(canConvert);

            foreach (var diaType in Enum.GetValues<DiaType>().Where(t => !DiaType.Symbol.Equals(t)))
            {
                canConvert = converter.CanConvert(diaType, typeof(SampleEnum));
                Assert.IsFalse(canConvert);
            }

            Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(DiaType.Symbol, null));
        }

        [TestMethod]
        public void ToClr_Tests()
        {
            var options = Dia.Convert.Type.Clr.ConverterOptionsBuilder.NewBuilder().Build();

            Assert.ThrowsException<ArgumentNullException>(() => converter.ToClr(SymbolValue.Of("Three"), null,
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(object)))));
            Assert.ThrowsException<ArgumentNullException>(() => converter.ToClr(null, typeof(SampleEnum),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(SampleEnum)))));
            Assert.ThrowsException<ArgumentException>(() => converter.ToClr(
                SymbolValue.Of("non identifier symbol"),
                typeof(SampleEnum),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(SampleEnum)))));

            var result = converter.ToClr(BoolValue.Null(), typeof(SampleEnum),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(SampleEnum))));
            Assert.IsTrue(result.IsErrorResult());
            var errorResult = result.AsError();
            Assert.IsTrue(errorResult.Cause().InnerException is IncompatibleClrConversionException);

            result = converter.ToClr(SymbolValue.Of("Stuff"), typeof(object),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(object))));
            Assert.IsTrue(result.IsErrorResult());
            errorResult = result.AsError();
            Assert.IsTrue(errorResult.Cause().InnerException is IncompatibleClrConversionException);


            result = converter.ToClr(SymbolValue.Of("Instant"), typeof(DiaType),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(DiaType))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(DiaType.Instant, result.Resolve());


            result = converter.ToClr(SymbolValue.Of("Two"), typeof(SampleEnum),
                new Dia.Convert.Type.Clr.ConverterContext(
                    options,
                    new ObjectPath(typeof(SampleEnum))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(SampleEnum.Two, result.Resolve());
        }

        [TestMethod]
        public void ToDia_Tests()
        {
            var options = Dia.Convert.Type.Dia.ConverterOptionsBuilder.NewBuilder().Build();

            Assert.ThrowsException<ArgumentNullException>(() => converter.ToDia(null, false,
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(object)))));
            Assert.ThrowsException<ArgumentException>(() => converter.ToDia(typeof(bool), 43,
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(bool)))));

            var result = converter.ToDia(typeof(SampleEnum), 43,
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(SampleEnum))));
            Assert.IsTrue(result.IsErrorResult());
            var errorResult = result.AsError();
            Assert.IsTrue(errorResult.Cause().InnerException is TypeMismatchException);

            result = converter.ToDia(typeof(DiaType), DiaType.Decimal,
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(DiaType))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(SymbolValue.Of("Decimal"), result.Resolve());

            result = converter.ToDia(typeof(SampleEnum), SampleEnum.Four,
                new Dia.Convert.Type.Dia.ConverterContext(
                    options,
                    new ObjectPath(typeof(SampleEnum))));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(SymbolValue.Of("Four"), result.Resolve());
        }

        public enum SampleEnum
        {
            One,
            Two,
            Three,
            Four,
            Five
        }
    }
}
