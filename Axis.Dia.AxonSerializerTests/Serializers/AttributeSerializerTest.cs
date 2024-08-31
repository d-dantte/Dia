using Axis.Dia.Axon.Serializers;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class AttributeSerializerTest
    {
        [TestMethod]
        public void Serialize_SingleAttribute()
        {
            var context = new SerializerContext(Options.Builder().Build());

            var attribute = Core.Types.Attribute.Of("something");
            var text = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@something;", text);

            attribute = Core.Types.Attribute.Of("something", "value");
            text = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@something:value;", text);

            attribute = Core.Types.Attribute.Of("something", "value with space and :; escapable characters \n \uf32c");
            text = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@something:value with space and \\:\\; escapable characters \\n \\uf32c;", text);

            var attributes = Core.Types.Attribute.Of("first", "second", "third");
            text = AttributeSerializer.Serialize(attributes, context);
            Assert.AreEqual("'@first;@second;@third;'", text);

            context = new SerializerContext(Options
                .Builder()
                .WithAttributeOptions(2)
                .Build());

            attributes = Core.Types.Attribute.Of("first", "second", "third");
            text = AttributeSerializer.Serialize(attributes, context);
            Assert.AreEqual("'@first;@second;@third;'", text);

            context = new SerializerContext(Options
                .Builder()
                .WithAttributeOptions(2)
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .Build())
                .Next();

            attributes = Core.Types.Attribute.Of("first", "second", "third");
            text = AttributeSerializer.Serialize(attributes, context);
            Assert.AreEqual("'@first;@second;'\r\n    + '@third;'", text);
        }
    }
}
