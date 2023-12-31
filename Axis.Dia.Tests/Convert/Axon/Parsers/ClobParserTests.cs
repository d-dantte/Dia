﻿using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Convert.Axon;
using Axis.Dia.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Axis.Luna.Common.Results;
using Axis.Luna.Common;

namespace Axis.Dia.Tests.Convert.Axon.Parsers
{
    [TestClass]
    public class ClobParserTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var options = SerializerOptionsBuilder.NewBuilder().Build();

            var value = ClobValue.Null();
            var result = ClobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            var text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual("null.clob", text);

            value = ClobValue.Of(Text);
            result = ClobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"<<\\{Environment.NewLine}{Text}\\{Environment.NewLine}>>", text);

            value = ClobValue.Of(Text, "fgh", "fghjk");
            result = ClobParser.Serialize(value, new SerializerContext(options));
            Assert.IsTrue(result.IsDataResult());
            text = result.Resolve();
            Console.WriteLine(text);
            Assert.AreEqual($"fgh::fghjk::<<\\{Environment.NewLine}{Text}\\{Environment.NewLine}>>", text);
        }

        [TestMethod]
        public void ParseTest()
        {
            var options = SerializerOptionsBuilder.NewBuilder().Build();
            var value = ClobValue.Null("ann1", "ann2");
            var textResult = ClobParser.Serialize(value, new SerializerContext(options));
            var valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            var valueInstance = valueResult.Resolve();
            Assert.IsNotNull(value);
            Assert.AreEqual(value, valueInstance);


            value = ClobValue.Of(Text);
            textResult = ClobParser.Serialize(value, new SerializerContext(options));
            valueResult = textResult.Bind(txt => AxonSerializer.ParseValue(txt));
            valueInstance = valueResult.Resolve();
            Assert.IsNotNull(value);
            Assert.AreEqual(value, valueInstance);
        }

        private static readonly string Text = @"abcd
efgh
ijkl
public static void Stuff() {
    if(true) {
        Console.WriteLine(""blare witch project""); 
    }
}";
    }
}
