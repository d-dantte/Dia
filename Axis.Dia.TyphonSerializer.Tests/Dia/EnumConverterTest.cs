using Axis.Dia.Typhon.Dia;
using Axis.Luna.Extensions;

namespace Axis.Dia.Typhon.Tests.Dia
{
    [TestClass]
    public class EnumConverterTest
    {
        [TestMethod]
        public void CanConvert_Tests()
        {
            var converter = new EnumConverter();
            var tinfo = typeof(TestEnum).ToTypeInfo();
            var tinfo2 = typeof(object).ToTypeInfo();

            var result = converter.CanConvert(tinfo);
            Assert.IsTrue(result);

            result = converter.CanConvert(tinfo2);
            Assert.IsFalse(result);

            Assert.ThrowsException<ArgumentException>(
                () => converter.CanConvert(default));
        }

        [TestMethod]
        public void ToDia_Tests()
        {
            var converter = new EnumConverter();
            var tinfo1 = typeof(TestEnum).ToTypeInfo();
            var tinfo2 = typeof(TestEnum2).ToTypeInfo();
            var tinfo3 = typeof(object).ToTypeInfo();

            Assert.ThrowsException<ArgumentException>(
                () => converter.ToDia(default, null, default!));
            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(tinfo3, null, default!));
            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(tinfo1, "", default!));
            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(tinfo1, TestEnum2.First, default!));

            var value = converter.ToDia(tinfo1, TestEnum.Fourth, default!);
            Assert.AreEqual(Core.DiaType.Symbol, value.Type);
            Assert.AreEqual("Fourth", value.As<Core.Types.Symbol>().Value);
        }

        #region nested types
        public enum TestEnum
        {
            First, Second, Third, Fourth
        }
        public enum TestEnum2
        {
            First, Second, Third, Fourth
        }
        #endregion
    }
}
