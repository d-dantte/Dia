using Axis.Dia.Json.Deserializers;
using Axis.Luna.Extensions;

namespace Axis.Dia.Json.Tests.Deserializers
{
    [TestClass]
    public class AttributeParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryParseAttributes_ShouldThrowOnNullReader()
        {
            AttributeParser.TryParseAttributes(null!, out _);
        }

        [TestMethod]
        public void TryParseAttributes_ShouldParseAttributesSuccessfully()
        {
            var reader = new CharSequenceReader("@key1:value1; @key2:value2;");
            var success = AttributeParser.TryParseAttributes(reader, out var attributes);

            Assert.IsTrue(success);
            Assert.IsNotNull(attributes);
            Assert.AreEqual(2, attributes.Length);
            Assert.AreEqual("key1", attributes[0].Key);
            Assert.AreEqual("value1", attributes[0].Value);
            Assert.AreEqual("key2", attributes[1].Key);
            Assert.AreEqual("value2", attributes[1].Value);
        }

        [TestMethod]
        public void TryParseAttributes_ShouldReturnFalseOnInvalidInput()
        {
            var reader = new CharSequenceReader("@key1:value1; invalid");
            var success = AttributeParser.TryParseAttributes(reader, out var attributes);

            Assert.IsFalse(success);
            Assert.IsNull(attributes);
            Assert.AreEqual(0, reader.CurrentIndex); // Reset index on failure
        }

        [TestMethod]
        public void TryParseAttributes_ShouldHandleEmptyInput()
        {
            var reader = new CharSequenceReader("");
            var success = AttributeParser.TryParseAttributes(reader, out var attributes);

            Assert.IsTrue(success);
            Assert.IsTrue(attributes.IsEmpty());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryParseLineSpace_ShouldThrowOnNullReader()
        {
            AttributeParser.TryParseLineSpace(null!, out _);
        }

        [TestMethod]
        public void TryParseLineSpace_ShouldParseWhiteSpace()
        {
            var reader = new CharSequenceReader("   ");
            var success = AttributeParser.TryParseLineSpace(reader, out var lineSpace);

            Assert.IsTrue(success);
            Assert.IsFalse(lineSpace.IsDefault);
            Assert.AreEqual("   ", lineSpace.ToString());
        }

        [TestMethod]
        public void TryParseLineSpace_ShouldReturnFalseIfNoWhiteSpace()
        {
            var reader = new CharSequenceReader("abc");
            var success = AttributeParser.TryParseLineSpace(reader, out var lineSpace);

            Assert.IsFalse(success);
            Assert.IsTrue(lineSpace.IsDefault);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryParseAttribute_ShouldThrowOnNullReader()
        {
            AttributeParser.TryParseAttribute(null!, out _);
        }

        [TestMethod]
        public void TryParseAttribute_ShouldParseAttributeSuccessfully()
        {
            var reader = new CharSequenceReader("@key:value;");
            var success = AttributeParser.TryParseAttribute(reader, out var attribute);

            Assert.IsTrue(success);
            Assert.IsNotNull(attribute);
            Assert.AreEqual("key", attribute!.Value.Key);
            Assert.AreEqual("value", attribute.Value!.Value);
        }

        [TestMethod]
        public void TryParseAttribute_ShouldParseAttributeWithNoValue()
        {
            var reader = new CharSequenceReader("@key;");
            var success = AttributeParser.TryParseAttribute(reader, out var attribute);

            Assert.IsTrue(success);
            Assert.IsNotNull(attribute);
            Assert.AreEqual("key", attribute!.Value.Key);
            Assert.IsNull(attribute.Value!.Value);
        }

        [TestMethod]
        public void TryParseAttribute_ShouldReturnFalseOnInvalidAttribute()
        {
            var reader = new CharSequenceReader("invalid");
            var success = AttributeParser.TryParseAttribute(reader, out var attribute);

            Assert.IsFalse(success);
            Assert.IsNull(attribute);
            Assert.AreEqual(0, reader.CurrentIndex); // Reset index on failure
        }

        [TestMethod]
        public void TryParseAttribute_ShouldReturnFalseOnMissingDelim()
        {
            var reader = new CharSequenceReader("@keyvalue");
            var success = AttributeParser.TryParseAttribute(reader, out var attribute);

            Assert.IsFalse(success);
            Assert.IsNull(attribute);
            Assert.AreEqual(0, reader.CurrentIndex); // Reset index on failure
        }

        [TestMethod]
        public void TryParseAttribute_ShouldReturnFalseOnMissingSemicolon()
        {
            var reader = new CharSequenceReader("@key:value");
            var success = AttributeParser.TryParseAttribute(reader, out var attribute);

            Assert.IsFalse(success);
            Assert.IsNull(attribute);
            Assert.AreEqual(0, reader.CurrentIndex); // Reset index on failure
        }

        [TestMethod]
        public void TryParseAttribute_ShouldHandleEscapedSemicolonInValue()
        {
            var reader = new CharSequenceReader("@key:val\\;ue;");
            var success = AttributeParser.TryParseAttribute(reader, out var attribute);

            Assert.IsTrue(success);
            Assert.IsNotNull(attribute);
            Assert.AreEqual("key", attribute!.Value.Key);
            Assert.AreEqual("val;ue", attribute.Value.Value);
        }

        [TestMethod]
        public void Fail_ShouldResetReaderIndexAndReturnFalse()
        {
            var reader = new CharSequenceReader("@key:value;");
            reader.Advance(5); // Move index forward

            var result = AttributeParser.TryParseAttribute(reader, out _);
            Assert.IsFalse(result);
            Assert.AreEqual(5, reader.CurrentIndex); // Index should reset on failure
        }
    }
}
