﻿using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class IntegerSerializerTest
    {
        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => IntegerSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_IntegerRegular()
        {
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.Integer.Of(12345);
            var withAttributes = Core.Types.Integer.Of(
                12345,
                attributes);
            var context = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Decimal)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Decimal)
                .WithIntegerCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = IntegerSerializer.Serialize(noAttributes, context);
            Assert.AreEqual("12345", text);
            text = IntegerSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 12345'", text);

            // Attribute
            text = IntegerSerializer.Serialize(withAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::12345", text);
            text = IntegerSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Int} 12345'", text);

            // Null
            text = IntegerSerializer.Serialize(default, context);
            Assert.AreEqual("*.int", text);
            text = IntegerSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.int", text);

            // zero
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), context);
            Assert.AreEqual("0", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0'", text);

            // negative
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-12345), context);
            Assert.AreEqual("-12345", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-12345), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} -12345'", text);
        }

        [TestMethod]
        public void Serialize_IntegerRegular_WithSeparator()
        {
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.Integer.Of(12345);
            var withAttributes = Core.Types.Integer.Of(
                12345,
                attributes);
            var context = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Decimal)
                .WithIntegerDigitSeparator(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Decimal)
                .WithIntegerDigitSeparator(true)
                .WithIntegerCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = IntegerSerializer.Serialize(noAttributes, context);
            Assert.AreEqual("12_345", text);
            text = IntegerSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 12_345'", text);

            // Attribute
            text = IntegerSerializer.Serialize(withAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::12_345", text);
            text = IntegerSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Int} 12_345'", text);

            // Null
            text = IntegerSerializer.Serialize(default, context);
            Assert.AreEqual("*.int", text);
            text = IntegerSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.int", text);

            // zero
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), context);
            Assert.AreEqual("0", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0'", text);

            // negative
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-12345), context);
            Assert.AreEqual("-12_345", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-12345), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} -12_345'", text);
        }

        [TestMethod]
        public void Serialize_IntegerHex()
        {
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.Integer.Of(12345);
            var withAttributes = Core.Types.Integer.Of(
                12345,
                attributes);
            var context = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Hex)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Hex)
                .WithIntegerCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = IntegerSerializer.Serialize(noAttributes, context);
            Assert.AreEqual("0x3039", text);
            text = IntegerSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0x3039'", text);

            // Attribute
            text = IntegerSerializer.Serialize(withAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::0x3039", text);
            text = IntegerSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Int} 0x3039'", text);

            // Null
            text = IntegerSerializer.Serialize(default, context);
            Assert.AreEqual("*.int", text);
            text = IntegerSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.int", text);

            // zero
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), context);
            Assert.AreEqual("0x0", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0x0'", text);

            // negative
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-7654321), context);
            Assert.AreEqual("0x8B344F", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-7654321), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0x8B344F'", text);
        }

        [TestMethod]
        public void Serialize_IntegerHex_WithLineSeparator()
        {
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.Integer.Of(12345);
            var withAttributes = Core.Types.Integer.Of(
                12345,
                attributes);
            var context = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Hex)
                .WithIntegerDigitSeparator(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Hex)
                .WithIntegerDigitSeparator(true)
                .WithIntegerCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = IntegerSerializer.Serialize(noAttributes, context);
            Assert.AreEqual("0x30_39", text);
            text = IntegerSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0x30_39'", text);

            // Attribute
            text = IntegerSerializer.Serialize(withAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::0x30_39", text);
            text = IntegerSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Int} 0x30_39'", text);

            // Null
            text = IntegerSerializer.Serialize(default, context);
            Assert.AreEqual("*.int", text);
            text = IntegerSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.int", text);

            // zero
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), context);
            Assert.AreEqual("0x0", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0x0'", text);

            // negative
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-7654321), context);
            Assert.AreEqual("0x8B_34_4F", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-7654321), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0x8B_34_4F'", text);
        }

        [TestMethod]
        public void Serialize_IntegerBinary()
        {
            var attributes = Core.Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var noAttributes = Core.Types.Integer.Of(12345);
            var withAttributes = Core.Types.Integer.Of(
                12345,
                attributes);
            var context = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Binary)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithIntegerStyle(Options.IntegerStyle.Binary)
                .WithIntegerCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            // No attribute
            var text = IntegerSerializer.Serialize(noAttributes, context);
            Assert.AreEqual("0b11_0000_0011_1001", text);
            text = IntegerSerializer.Serialize(noAttributes, canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0b11_0000_0011_1001'", text);

            // Attribute
            text = IntegerSerializer.Serialize(withAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::0b11_0000_0011_1001", text);
            text = IntegerSerializer.Serialize(withAttributes, canonicalContext);
            Assert.AreEqual($"'@stuff;@other:more-stuff;@last;'::'#{DiaType.Int} 0b11_0000_0011_1001'", text);

            // Null
            text = IntegerSerializer.Serialize(default, context);
            Assert.AreEqual("*.int", text);
            text = IntegerSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual("*.int", text);

            // zero
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), context);
            Assert.AreEqual("0b0", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(0), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0b0'", text);

            // negative
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-7654321), context);
            Assert.AreEqual("0b1000_1011_0011_0100_0100_1111", text);
            text = IntegerSerializer.Serialize(Core.Types.Integer.Of(-7654321), canonicalContext);
            Assert.AreEqual($"'#{DiaType.Int} 0b1000_1011_0011_0100_0100_1111'", text);
        }
    }
}
