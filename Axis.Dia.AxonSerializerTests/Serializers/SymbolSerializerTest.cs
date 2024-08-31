using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class SymbolSerializerTest
    {

        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => SymbolSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_IdentifierSymbol()
        {
            var symbol = "the_quick_brown_fox_jumps_over_the_lazy_dog";
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.Symbol.Of(symbol);
            var withAttributes = Core.Types.Symbol.Of(
                symbol,
                attributes);
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithSymbolCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContextMl = Options
                .Builder()
                .WithSymbolCanonicalForm(true)
                .WithSymbolLineThreshold(20)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = SymbolSerializer.Serialize(noAttributes, context);
            Assert.AreEqual($"{symbol}", text);
            text = SymbolSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Symbol} {symbol}'", text);
            text = SymbolSerializer.Serialize(noAttributes, canonicalContextMl);
            Assert.AreEqual($"'#Symbol the_quick_brown_fox_'\r\n+ 'jumps_over_the_lazy_'\r\n+ 'dog'", text);

            // Attribute
            text = SymbolSerializer.Serialize(withAttributes, context);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::{symbol}", text);
            text = SymbolSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Symbol} {symbol}'", text);
            text = SymbolSerializer.Serialize(withAttributes, canonicalContextMl);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#Symbol the_quick_brown_fox_'\r\n+ 'jumps_over_the_lazy_'\r\n+ 'dog'", text);

            // Null
            text = SymbolSerializer.Serialize(default, context);
            Assert.AreEqual("*.symbol", text);
            text = SymbolSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.symbol", text);
        }

        [TestMethod]
        public void Serialize_Symbol()
        {
            var symbol = "the quick brown\u5cc1 fox jumps\n over the lazy\v\a dog";
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.Symbol.Of(symbol);
            var withAttributes = Core.Types.Symbol.Of(
                symbol,
                attributes);
            var context = Options
                .Builder()
                .WithSymbolLineThreshold(20)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithSymbolLineThreshold(20)
                .WithSymbolCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var expected = "'#Symbol the quick brown峁 fox'\r\n+ ' jumps\\n over the laz'\r\n+ 'y\\v\\a dog'";
            var canonicalExpected = "'#Symbol the quick brown峁 fox'\r\n+ ' jumps\\n over the laz'\r\n+ 'y\\v\\a dog'";
            var text = SymbolSerializer.Serialize(noAttributes, context);
            Assert.AreEqual($"{expected}", text);
            text = SymbolSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"{canonicalExpected}", text);

            // Attribute
            text = SymbolSerializer.Serialize(withAttributes, context);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::{expected}", text);
            text = SymbolSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::{canonicalExpected}", text);

            // Null
            text = SymbolSerializer.Serialize(default, context);
            Assert.AreEqual("*.symbol", text);
            text = SymbolSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.symbol", text);
        }
    }
}
