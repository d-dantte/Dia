﻿using Axis.Dia.Convert.Axon;
using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class StringParserTests
    {
        [TestMethod]
        public void SerializeTests()
        {
            var builder = SerializerOptionsBuilder.NewBuilder();

            #region multiline
            builder.WithStringOptions(new SerializerOptions.StringOptions
            {
                LineStyle = SerializerOptions.TextLineStyle.Multiline,
                MaxLineLength = 90,
                AlignmentIndentation = 5
            });

            var value = StringValue.Of(Text);
            var result = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual(MLResultText, text);

            value = StringValue.Of(Text, "fgh", "fghjk");
            result = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"fgh::fghjk::{MLResultText}", text);

            value = StringValue.Null();
            result = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.string", text);
            #endregion

            #region singleline
            builder.WithStringOptions(new SerializerOptions.StringOptions
            {
                LineStyle = SerializerOptions.TextLineStyle.Singleline
            });

            value = StringValue.Of(Text);
            result = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual(SLResultText, text[1..^1]);

            value = StringValue.Of(Text, "fgh", "fghjk");
            result = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"fgh::fghjk::\"{SLResultText}\"", text);

            value = StringValue.Null();
            result = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.string", text);
            #endregion
        }

        [TestMethod]
        public void FormatAsMultilineTests()
        {
            var options = SerializerOptionsBuilder.NewBuilder().Build();
            options.Strings.AlignmentIndentation = 8;
            options.Strings.IsTextAligned = true;
            options.Strings.LineStyle = SerializerOptions.TextLineStyle.Multiline;
            options.Strings.MaxLineLength = 50;

            var formatted = StringParser.FormatAsMultiline(Text, options.Strings);
            Console.WriteLine(formatted.JoinUsing("\n"));
        }

        [TestMethod]
        public void DeserializeTests()
        {
            var builder = SerializerOptionsBuilder.NewBuilder();

            #region Singleline
            builder.WithStringOptions(new SerializerOptions.StringOptions { LineStyle = SerializerOptions.TextLineStyle.Singleline });
            var value = StringValue.Null("ann1", "ann2");
            var textResult = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            var valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            var result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = StringValue.Of("The lame pink unicorn flew over enraged pigs", "ann1");
            textResult = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = StringValue.Of("The lame pink unicorn flew over \tenraged pigs", "ann1");
            textResult = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
            #endregion

            #region Multiline
            builder.WithStringOptions(new SerializerOptions.StringOptions { LineStyle = SerializerOptions.TextLineStyle.Multiline });
            value = StringValue.Null("ann1", "ann2");
            textResult = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = StringValue.Of("The lame pink unicorn flew over enraged pigs", "ann1");
            textResult = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            value = StringValue.Of("The lame pink unicorn flew over \tenraged pigs", "ann1");
            textResult = StringParser.Serialize(value, new SerializerContext(builder.Build()));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            result = valueResult.Resolve();
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
            #endregion
        }


        [TestMethod]
        public void StringReaderTest()
        {
            var reader = new StringParser.WordGroupReader(Text);
            var results = new List<IResult<StringParser.WordGroup>>();
            while (reader.TryNextWordGroup(out var wordGroup))
                results.Add(wordGroup);

            results.Fold().Consume(groups => groups.ForAll(x => Console.WriteLine(x)));
        }

        private static readonly string Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's "
            +"standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.\n\n  It has "
            +"survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. \n\n It was popularised in the 1960s"
            +" with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

        private static readonly string MLResultText = "@\"\\'5\n     Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum \\\n     has been the industry's standard dummy text ever since the 1500s, when an unknown \\\n     printer took a galley of type and scrambled it to make a type specimen book.\\^nn\n      It has survived not only five centuries, but also the leap into electronic typesetting, \\\n     remaining essentially unchanged. \\^n\n     It was popularised in the 1960s with the release of Letraset sheets containing Lorem \\\n     Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker \\\n     including versions of Lorem Ipsum.\\\n     \"";

        private static readonly string SLResultText = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.\\n\\n  It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. \\n\\n It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

    }
}
