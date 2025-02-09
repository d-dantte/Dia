﻿using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Dia.Axon.Tests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            var rec = new Record(["flag.abc", "flag.xyz"], [])
            {
                ["abcd"] = 5.2e-4m,
                ["selense"] = new Sequence()
                {
                    true, "me"
                },
                ["when"] = DateTimeOffset.Parse("2024-07-25 19:26:41.9781812 +01:00")
            };

            var options = Options
                .Builder()
                .Build();
            var cxt = new SerializerContext(options);
            var text = Serializer.Serialize(rec, cxt);

            var expectedText = "#0; @flag.abc; @flag.xyz;{abcd: 5.2E-4, selense: #1; [true, \"me\"], when: 'Timestamp 2024-07-25 19:26:41.9781812 +01:00'}";
            Assert.AreEqual(expectedText, text);

            rec["self.ref"] = rec;
            text = Serializer.Serialize(rec, new SerializerContext(options));
            expectedText = "#0; @flag.abc; @flag.xyz;{abcd: 5.2E-4, selense: #1; [true, \"me\"], when: 'Timestamp 2024-07-25 19:26:41.9781812 +01:00', self.ref: 'Ref:Record 0x0'}";
            Assert.AreEqual(expectedText, text);

            options = Options
                .Builder()
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .WithRecordUseMultiline(true)
                .WithSequenceUseMultiline(true)
                .Build();
            text = Serializer.Serialize(rec, new SerializerContext(options));
            expectedText = $"#0; @flag.abc; @flag.xyz;{{{Environment.NewLine}    abcd: 5.2E-4,{Environment.NewLine}    selense: #1; [{Environment.NewLine}        true,{Environment.NewLine}        \"me\"{Environment.NewLine}    ],{Environment.NewLine}    when: 'Timestamp 2024-07-25 19:26:41.9781812 +01:00',{Environment.NewLine}    self.ref: 'Ref:Record 0x0'{Environment.NewLine}}}";
            Assert.AreEqual(expectedText, text);
        }

        [TestMethod]
        public void Deserialize_Tests()
        {
            var text = $"#0; @flag.abc; @flag.xyz;{{{Environment.NewLine}    abcd: 5.2E-4,{Environment.NewLine}    selense: #1; [{Environment.NewLine}        true,{Environment.NewLine}        \"me\"{Environment.NewLine}    ],{Environment.NewLine}    when: 'Timestamp 2024-07-25 19:00:00.1234567 +01:00',{Environment.NewLine}    self.ref: 'Ref:Record 0x0'{Environment.NewLine}}}";
            var value = Serializer.Deserialize(text);

            var expected = new Record(["flag.abc", "flag.xyz"], [])
            {
                ["abcd"] = 5.2e-4m,
                ["selense"] = new Sequence()
                {
                    true, "me"
                },
                ["when"] = DateTimeOffset.Parse("2024-07-25 19:0:00.1234567 +01:00")
            };

            expected["self.ref"] = expected;

            Assert.IsTrue(value.Is(out Record rec));
            Assert.IsTrue(expected["abcd"].ValueEquals(rec["abcd"]));
            Assert.IsTrue(expected["selense"].ValueEquals(rec["selense"]));
            Assert.IsTrue(expected["when"].ValueEquals(rec["when"]));
            Assert.IsTrue(expected["self.ref"].ValueEquals(rec["self.ref"]));
        }

        [TestMethod]
        public void Misc()
        {
            var arr1 = new int[] { 1, 2, 3, 4 }.ToImmutableArray();
            var arr2 = new int[] { 1, 2, 3, 4 }.ToImmutableArray();

            Console.WriteLine($"arr1-hash: {arr1.GetHashCode()}");
            Console.WriteLine($"arr2-hash: {arr2.GetHashCode()}");
            Console.WriteLine($"arr1 == arr2: {arr1.Equals(arr2)}");
            Console.WriteLine($"arr1 seq-equal arr2: {arr1.SequenceEqual(arr2)}");
        }
    }
}
