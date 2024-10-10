using Axis.Dia.Core.Types;
using Axis.Dia.Typhon.Dia;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.Typhon.Tests.Dia
{
    [TestClass]
    public class RecordConverterTests
    {
        [TestMethod]
        public void CanConvert_Tests()
        {
            var converter = new RecordConverter();

            var typeinfo = typeof(IDictionary<string, Guid>).ToTypeInfo();
            var result = converter.CanConvert(typeinfo);
            Assert.IsTrue(result);

            typeinfo = typeof(Info).ToTypeInfo();
            result = converter.CanConvert(typeinfo);
            Assert.IsTrue(result);

            typeinfo = typeof(float).ToTypeInfo();
            result = converter.CanConvert(typeinfo);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ToDia_Tests()
        {
            var converter = new RecordConverter();
            var typeInfo = typeof(bool).ToTypeInfo();
            var context = new ConverterContext(Options.NewBuilder().Build());

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToDia(typeInfo, true, context));

            object instance = new Dictionary<string, int>()
            {
                ["first"] = 0,
                ["second"] = -54
            };
            typeInfo = instance.GetType().ToTypeInfo();
            var dia = converter.ToDia(typeInfo, null, context);
            Assert.IsNotNull(dia);
            Assert.IsTrue(dia.Is(out Record rec));
            Assert.IsTrue(rec.IsNull);

            dia = converter.ToDia(typeInfo, instance, context);
            Assert.IsNotNull(dia);
            Assert.IsTrue(dia.Is(out rec));
            Assert.IsFalse(rec.IsNull);
            Assert.AreEqual(2, rec.Count);
            Assert.AreEqual(0, rec["first"].AsInteger().Value!.Value);
            Assert.AreEqual(-54, rec["second"].AsInteger().Value!.Value);

            instance = new Info
            {
                Name = "bleh",
                Description = "bleh description",
                Type = "The type"
            };
            typeInfo = instance.GetType().ToTypeInfo();
            dia = converter.ToDia(typeInfo, instance, context);
            Assert.IsNotNull(dia);
            Assert.IsTrue(dia.Is(out rec));
            Assert.IsFalse(rec.IsNull);
            Assert.AreEqual(4, rec.Count);
            Assert.AreEqual("bleh", rec["Name"].AsString().Value);
            Assert.AreEqual("bleh description", rec["Description"].AsString().Value);
            Assert.AreEqual("The type", rec["Type"].AsString().Value);
            Assert.IsTrue(rec["Ref"].AsRecord().IsNull);

            instance = new Info
            {
                Name = "bluh",
                Description = "bluh description",
                Type = "The type"
            };
            dia = converter.ToDia(typeInfo, instance, context);
            Assert.IsTrue(dia.Is(out rec));
            Assert.AreEqual(4, rec.Count);
        }

        #region Nested 
        public class Info
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Type { get; set; }
            public object? Ref { get; set; }
        }

        #endregion
    }
}
