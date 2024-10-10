using Axis.Dia.Core.Types;
using Axis.Dia.Typhon.Dia;
using Axis.Luna.Extensions;

namespace Axis.Dia.Typhon.Tests.Dia
{
    [TestClass]
    public class SequenceConverterTests
    {
        [TestMethod]
        public void CanConvert_Tests()
        {
            var converter = new SequenceConverter();

            var typeinfo = typeof(int[]).ToTypeInfo();
            var result = converter.CanConvert(typeinfo);
            Assert.IsTrue(result);

            typeinfo = typeof(List<int>).ToTypeInfo();
            result = converter.CanConvert(typeinfo);
            Assert.IsTrue(result);

            typeinfo = typeof(Guid).ToTypeInfo();
            result = converter.CanConvert(typeinfo);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ToDia_Tests()
        {
            var converter = new SequenceConverter();
            var typeInfo = typeof(bool).ToTypeInfo();
            var context = new ConverterContext(Options.NewBuilder().Build());

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, context));

            typeInfo = typeof(int[]).ToTypeInfo();
            var dia = converter.ToDia(typeInfo, null, context);
            Assert.IsNotNull(dia);
            Assert.IsTrue(dia.Is(out Sequence seq));
            Assert.IsTrue(seq.IsNull);

            object instance = new int[] { 1, 2, 3, 4, 5 };
            dia = converter.ToDia(typeInfo, instance, context);
            Assert.IsNotNull(dia);
            Assert.IsTrue(dia.Is(out seq));
            Assert.AreEqual(5, seq.Count);
            CollectionAssert.AreEqual(
                instance.As<int[]>().ToList(),
                seq.Select(i => (int)i.AsInteger().Value!.Value).ToList());
        }
    }
}
