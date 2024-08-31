using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core.Types;
using Axis.Dia.Core.Utils;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class RecordSerializerTest
    {
        private static readonly string TimeZonePrecision = "yyyy-MM-dd HH:mm:ss.fffffff zzz";

        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => RecordSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_Record()
        {
            var now = DateTimeOffset.Now;
            var tsText = now.ToString(TimeZonePrecision);
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var multilineContext = Options
                .Builder()
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .WithRecordUseMultiline(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var multilineAlwaysQuoteContext = Options
                .Builder()
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .WithRecordUseMultiline(true)
                .WithRecordAlwaysQuotePropertyName(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var seq = new Core.Types.Record
            {
                ["the bool"] = true,
                ["data"] = Array.Empty<byte>(),
                ["count"] = 56,
                [Record.PropertyName.Of("weight", "killograms")] = 544.2m,
                ["created"] = now,
                ["name"] = "something stringy",
                ["tags"] = Core.Types.Symbol.Of("a symbol"),
                ["scopes"] = new Core.Types.Record
                {
                    ["first"] = true,
                    ["second"] = new Sequence
                    {
                        false
                    }
                }
            };

            // Null
            var text = RecordSerializer.Serialize(default, context);
            Assert.AreEqual("*.record", text);

            // Empty
            text = RecordSerializer.Serialize(new Record(), context);
            Assert.AreEqual("{}", text);

            // Seq
            var expected = $"{{\"the bool\": true, data: <>, count: 56, '@killograms;'::weight: 5.442E2, created: '#Timestamp {tsText}', name: \"something stringy\", tags: '#Symbol a symbol', scopes: {{first: true, second: [false]}}}}";
            text = RecordSerializer.Serialize(seq, context);
            Assert.AreEqual(expected, text);

            // Multiline Seq
            expected = $"{{\r\n    \"the bool\": true,\r\n    data: <>,\r\n    count: 56,\r\n    '@killograms;'::weight: 5.442E2,\r\n    created: '#Timestamp {tsText}',\r\n    name: \"something stringy\",\r\n    tags: '#Symbol a symbol',\r\n    scopes: {{\r\n        first: true,\r\n        second: [false]\r\n    }}\r\n}}";
            text = RecordSerializer.Serialize(seq, multilineContext);
            Assert.AreEqual(expected, text);

            // Multiline always quote Seq
            expected = $"{{\r\n    \"the bool\": true,\r\n    \"data\": <>,\r\n    \"count\": 56,\r\n    '@killograms;'::\"weight\": 5.442E2,\r\n    \"created\": '#Timestamp {tsText}',\r\n    \"name\": \"something stringy\",\r\n    \"tags\": '#Symbol a symbol',\r\n    \"scopes\": {{\r\n        \"first\": true,\r\n        \"second\": [false]\r\n    }}\r\n}}";
            text = RecordSerializer.Serialize(seq, multilineAlwaysQuoteContext);
            Assert.AreEqual(expected, text);
        }
    }
}
