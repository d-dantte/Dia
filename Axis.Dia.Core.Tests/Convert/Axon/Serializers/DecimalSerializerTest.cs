using Axis.Dia.Core.Convert.Axon;
using Axis.Dia.Core.Convert.Axon.Serializers;
using Axis.Luna.Extensions;

namespace Axis.Dia.Core.Tests.Convert.Axon.Serializers
{
    [TestClass]
    public class DecimalSerializerTest
    {
        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => DecimalSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_Decimal()
        {
            var attributes = Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Types.Decimal.Of(0.000231m);
            var withAttributes = Types.Decimal.Of(
                43232.0000000765008m,
                attributes);
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithDecimalCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = DecimalSerializer.Serialize(noAttributes, context);
            Assert.AreEqual("2.31E-4", text);
            text = DecimalSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Decimal} 2.31E-4'", text);

            // Attribute
            text = DecimalSerializer.Serialize(withAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::4.32320000000765008E4", text);
            text = DecimalSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Decimal} 4.32320000000765008E4'", text);

            // Null
            text = DecimalSerializer.Serialize(default, context);
            Assert.AreEqual("*.decimal", text);
            text = DecimalSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.decimal", text);
        }
    }
}
