using Axis.Dia.Axon;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core.Types;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class RecordDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            var cxt = new DeserializerContext();
            Assert.ThrowsException<FormatException>(
                () => RecordDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => RecordDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => RecordDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks"), cxt));

            Assert.ThrowsException<ArgumentNullException>(
                () => RecordDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks"), null));

            var result = RecordDeserializer.Deserialize("{}", cxt);
            Assert.AreEqual(0, result.Count);

            result = RecordDeserializer.Deserialize("@flag; {}", cxt);
            Assert.AreEqual(0, result.Count);
            Assert.IsTrue(result.Attributes.ContainsKey("flag"));

            result = RecordDeserializer.Deserialize("@new; null.record", cxt);
            Assert.IsTrue(result.IsNull);
            Assert.IsTrue(result.Attributes.ContainsKey("new"));

            result = RecordDeserializer.Deserialize("#2; {}", cxt);
            Assert.IsTrue(cxt.ReferenceMap.TryGetRef(2, out var r));
            Assert.IsTrue(result.ValueEquals((Record)r!));

            result = RecordDeserializer.Deserialize("#6; {some.thing.else: 65456}", cxt);
            Assert.IsTrue(cxt.ReferenceMap.TryGetRef(6, out r));
            Assert.IsTrue(result.ValueEquals((Record)r!));

            result = RecordDeserializer.Deserialize("#7; {more: @bleh; []}", cxt);
            Assert.IsTrue(cxt.ReferenceMap.TryGetRef(7, out r));
            Assert.IsTrue(result.ValueEquals((Record)r!));

            cxt = new();
            result = RecordDeserializer.Deserialize("#2; {abcd: 1, \"xyz\": 'Ref:Record 0x2', some.thing.else: 65.6E-2, more: @bleh; []}", cxt);
            Assert.AreEqual(4, result.Count);
            var secRef = result["more"].AsSequence();
            Assert.IsFalse(secRef.Attributes.ContainsKey(RefDeserializer.RefMarkerAttribute));
            var recRef = result["xyz"].AsRecord();
            Assert.IsTrue(recRef.Attributes.ContainsKey(RefDeserializer.RefMarkerAttribute));

            cxt.ExecuteResolvers();
            var refRec2 = result["xyz"].AsRecord();
            Assert.AreEqual(refRec2.RefHash(), result.RefHash());
        }
    }
}
