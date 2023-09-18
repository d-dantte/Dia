using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Convert.Text;
using Axis.Dia.Types;
using System.Text;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Text.Parsers
{
    [TestClass]
    public class RecordParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var options = new TextSerializerOptions();
            options.IndentationStyle = TextSerializerOptions.IndentationStyles.Spaces;

            var value = RecordValue.Null();
            var result = RecordParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.record", text);


            value = new RecordValue("ann1", "ann2")
            {
                ["abcd"] = 43,
                ["efgh"] = true,
                ["ijkl"] = new RecordValue
                {
                    ["1234"] = "meh"
                }
            };
            result = RecordParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::{abcd:43, efgh:true, ijkl:{'1234':\"meh\"}}", text);

            options.Records.UseQuotedIdentifierPropertyNames = true;
            result = RecordParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::{\"abcd\":43, \"efgh\":true, \"ijkl\":{\"1234\":\"meh\"}}", text);


            options.Records.UseQuotedIdentifierPropertyNames = false;
            options.Records.UseMultipleLines = true;
            result = RecordParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::{\r\n    abcd:43,\r\n    efgh:true,\r\n    ijkl:{\r\n        '1234':\"meh\"\r\n    }\r\n}", text);
        }

        [TestMethod]
        public void ParseTests()
        {
            var options = new TextSerializerOptions();

            var value = RecordValue.Null("ann1", "ann2");
            var textResult = RecordParser.Serialize(value, new SerializerContext(options));
            var valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            var result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = new RecordValue("ann1", "ann2")
            {
                ["abcd"] = 43,
                ["efgh"] = true,
                ["ijkl"] = new RecordValue
                {
                    ["1234"] = "meh",
                    ["meh-yeh"] = 45.65m,
                    ["tel"] = DateTimeOffset.Now,
                    ["three"] = Encoding.UTF8.GetBytes("some random string"),
                    [SymbolValue.Of("list", "ann3")] = new ListValue
                    {
                        ClobValue.Null("annotationX"),
                        SymbolValue.Of("something")
                    }
                }
            };
            textResult = RecordParser.Serialize(value, new SerializerContext(options));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            options.Records.UseQuotedIdentifierPropertyNames = true;
            options.Records.UseMultipleLines = true;
            options.IndentationStyle = TextSerializerOptions.IndentationStyles.Spaces;
            textResult = RecordParser.Serialize(value, new SerializerContext(options));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            options.Records.UseQuotedIdentifierPropertyNames = false;
            textResult = RecordParser.Serialize(value, new SerializerContext(options));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
        }
    }
}
