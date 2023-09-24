using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Convert.Axon;
using Axis.Dia.Types;
using System.Text;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class RecordParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var builder = SerializerOptionsBuilder
                .NewBuilder()
                .WithIndentationStyle(SerializerOptions.IndentationStyles.Spaces);

            var value = RecordValue.Null();
            var result = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
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
            result = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::{ abcd: 43, efgh: true, ijkl: { '1234': \"meh\" } }", text);

            builder.WithRecordOptions(new SerializerOptions.RecordOptions { UseQuotedIdentifierPropertyNames = true });
            result = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::{ \"abcd\": 43, \"efgh\": true, \"ijkl\": { \"1234\": \"meh\" } }", text);


            builder.WithRecordOptions(new SerializerOptions.RecordOptions
            {
                UseQuotedIdentifierPropertyNames = false,
                UseMultipleLines = true
            });
            result = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("ann1::ann2::{\r\n    abcd: 43,\r\n    efgh: true,\r\n    ijkl: {\r\n        '1234': \"meh\"\r\n    }\r\n}", text);
        }

        [TestMethod]
        public void ParseTests()
        {
            var builder = SerializerOptionsBuilder.NewBuilder();

            var value = RecordValue.Null("ann1", "ann2");
            var textResult = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
            var valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
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
            textResult = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            builder
                .WithIndentationStyle(SerializerOptions.IndentationStyles.Spaces)
                .WithRecordOptions(new SerializerOptions.RecordOptions
                {
                    UseQuotedIdentifierPropertyNames = true,
                    UseMultipleLines = true
                });
            textResult = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            builder.WithRecordOptions(new SerializerOptions.RecordOptions { UseQuotedIdentifierPropertyNames = false });
            textResult = RecordParser.Serialize(value, new SerializerContext(builder.Build()));
            textResult.Consume(Console.WriteLine);
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
        }
    }
}
