using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class ListParser_Tests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ListParser
                .Serialize(null, new()));

            Assert.ThrowsException<ArgumentException>(() => ListParser
                .Serialize(new(), default));

            var list = ListValue.Null();
            var result = ListParser.Serialize(list, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("\"[$List;]null\"", result.Resolve());

            list = ListValue.Null("me", "you");
            result = ListParser.Serialize(list, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("\"[$List;me;you;]null\"", result.Resolve());

            list = ListValue.Of();
            result = ListParser.Serialize(list, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("[]", result.Resolve());

            list = new ListValue
            {
                45.48m,
                SymbolValue.Of("jared", "hopeless")
            };
            result = ListParser.Serialize(list, new());
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(@"[
    45.48,
    ""[$Symbol;hopeless;]jared""
]", result.Resolve());

            var optionBuilder = SerializerOptionsBuilder
                .NewBuilder()
                .WithListOptions(false);
            result = ListParser.Serialize(list, new(optionBuilder.Build()));
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(@"[ 45.48, ""[$Symbol;hopeless;]jared"" ]", result.Resolve());


            list = new ListValue
            {
                45.48m,
                SymbolValue.Of("jared", "hopeless"),
                new ListValue
                {
                    "Meltigemini"
                }
            };
            list.Add(ReferenceValue.Of(list));

            var context = new SerializerContext(optionBuilder.WithListOptions(true).Build());
            list.LinkReferences();
            context.BuildAddressIndices(list);

            result = ListParser.Serialize(list, context);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual(@"[
    ""0$[#0x1;]"",
    45.48,
    ""[$Symbol;hopeless;]jared"",
    [
        ""Meltigemini""
    ],
    ""[$Ref;]0x1""
]", result.Resolve());
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ListParser
                .Parse(null, new()));

            Assert.ThrowsException<ArgumentException>(() => ListParser
                .Parse(CSTNode.Of("a", "b"), default));

            Assert.ThrowsException<ArgumentException>(() => ListParser
                .Parse(CSTNode.Of("a", "b"), new()));

            var listText = @"[
    ""0$[#0x1;]"",
    45.48,
    ""[$Symbol;hopeless;]jared"",
    [
        ""Meltigemini""
    ],
    ""[$Ref;]0x1""
]";

            var cst = ParserUtil.ParseTokens("array", listText);
            var result = ListParser.Parse(cst, new());
            Assert.IsTrue(result.IsDataResult());

            listText = @"[""0$[#0x1;]"", 45.48, ""[$Symbol;hopeless;]jared"", [""Meltigemini""], ""[$Ref;]0x1""]";
            cst = ParserUtil.ParseTokens("array", listText);
            result = ListParser.Parse(cst, new());
            Assert.IsTrue(result.IsDataResult());
        }


        [TestMethod]
        public void Grammar_Tests()
        {
            var result = GrammarUtil.Grammar.GetRecognizer("array").Recognize("[]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("array").Recognize("[\"0$[#0xabc;]\"]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("array").Recognize("[45, \"abcd\", true]");
            Assert.IsTrue(result is SuccessResult);


            result = GrammarUtil.Grammar.GetRecognizer("array").Recognize("[\"0$[#0xabc;]\", 45, \"abcd\", true]");
            Assert.IsTrue(result is SuccessResult);

        }
    }
}
