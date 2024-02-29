using Axis.Dia.Core.Convert.Axon;
using Axis.Dia.Core.Convert.Axon.Serializers;

namespace Axis.Dia.Core.Tests.Convert.Axon.Serializers
{
    [TestClass]
    public class AttributeSerializerTest
    {
        [TestMethod]
        public void Serialize_SingleAttribute()
        {
            var context = new SerializerContext(Options.Builder().Build());

            var attribute = Types.Attribute.Of("something");
            var text = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@something;", text);

            attribute = Types.Attribute.Of("something", "value");
            text = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@something:value;", text);

            attribute = Types.Attribute.Of("something", "value with space and :; escapable characters \n \uf32c");
            text = AttributeSerializer.Serialize(attribute);
            Assert.AreEqual("@something:value with space and \\:\\; escapable characters \\n \\uf32c;", text);

            var attributes = Types.Attribute.Of("first", "second", "third");
            text = AttributeSerializer.Serialize(attributes, context);
            Assert.AreEqual("'@first;@second;@third;'", text);

            context = new SerializerContext(Options
                .Builder()
                .WithAttributeOptions(2)
                .Build());

            attributes = Types.Attribute.Of("first", "second", "third");
            text = AttributeSerializer.Serialize(attributes, context);
            Assert.AreEqual("'@first;@second;@third;'", text);

            context = new SerializerContext(Options
                .Builder()
                .WithAttributeOptions(2)
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .Build())
                .Next();

            attributes = Types.Attribute.Of("first", "second", "third");
            text = AttributeSerializer.Serialize(attributes, context);
            Assert.AreEqual("'@first;@second;'\r\n    + '@third;'", text);
        }
    }
}
