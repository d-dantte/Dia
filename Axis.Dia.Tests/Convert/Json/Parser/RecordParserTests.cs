using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class RecordParserTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => RecordParser
                .Serialize(null, new()));

            Assert.ThrowsException<ArgumentException>(() => RecordParser
                .Serialize(new(), default));

            var record = RecordValue.Null();
            var result = RecordParser.Serialize(record, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("\"[$Record;]null\"", result.Resolve());

            record = RecordValue.Null("me", "you");
            result = RecordParser.Serialize(record, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("\"[$Record;me;you;]null\"", result.Resolve());

            record = RecordValue.Of();
            result = RecordParser.Serialize(record, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("{}", result.Resolve());

            record = new RecordValue
            {
                ["price"] = 45.48m,
                ["type"] = SymbolValue.Of("jared", "hopeless")
            };
            result = RecordParser.Serialize(record, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(@"{
    ""price"": 45.48,
    ""type"": ""[$Symbol;hopeless;]jared""
}", result.Resolve());


            record = new RecordValue
            {
                ["price"] = 45.48m,
                ["type"] = SymbolValue.Of("jared", "hopeless"),
                [SymbolValue.Of("inner", "outer")] = RecordValue.Of(),
                [SymbolValue.Of("inner2", "outer2")] = new RecordValue
                {
                    ["age"] = "old enough"
                }
            };
            record["self"] = ReferenceValue.Of(record);

            var optionBuilder = SerializerOptionsBuilder
                .NewBuilder()
                .WithRecordOptions(true);
            var context = new SerializerContext(optionBuilder.Build());
            ReferenceUtil.LinkReferences(record, out var linkedRefs);
            context.BuildAddressIndices(linkedRefs);
            result = RecordParser.Serialize(record, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(@"{
    ""0$"": ""[#0x1;]"",
    ""price"": 45.48,
    ""type"": ""[$Symbol;hopeless;]jared"",
    ""[outer;]inner"": {},
    ""[outer2;]inner2"": {
        ""age"": ""old enough""
    },
    ""self"": ""[$Ref;]0x1""
}", result.Resolve());


            optionBuilder = SerializerOptionsBuilder
                .NewBuilder()
                .WithRecordOptions(false);
            context = new SerializerContext(optionBuilder.Build());
            ReferenceUtil.LinkReferences(record, out linkedRefs);
            context.BuildAddressIndices(linkedRefs);
            result = RecordParser.Serialize(record, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(
                "{ \"0$\": \"[#0x1;]\", \"price\": 45.48, \"type\": \"[$Symbol;hopeless;]jared\", \"[outer;]inner\": {}, \"[outer2;]inner2\": { \"age\": \"old enough\" }, \"self\": \"[$Ref;]0x1\" }",
                result.Resolve());
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => RecordParser
                .Parse(null, new()));

            Assert.ThrowsException<ArgumentException>(() => RecordParser
                .Parse(CSTNode.Of("a", "b"), default));

            Assert.ThrowsException<ArgumentException>(() => RecordParser
                .Parse(CSTNode.Of("a", "b"), new()));

            var recordText = "{}";
            var cst = ParserUtil.ParseTokens("object", recordText);
            var result = RecordParser.Parse(cst, new());
            Assert.IsTrue(result.IsDataResult());
            var record = result.Resolve();
            Assert.AreEqual(0, record.Count);


            recordText = "{\"abcd\": \"efgh\"}";
            cst = ParserUtil.ParseTokens("object", recordText);
            result = RecordParser.Parse(cst, new());
            Assert.IsTrue(result.IsDataResult());
            record = result.Resolve();
            Assert.AreEqual(1, record.Count);
            Assert.AreEqual("abcd", record.Keys![0].Value!);


            recordText = "{" +
                "\"abcd\": \"efgh\"" +
            "}";
            cst = ParserUtil.ParseTokens("object", recordText);
            result = RecordParser.Parse(cst, new());
            Assert.IsTrue(result.IsDataResult());
            record = result.Resolve();
            Assert.AreEqual(1, record.Count);
            Assert.AreEqual("abcd", record.Keys![0].Value!);

            recordText = @"{
    ""0$"": ""[#0x1;]"",
    ""price"": 45.48,
    ""type"": ""[$Symbol;hopeless;]jared"",
    ""[outer;]inner"": {},
    ""[outer2;]inner2"": {
        ""age"": ""old enough"",
        ""sports"": null,
        ""auctions"": ""[$Symbol;Me3;]priority""
    },
    ""self"": ""[$Ref;]0x1""
}";
            cst = ParserUtil.ParseTokens("object", recordText);
            result = RecordParser.Parse(cst, new());
            Assert.IsTrue(result.IsDataResult());
            record = result.Resolve();
            Assert.AreEqual(5, record.Count);

        }
    }
}
