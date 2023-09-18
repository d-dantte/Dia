using Axis.Dia.Convert.Text;
using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Text.Parsers
{
    [TestClass]
    public class BoolParserTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            var options = new TextSerializerOptions();

            options.Bools.ValueCase = TextSerializerOptions.Case.Uppercase;
            var text = BoolParser.Serialize(BoolValue.Null("ann1", "ann2"), new SerializerContext(options));
            Assert.AreEqual("ann1::ann2::null.bool", text.Resolve());

            text = BoolParser.Serialize(BoolValue.Of(true, "ann1"), new SerializerContext(options));
            Assert.AreEqual("ann1::TRUE", text.Resolve());

            text = BoolParser.Serialize(BoolValue.Of(false), new SerializerContext(options));
            Assert.AreEqual("FALSE", text.Resolve());


            options.Bools.ValueCase = TextSerializerOptions.Case.Lowercase;
            text = BoolParser.Serialize(BoolValue.Null("ann1", "ann2"), new SerializerContext(options));
            Assert.AreEqual("ann1::ann2::null.bool", text.Resolve());

            text = BoolParser.Serialize(BoolValue.Of(true, "ann1"), new SerializerContext(options));
            Assert.AreEqual("ann1::true", text.Resolve());

            text = BoolParser.Serialize(BoolValue.Of(false), new SerializerContext(options));
            Assert.AreEqual("false", text.Resolve());


            options.Bools.ValueCase = TextSerializerOptions.Case.Titlecase;
            text = BoolParser.Serialize(BoolValue.Null("ann1", "ann2"), new SerializerContext(options));
            Assert.AreEqual("ann1::ann2::null.bool", text.Resolve());

            text = BoolParser.Serialize(BoolValue.Of(true, "ann1"), new SerializerContext(options));
            Assert.AreEqual("ann1::True", text.Resolve());

            text = BoolParser.Serialize(BoolValue.Of(false), new SerializerContext(options));
            Assert.AreEqual("False", text.Resolve());
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var options = new TextSerializerOptions();

            options.Bools.ValueCase = TextSerializerOptions.Case.Uppercase;
            var value = BoolValue.Null("ann1", "ann2");
            var textResult = BoolParser.Serialize(value, new SerializerContext(options));
            var valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            var result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = BoolValue.Of(true, "ann1");
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = BoolValue.Of(false);
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);


            options.Bools.ValueCase = TextSerializerOptions.Case.Lowercase;
            value = BoolValue.Null("ann1", "ann2");
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = BoolValue.Of(true, "ann1");
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = BoolValue.Of(false);
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);


            options.Bools.ValueCase = TextSerializerOptions.Case.Titlecase;
            value = BoolValue.Null("ann1", "ann2");
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = BoolValue.Of(true, "ann1");
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = BoolValue.Of(false);
            textResult = BoolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
        }
    }
}
