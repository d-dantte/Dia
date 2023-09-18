using Axis.Dia.Convert.Axon;
using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class SymbolParserTests
    {
        [TestMethod]
        public  void SerializeTest()
        {
            var options = new SerializerOptions();

            #region Idntifier
            var symbol = "Some_Identifier";
            var value = SymbolValue.Of(symbol);
            var result = SymbolParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual(symbol, text);

            value = SymbolValue.Of(symbol, "fgh", "fghjk");
            result = SymbolParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"fgh::fghjk::{symbol}", text);

            value = SymbolValue.Null();
            result = SymbolParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.symbol", text);
            #endregion

            #region long form symbol
            symbol = "symbol with spaces and...";
            value = SymbolValue.Of(symbol);
            result = SymbolParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual(symbol, text[1..^1]);

            value = SymbolValue.Of(symbol, "fgh", "fghjk");
            result = SymbolParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"fgh::fghjk::'{symbol}'", text);
            #endregion
        }


        [TestMethod]
        public void ParseTest()
        {
            var options = new SerializerOptions();

            #region long form symbol
            var value = SymbolValue.Null("ann1", "ann2");
            var textResult = SymbolParser.Serialize(value, new SerializerContext(options));
            var valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt, new ParserContext()));
            var result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = SymbolValue.Of("The lame pink unicorn flew over enraged pigs", "ann1");
            textResult = SymbolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = SymbolValue.Of("Identifier");
            textResult = Result.Of("'Identifier'");
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = SymbolValue.Of("list", "ann3");
            textResult = SymbolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
            #endregion

            #region identifier symbol
            var symbol = "Some_Identifier";
            value = SymbolValue.Of(symbol);
            textResult = SymbolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = SymbolValue.Of(symbol, "fgh", "fghjk");
            textResult = SymbolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = SymbolValue.Null();
            textResult = SymbolParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt, new ParserContext()));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
            #endregion
        }
    }
}
