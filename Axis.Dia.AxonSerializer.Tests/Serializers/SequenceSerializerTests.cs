using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core.Types;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class SequenceSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialize_ContextIsDefault_ThrowsArgumentException()
        {
            SequenceSerializer.Serialize(Sequence.Empty(), default);
        }

        [TestMethod]
        public void Serialize_ValueIsNull_ReturnsNullTypeText()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);

            var result = SequenceSerializer.Serialize(Sequence.Null(), context);

            Assert.AreEqual("null.sequence", result);
        }

        [TestMethod]
        public void Serialize_ValueIsEmpty_ReturnsEmptyArray()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Sequence.Empty();

            var result = SequenceSerializer.Serialize(value, context);

            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public void Serialize_ValueHasAttributes_ReturnsSerializedWithAttributes()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Sequence.Empty("flag", ("stuff", "ffuts"));
            var result = SequenceSerializer.Serialize(value, context);

            Assert.AreEqual("@flag; @stuff:ffuts; []", result);
        }

        [TestMethod]
        public void Serialize_ValueIsNotEmpty_ReturnsSerializedItems()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Sequence.Of("string", true, 88.5m);
            var result = SequenceSerializer.Serialize(value, context);

            var expected = "#0; [\"string\", true, 8.85E1]";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Serialize_ValueIsNotEmpty_Multiline_ReturnsSerializedItems()
        {
            var options = Options
                .Builder()
                .WithSequenceUseMultiline(true)
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Sequence.Of("string", true, 88.5m);
            var result = SequenceSerializer.Serialize(value, context);

            var expected = "#0; [\r\n    \"string\",\r\n    true,\r\n    8.85E1\r\n]";
            Assert.AreEqual(expected, result);
        }
    }
}
