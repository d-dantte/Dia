using Axis.Dia.Contracts;
using Axis.Dia.Convert.Json;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Common.Utils;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Tests.Convert.Json.Parser
{
    [TestClass]
    public class EncodedValueParserTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => EncodedValueParser
                .Serialize(null, new SerializerContext()));

            Assert.ThrowsException<ArgumentException>(() => EncodedValueParser
                .Serialize(IntValue.Null(), default));

            // null
            Enum.GetValues<DiaType>()
                .Where(type => !DiaType.Annotation.Equals(type))
                .Where(type => !DiaType.Ref.Equals(type))
                .ForAll(type =>
                {
                    var result = EncodedValueParser.Serialize(IDiaValue.NullOf(type), new SerializerContext());
                    Assert.IsTrue(result.IsDataResult());
                    Assert.AreEqual($"\"[${type};]null\"", result.Resolve());
                });

            // values
            var array = ArrayUtil.Of<(IDiaValue Value, bool UseAddressIndex, Func<string, string> Result)>(
                (BoolValue.Of(true), true, index => $"[#0x{index};$Bool;]true"),
                (IntValue.Of(23, "ann"), false, _ => "[$Int;ann;]23"),
                (DecimalValue.Of(-0.00451m), false, _ => "[$Decimal;]-0.00451"),
                (InstantValue.Of(
                    new DateTimeOffset(2020, 01, 01, 00, 00, 00, TimeSpan.Zero), "schedule"),
                    true,
                    index => $"[#0x{index};$Instant;schedule;]2020-01-01T00:00:00.0000000+00:00"),
                (SymbolValue.Of("Killograms or Grams", "scale"), false, _ => "[$Symbol;scale;]Killograms or Grams"),
                (StringValue.Of("the quick brown fox, etc..."), false, _ => "[$String;]the quick brown fox, etc..."),
                (ClobValue.Of(
                    "the quick brown fox, etc..."),
                    true,
                    index => $"[#0x{index};$Clob;]dABoAGUAIABxAHUAaQBjAGsAIABiAHIAbwB3AG4AIABmAG8AeAAsACAAZQB0AGMALgAuAC4A"),
                (BlobValue.Of(new byte[] { 0, 1, 2 }), false, _ => "[$Blob;]AAEC"),
                (ReferenceValue.Of(IntValue.Of(4)), false, _ => "[$Ref;]0x1"));

            array.ForAll(testArgs =>
            {
                var context = new SerializerContext();
                if (testArgs.UseAddressIndex)
                {
                    var @ref = ReferenceValue.Of((testArgs.Value as IDiaAddressProvider)!);
                    context.BuildAddressIndices(ArrayUtil.Of<IDiaReference>(@ref));
                    var result = EncodedValueParser.Serialize(testArgs.Value, context);

                    Assert.IsTrue(result.IsDataResult());
                    Assert.IsTrue(context.TryGetAddressIndex((testArgs.Value as IDiaAddressProvider)!, out int index));
                    Assert.AreEqual(testArgs.Result(index.ToString("x")).WrapIn("\""), result.Resolve());
                }
                else
                {
                    if (testArgs.Value is IDiaReference r)
                        context.BuildAddressIndices(ArrayUtil.Of(r));

                    var result = EncodedValueParser.Serialize(testArgs.Value, context);

                    Assert.IsTrue(result.IsDataResult());
                    Assert.AreEqual(testArgs.Result("").WrapIn("\""), result.Resolve());
                }
            });
        }

        [TestMethod]
        public void Parse_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => EncodedValueParser
                .Parse(null, new ParserContext()));

            Assert.ThrowsException<ArgumentException>(() => EncodedValueParser
                .Parse(CSTNode.Of("a", "b"), default));

            var result = EncodedValueParser.Parse(CSTNode.Of("a", "b"), new());
            Assert.IsTrue(result.IsErrorResult());

            var dt = new DateTimeOffset(2020, 01, 01, 00, 00, 00, TimeSpan.Zero);
            var testArgs = ArrayUtil.Of<(string Tokens, IDiaValue? ExpectedResult)>(
                ("[#0x8c;$Bool;]true", BoolValue.Of(true)),
                ("[$Int;ann;]23", IntValue.Of(23, "ann")),
                ("[$Decimal;]-0.00451", DecimalValue.Of(-0.00451m)),
                ("[#0x1c2b;$Instant;schedule;]2020-01-01T00:00:00.0000000+00:00", InstantValue.Of(dt, "schedule")),
                ("[$Symbol;scale;]Killograms or Grams", SymbolValue.Of("Killograms or Grams", "scale")),
                ("[$String;]the quick brown fox, etc...", StringValue.Of("the quick brown fox, etc...")),
                ("[#0x11;$Clob;]dABoAGUAIABxAHUAaQBjAGsAIABiAHIAbwB3AG4AIABmAG8AeAAsACAAZQB0AGMALgAuAC4A",
                 ClobValue.Of("the quick brown fox, etc...")),
                ("[$Blob;]AAEC", BlobValue.Of(new byte[] { 0, 1, 2 })),
                ("[$Instant;]-0.00451", null));

            testArgs.ForAll(args =>
            {
                var context = new ParserContext();
                var cst = ParserUtil.ParseTokens("encoded-value", args.Tokens.WrapIn("\""));
                var result = EncodedValueParser.Parse(cst, context);

                if (args.ExpectedResult is not null)
                {
                    Assert.IsTrue(result.IsDataResult());
                    Assert.AreEqual(args.ExpectedResult, result.Resolve());
                }
                else
                {
                    Assert.IsTrue(result.IsErrorResult());
                }
            });

            var refText = "[$Ref;]0x12";
            var xcst = ParserUtil.ParseTokens("encoded-value", refText.WrapIn("\""));
            var context = new ParserContext();
            var xresult = EncodedValueParser.Parse(xcst, context);
            Assert.IsTrue(xresult.IsDataResult());
            Assert.AreEqual(DiaType.Ref, xresult.Resolve().Type);
        }

        [TestMethod]
        public void Grammar_Tests()
        {
            var result = ParserUtil.ParseTokens("encoded-value", "[ #0x4f;$Int; ann1;ann2;'abc:xyz';]54".WrapIn("\""));
            Assert.IsInstanceOfType<CSTNode>(result);

            result = ParserUtil.ParseTokens("encoded-value", "[$Int; ann1;] 54".WrapIn("\""));
            Assert.IsInstanceOfType<CSTNode>(result);

            result = ParserUtil.ParseTokens("encoded-value", "[$Int;]54".WrapIn("\""));
            Assert.IsInstanceOfType<CSTNode>(result);
        }
    }
}
