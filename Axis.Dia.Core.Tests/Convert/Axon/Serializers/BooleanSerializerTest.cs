using Axis.Dia.Core.Convert.Axon;
using Axis.Dia.Core.Convert.Axon.Serializers;
using Axis.Luna.Extensions;

namespace Axis.Dia.Core.Tests.Convert.Axon.Serializers
{
    [TestClass]
    public class BooleanSerializerTest
    {
        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => BooleanSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_SingleLine()
        {
            var attributes = Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var blob = Types.Boolean.Of(true);
            var blobWithAttributes = Types.Boolean.Of(false, attributes);
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithBoolCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            var text = BooleanSerializer.Serialize(blob, context);
            Assert.AreEqual("true", text);
            text = BooleanSerializer.Serialize(blob, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Bool} true'", text);

            text = BooleanSerializer.Serialize(blobWithAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::false", text);
            text = BooleanSerializer.Serialize(blobWithAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Bool} false'", text);

            text = BooleanSerializer.Serialize(default, context);
            Assert.AreEqual("*.bool", text);
            text = BooleanSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual($"*.bool", text);
        }
    }
}
