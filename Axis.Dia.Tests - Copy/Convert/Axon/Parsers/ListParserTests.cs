using Axis.Dia.Convert.Axon;
using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using System.Text;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class ListParserTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var optionsBuilder = SerializerOptionsBuilder.NewBuilder();

            var value = ListValue.Null();
            var result = ListParser.Serialize(value, new SerializerContext(optionsBuilder.Build()));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.list", text);


            value = new ListValue("ann1", "ann2")
            {
                2,3,4,5
            };
            result = ListParser.Serialize(value, new SerializerContext(optionsBuilder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::[ 2, 3, 4, 5 ]", text);


            optionsBuilder
                .WithIndentationStyle(SerializerOptions.IndentationStyles.Spaces)
                .WithListOptions(new SerializerOptions.ListOptions
                {
                    UseMultipleLines = true
                });
            result = ListParser.Serialize(value, new SerializerContext(optionsBuilder.Build()));
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
            var optionsBuilder = SerializerOptionsBuilder.NewBuilder();

            var value = ListValue.Null("ann1", "ann2");
            var textResult = ListParser.Serialize(value, new SerializerContext(optionsBuilder.Build()));
            var valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
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
            textResult = ListParser.Serialize(value, new SerializerContext(optionsBuilder.Build()));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            optionsBuilder
                .WithIndentationStyle(SerializerOptions.IndentationStyles.Spaces)
                .WithListOptions(new SerializerOptions.ListOptions { UseMultipleLines = true });
            textResult = ListParser.Serialize(value, new SerializerContext(optionsBuilder.Build()));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
        }
    }
}
