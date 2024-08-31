using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Convert.Axon;
using Axis.Dia.Types;
using System.Text;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class BlobParserTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var optionsBuilder = SerializerOptionsBuilder.NewBuilder();
            var options = optionsBuilder.Build();

            var value = BlobValue.Null();
            var result = BlobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.blob", text);

            #region Singleline
            options = optionsBuilder
                .WithBlobOptions(new SerializerOptions.BlobOptions
                {
                    LineStyle = SerializerOptions.TextLineStyle.Singleline
                })
                .Build();
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
            options = optionsBuilder
                .WithIndentationStyle(SerializerOptions.IndentationStyles.Spaces)
                .WithBlobOptions(new SerializerOptions.BlobOptions
                {
                    LineStyle = SerializerOptions.TextLineStyle.Multiline,
                    MaxLineLength = 50
                })
                .Build();
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
            var options = SerializerOptionsBuilder.NewBuilder().Build();
            var value = BlobValue.Null("ann1", "ann2");
            var textResult = BlobParser.Serialize(value, new SerializerContext(options));
            var valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            var valueInstance = valueResult.Resolve();
            Assert.IsNotNull(value);
            Assert.AreEqual(value, valueInstance);


            value = BlobValue.Of(Bytes);
            textResult = BlobParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            valueInstance = valueResult.Resolve();
            Assert.IsNotNull(value);
            Assert.AreEqual(value, valueInstance);
        }

        [TestMethod]
        public void WrapLinesTest()
        {
            var optionsBuilder = SerializerOptionsBuilder.NewBuilder();
            var options = optionsBuilder
                .WithIndentationStyle(SerializerOptions.IndentationStyles.Spaces)
                .Build();
            var slines = new[]
            {
                "first line",
                "second line",
                "third line"
            };

            var lines = BlobParser.WrapLines(slines, new SerializerContext(options));
            Assert.AreEqual("<\r\n    first line\r\n    second line\r\n    third line\r\n>", lines);


            lines = BlobParser.WrapLines(slines, new SerializerContext(options).Indent());

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
