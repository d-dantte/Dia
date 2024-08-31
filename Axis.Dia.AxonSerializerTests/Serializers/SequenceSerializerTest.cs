using Axis.Dia.Axon.Serializers;
using Axis.Luna.Extensions;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class SequenceSerializerTest
    {
        private static readonly string TimeZonePrecision = "yyyy-MM-dd HH:mm:ss.fffffff zzz";

        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => SequenceSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_Sequence()
        {
            var now = DateTimeOffset.Now;
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var multilineContext = Options
                .Builder()
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .WithSequenceUseMultiline(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var seq = new Core.Types.Sequence
            {
                true,
                Array.Empty<byte>(),
                56,
                544.2m,
                now,
                "something stringy",
                Core.Types.Symbol.Of("a symbol"),
                new Core.Types.Sequence
                {
                    true, false
                }
            };

            // Null
            var text = SequenceSerializer.Serialize(default, context);
            Assert.AreEqual("*.sequence", text);

            // Empty
            text = SequenceSerializer.Serialize(new Core.Types.Sequence(), context);
            Assert.AreEqual("[]", text);

            // Seq
            var expected = $"[true, <>, 56, 5.442E2, '#Timestamp {now.ToString(TimeZonePrecision)}', \"something stringy\", '#Symbol a symbol', [true, false]]";
            text = SequenceSerializer.Serialize(seq, context);
            Assert.AreEqual(expected, text);

            // Multiline Seq
            expected = $"[\r\n    true,\r\n    <>,\r\n    56,\r\n    5.442E2,\r\n    '#Timestamp {now.ToString(TimeZonePrecision)}',\r\n    \"something stringy\",\r\n    '#Symbol a symbol',\r\n    [\r\n        true,\r\n        false\r\n    ]\r\n]";
            text = SequenceSerializer.Serialize(seq, multilineContext);
            Assert.AreEqual(expected, text);
        }
    }
}
