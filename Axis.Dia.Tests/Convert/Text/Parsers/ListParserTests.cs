using Axis.Dia.Convert.Text;
using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using System.Text;

namespace Axis.Dia.Tests.Convert.Text.Parsers
{
    [TestClass]
    public class ListParserTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var options = new TextSerializerOptions();

            var value = ListValue.Null();
            var result = ListParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.list", text);


            value = new ListValue("ann1", "ann2")
            {
                2,3,4,5
            };
            result = ListParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::[2, 3, 4, 5]", text);


            options.Lists.UseMultipleLines = true;
            options.IndentationStyle = TextSerializerOptions.IndentationStyles.Spaces;
            result = ListParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual(@"ann1::ann2::[
    2,
    3,
    4,
    5
]", text);
        }

        [TestMethod]
        public void ParseTests()
        {
            var options = new TextSerializerOptions();

            var value = ListValue.Null("ann1", "ann2");
            var textResult = ListParser.Serialize(value, new SerializerContext(options));
            var valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            var result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = new ListValue("ann1", "ann2")
            {
                43,
                true,
                new ListValue
                {
                    "meh",
                     45.65m,
                    DateTimeOffset.Now,
                    Encoding.UTF8.GetBytes("some random string"),
                    new ListValue("bleh ink")
                    {
                        ClobValue.Null("annotationX"),
                        SymbolValue.Of("something")
                    }
                }
            };
            textResult = ListParser.Serialize(value, new SerializerContext(options));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            options.Lists.UseMultipleLines = true;
            options.IndentationStyle = TextSerializerOptions.IndentationStyles.Spaces;
            textResult = ListParser.Serialize(value, new SerializerContext(options));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
        }
    }
}
