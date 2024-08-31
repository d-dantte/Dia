using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core.Types;

namespace Axis.Dia.AxonSerializer.Tests.Serializers
{
    [TestClass]
    public class SymbolSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Invalid context: default")]
        public void Serialize_InvalidContext_ThrowsException()
        {
            SymbolSerializer.Serialize(Symbol.Null(), default);
        }

        [TestMethod]
        public void Serialize_NullValue_ReturnsNullTypeText()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Symbol.Null();

            var result = SymbolSerializer.Serialize(value, context);

            Assert.AreEqual("null.symbol", result);
        }

        [TestMethod]
        public void Serialize_CanonicalForm_ReturnsSerializedString()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Symbol.Of("test");

            var result = SymbolSerializer.Serialize(value, context);

            Assert.AreEqual("'Symbol test'", result);
        }

        [TestMethod]
        public void Serialize_WithAttributes_ReturnsSerializedStringWithAttributes()
        {
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);
            var value = Symbol.Of("test", "flag");

            var result = SymbolSerializer.Serialize(value, context);

            Assert.AreEqual($"@flag; 'Symbol test'", result);
        }

        [TestMethod]
        public void SerializeCanonicalForm_WithLineThreshold_ReturnsBrokenLines()
        {
            var options = Options
                .Builder()
                .WithSymbolLineThreshold(20)
                .Build();
            var context = SerializerContext.Of(options);
            var value = Symbol.Of("This is a \\long string that needs 'to be broken into multiple lines.");

            var result = SymbolSerializer.Serialize(value, context);

            var expected = "'Symbol This is a \\\\long stri'\r\n+ 'ng that needs \\'to be'\r\n+ ' broken into multipl'\r\n+ 'e lines.'";
            Assert.AreEqual(expected, result);
        }
    }
}
