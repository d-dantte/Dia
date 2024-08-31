using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Tests.Serializers
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
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var blob = Core.Types.Boolean.Of(true);
            var blobWithAttributes = Core.Types.Boolean.Of(false, attributes);
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
            Assert.AreEqual($"'#{Core.DiaType.Bool} true'", text);

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
