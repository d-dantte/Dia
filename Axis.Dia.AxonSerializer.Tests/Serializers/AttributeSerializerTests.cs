using Axis.Dia.Axon.Serializers;
using Axis.Dia.Axon;
using Axis.Dia.Core.Types;

namespace Axis.Dia.AxonSerializer.Tests.Serializers
{
    [TestClass]
    public class AttributeSerializerTests
    {
        [TestMethod]
        public void Serialize_AttributesAreDefault_ThrowsArgumentException()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);

            Assert.ThrowsException<ArgumentException>(
                () => AttributeSerializer.Serialize(default, context));
        }

        [TestMethod]
        public void Serialize_ContextIsDefault_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => AttributeSerializer.Serialize(AttributeSet.Of([]), default));
        }

        [TestMethod]
        public void Serialize_AttributesAreEmpty_ReturnsEmptyString()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);

            var result = AttributeSerializer.Serialize(AttributeSet.Of([]), context);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Serialize_MultipleAttributes_NoIndentation_ReturnsSerializedAttributes()
        {
            var attributeSet = AttributeSet.Of(
                ("key", "value"),
                "flag1",
                ("key2", "value2"));
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);

            var result = AttributeSerializer.Serialize(attributeSet, context);
            Assert.AreEqual("@flag1; @key:value; @key2:value2;", result);
        }

        [TestMethod]
        public void Serialize_MultipleAttributes_WithIndentation_ReturnsSerializedAttributes()
        {
            var attributeSet = AttributeSet.Of(
                ("key", "value"),
                "flag1",
                ("key2", "value2"));
            var options = Options
                .Builder()
                .WithAttributeOptions(2)
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .Build();
            var context = SerializerContext.Of(options);

            var result = AttributeSerializer.Serialize(attributeSet, context);
            Assert.AreEqual("@flag1; @key:value;\r\n@key2:value2;", result);
        }

        [TestMethod]
        public void Serialize_InvalidSingleAttribute_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(
                () => AttributeSerializer.Serialize(default));
        }

        [TestMethod]
        public void Serialize_SingleAttribute_ReturnsSerializedAttribute()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);

            var attribute = Core.Types.Attribute.Of("key", "value");
            var result = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@key:value;", result);

            attribute = Core.Types.Attribute.Of("flag");
            result = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@flag;", result);
        }

        [TestMethod]
        public void EscapeAttributeValue_SpecialCharacters_EscapesCorrectly()
        {
            var input = ":;'\\";
            var expectedOutput = "\\:\\;\\'\\\\";

            var result = AttributeSerializer.EscapeAttributeValue(input);

            Assert.AreEqual(expectedOutput, result);
        }
    }
}
