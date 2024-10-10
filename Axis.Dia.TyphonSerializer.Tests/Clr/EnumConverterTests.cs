using Axis.Dia.Typhon.Clr;

namespace Axis.Dia.Typhon.Tests.Clr
{
    [TestClass]
    public class EnumConverterTests
    {
        [TestMethod]
        public void CanConvert_Tests()
        {
            var converter = new EnumConverter();

            Assert.ThrowsException<ArgumentException>(
                () => converter.CanConvert(Core.DiaType.Symbol, default));

            Assert.IsTrue(converter.CanConvert(Core.DiaType.Symbol, typeof(Enum1).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Int, typeof(Enum1).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Symbol, typeof(EnumConverterTests).ToTypeInfo()));
        }

        [TestMethod]
        public void ToClr_Tests()
        {
            var converter = new EnumConverter();

            Assert.ThrowsException<ArgumentNullException>(
                () => converter.ToClr(null!, typeof(Enum1).ToTypeInfo(), null!));
            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToClr(Core.Types.Symbol.Null(), typeof(object).ToTypeInfo(), null!));

            var result = converter.ToClr(
                Core.Types.Symbol.Null(),
                typeof(Enum1).ToTypeInfo(),
                null!);
            Assert.IsNull(result);

            Assert.ThrowsException<FormatException>(() => converter.ToClr(
                Core.Types.Symbol.Of("abc_xyz"),
                typeof(Enum1).ToTypeInfo(),
                null!));

            result = converter.ToClr(
                Core.Types.Symbol.Of("First"),
                typeof(Enum1).ToTypeInfo(),
                null!);
            Assert.AreEqual(Enum1.First, result);
        }

        #region Nested Types
        public enum Enum1 { First, Second, Third }
        #endregion
    }
}
