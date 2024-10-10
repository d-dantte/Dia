using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class StringSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid context: default")]
        public void Serialize_InvalidContext_ThrowsException()
        {
            StringSerializer.Serialize(Core.Types.String.Of(""), default);
        }

        [TestMethod]
        public void Serialize_NullValue_ReturnsNullTypeText()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.String.Null();

            var result = StringSerializer.Serialize(value, context);

            Assert.AreEqual("null.string", result);
        }

        [TestMethod]
        public void Serialize_InlineStyle_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.String.Of("test");

            var result = StringSerializer.Serialize(value, context);

            Assert.AreEqual("\"test\"", result);
        }

        [TestMethod]
        public void Serialize_VerbatimStyle_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .WithStringStyle(Options.StringStyle.Verbatim)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.String.Of("test");

            var result = StringSerializer.Serialize(value, context);

            Assert.AreEqual("`test`", result);
        }

        [TestMethod]
        public void Serialize_WithAttributes_ReturnsSerializedStringWithAttributes()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Core.Types.String.Of(
                "test",
                "flag");

            var result = StringSerializer.Serialize(value, context);

            Assert.AreEqual("@flag; \"test\"", result);
        }

        [TestMethod]
        public void SerializeVerbatim_WithLineThreshold_ReturnsBrokenLines()
        {
            var options = Options
                .Builder()
                .WithStringLineThreshold(20)
                .WithStringStyle(Options.StringStyle.Verbatim)
                .Build();
            var context = SerializerContext.Of(options);
            var value = "This is a long string that needs to be broken into multiple lines.";

            var result = StringSerializer.Serialize(value, context);

            var expected = $"\\{Environment.NewLine}This is a long strin\\{Environment.NewLine}g that needs to be b\\{Environment.NewLine}roken into multiple \\{Environment.NewLine}lines.";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SerializInline_WithLineThreshold_ReturnsBrokenLines()
        {
            var options = Options
                .Builder()
                .WithStringLineThreshold(20)
                .WithStringStyle(Options.StringStyle.Inline)
                .Build();
            var context = SerializerContext.Of(options);
            var value = "This is a long string that needs to be broken into multiple lines.";

            var result = StringSerializer.Serialize(value, context);

            var expected = $"\"This is a long strin\"{Environment.NewLine}+ \"g that needs to be b\"{Environment.NewLine}+ \"roken into multiple \"{Environment.NewLine}+ \"lines.\"";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void BreakVerbatimLines_LongString_ReturnsBrokenLines()
        {
            var value = "This is a long string that needs to be broken into multiple lines.";
            var result = StringSerializer.BreakVerbatimLines(value, 20).ToList();

            var expected = new List<string>
            {
                "This is a long strin",
                "g that needs to be b",
                "roken into multiple ",
                "lines."
            };

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SanitizeVerbatim_EscapedNewLine_RemovesEscapedNewLine()
        {
            var value = "This is a long string\\\n";
            var result = StringSerializer.SanitizeVerbatim(value);

            var expected = "This is a long string";
            Assert.AreEqual(expected, result);
        }
    }
}
