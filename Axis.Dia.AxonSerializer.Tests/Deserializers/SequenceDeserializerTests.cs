using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Types;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class SequenceDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            var cxt = new DeserializerContext();
            Assert.ThrowsException<FormatException>(
                () => SequenceDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => SequenceDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => SequenceDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks"), cxt));

            Assert.ThrowsException<ArgumentNullException>(
                () => SequenceDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks"), null));

            var result = SequenceDeserializer.Deserialize("[]", cxt);
            Assert.AreEqual(0, result.Count);

            result = SequenceDeserializer.Deserialize("@flag; []", cxt);
            Assert.IsTrue(result.Attributes.ContainsKey("flag"));

            result = SequenceDeserializer.Deserialize("@flag; null.sequence", cxt);
            Assert.IsTrue(result.Attributes.ContainsKey("flag"));
            Assert.IsTrue(result.IsNull);

            result = SequenceDeserializer.Deserialize("#2; []", cxt);
            Assert.IsTrue(cxt.ReferenceMap.TryGetRef(2, out var r));
            Assert.IsTrue(result.ValueEquals((Sequence)r!));

            cxt = new();
            result = SequenceDeserializer.Deserialize("#2; [1, 'Ref:Sequence 0x2']", cxt);
            Assert.AreEqual(2, result.Count);
            var secRef = result[1].AsSequence();
            Assert.IsTrue(secRef.Attributes.ContainsKey(RefDeserializer.RefMarkerAttribute));

            cxt.ExecuteResolvers();
            var secRef2 = result[1].AsSequence();
            Assert.AreEqual(secRef2.RefHash(), result.RefHash());
        }
    }
}
