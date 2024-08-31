using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core;
using Axis.Dia.Core.Utils;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class StringSerializerTest
    {
        [TestMethod]
        public void EscapeLineBreak_ShouldEscapeProperly()
        {
            var line = "something";
            var result = StringSerializer.EscapeLinebreak(line);
            Assert.AreEqual($"{line}\\{Environment.NewLine}", result);

            line = "something\n";
            result = StringSerializer.EscapeLinebreak(line);
            Assert.AreEqual(line, result);

            line = "something\r";
            result = StringSerializer.EscapeLinebreak(line);
            Assert.AreEqual(line, result);

            line = "something\r\n";
            result = StringSerializer.EscapeLinebreak(line);
            Assert.AreEqual(line, result);

            line = null!;
            result = StringSerializer.EscapeLinebreak(line);
            Assert.AreEqual(line, result);
        }

        [TestMethod]
        public void BreakVerbatimLines_ShouldBreak()
        {
            string line = null!;
            var result = StringSerializer.BreakVerbatimLines(line, 10);
            Assert.AreEqual(0, result.Count());

            line = "0123456789";
            result = StringSerializer.BreakVerbatimLines(line, 10);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(line, result.First());

            line = "01234567890";
            result = StringSerializer.BreakVerbatimLines(line, 10);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(line[..10], result.First());
            Assert.AreEqual(line[10..], result.Last());

            line = "01234\n567890";
            result = StringSerializer.BreakVerbatimLines(line, 10);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(line[..6], result.First());
            Assert.AreEqual(line[6..], result.Last());

            line = "01234\r567890";
            result = StringSerializer.BreakVerbatimLines(line, 10);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(line[..6], result.First());
            Assert.AreEqual(line[6..], result.Last());

            line = "01234\r\n567890";
            result = StringSerializer.BreakVerbatimLines(line, 10);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(line[..7], result.First());
            Assert.AreEqual(line[7..], result.Last());
        }

        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => StringSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_SinglelineString()
        {
            var @string = "the quick brown fox jumps over the lazy dog";
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.String.Of(@string);
            var withAttributes = Core.Types.String.Of(
                @string,
                attributes);
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithStringCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = StringSerializer.Serialize(noAttributes, context);
            Assert.AreEqual($"\"{@string}\"", text);
            text = StringSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.String} {@string}'", text);

            // Attribute
            text = StringSerializer.Serialize(withAttributes, context);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::\"{@string}\"", text);
            text = StringSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.String} {@string}'", text);

            // Null
            text = StringSerializer.Serialize(default, context);
            Assert.AreEqual("*.string", text);
            text = StringSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.string", text);
        }

        [TestMethod]
        public void Serialize_MultilineString()
        {
            var @string = "the quick brown fox jumps over the lazy dog";
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.String.Of(@string);
            var withAttributes = Core.Types.String.Of(
                @string,
                attributes);
            var context = Options
                .Builder()
                .WithStringLineThreshold(20)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithStringLineThreshold(20)
                .WithStringCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var expected = "the quick brown fox \"\r\n+ \"jumps over the lazy \"\r\n+ \"dog";
            var canonicalExpected = "'#String the quick brown fox '\r\n+ 'jumps over the lazy '\r\n+ 'dog'";
            var text = StringSerializer.Serialize(noAttributes, context);
            Assert.AreEqual($"\"{expected}\"", text);
            text = StringSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"{canonicalExpected}", text);

            // Attribute
            text = StringSerializer.Serialize(withAttributes, context);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::\"{expected}\"", text);
            text = StringSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::{canonicalExpected}", text);

            // Null
            text = StringSerializer.Serialize(default, context);
            Assert.AreEqual("*.string", text);
            text = StringSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.string", text);
        }
    }
}
