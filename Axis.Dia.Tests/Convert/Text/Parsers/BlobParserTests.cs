using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Convert.Text;
using Axis.Dia.Types;
using System.Text;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Text.Parsers
{
    [TestClass]
    public class BlobParserTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var options = new TextSerializerOptions();

            var value = BlobValue.Null();
            var result = BlobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.blob", text);

            #region Singleline
            options.Blobs.LineStyle = TextSerializerOptions.TextLineStyle.Singleline;
            value = BlobValue.Of(Bytes);
            result = BlobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"< {System.Convert.ToBase64String(Bytes)} >", text);

            value = BlobValue.Of(Bytes, "fgh", "fghjk");
            result = BlobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"fgh::fghjk::< {System.Convert.ToBase64String(Bytes)} >", text);
            #endregion

            #region multiline
            options.Blobs.LineStyle = TextSerializerOptions.TextLineStyle.Multiline;
            options.IndentationStyle = TextSerializerOptions.IndentationStyles.Spaces;
            options.Blobs.MaxLineLength = 50;
            value = BlobValue.Of(Bytes);
            result = BlobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);

            value = BlobValue.Of(Bytes, "fgh", "fghjk");
            result = BlobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            #endregion
        }

        [TestMethod]
        public void ParseTest()
        {
            var options = new TextSerializerOptions();
            var value = BlobValue.Null("ann1", "ann2");
            var textResult = BlobParser.Serialize(value, new SerializerContext(options));
            var valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            var valueInstance = valueResult.Resolve();
            Assert.IsNotNull(value);
            Assert.AreEqual(value, valueInstance);


            value = BlobValue.Of(Bytes);
            textResult = BlobParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => TextSerializer.ParseValue(txt, new ParserContext()));
            valueInstance = valueResult.Resolve();
            Assert.IsNotNull(value);
            Assert.AreEqual(value, valueInstance);
        }

        [TestMethod]
        public void WrapLinesTest()
        {
            var options = new TextSerializerOptions();
            options.IndentationStyle = TextSerializerOptions.IndentationStyles.Spaces;
            var slines = new[]
            {
                "first line",
                "second line",
                "third line"
            };

            var lines = BlobParser.WrapLines(slines, new SerializerContext(options));
            Assert.AreEqual("<\r\n    first line\r\n    second line\r\n    third line\r\n>", lines);


            lines = BlobParser.WrapLines(slines, new SerializerContext(options).IndentContext());

            Assert.AreEqual("<\r\n        first line\r\n        second line\r\n        third line\r\n>", lines);
        }

        private static readonly string Text = @"abcd
efgh
ijkl
public static void Stuff() {
    if(true) {
        Console.WriteLine(""blare witch project""); 
    }
}";

        private static readonly byte[] Bytes = Encoding.Unicode.GetBytes(Text);
    }
}
