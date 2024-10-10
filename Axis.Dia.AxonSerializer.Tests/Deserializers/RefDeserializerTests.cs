using Axis.Dia.Axon.Deserializers;
using Axis.Luna.Extensions;
using Axis.Pulsar.Core.CST;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class RefDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            Assert.ThrowsException<FormatException>(
                () => RefDeserializer.Deserialize("abcd"));

            Assert.ThrowsException<ArgumentNullException>(
                () => RefDeserializer.Deserialize(default!));

            Assert.ThrowsException<ArgumentException>(
                () => RefDeserializer.Deserialize(ISymbolNode.Of("bleh", "toks")));

            Assert.ThrowsException<FormatException>(
                () => RefDeserializer.Deserialize("'r:bleh 0xA'"));

            #region Bool
            var result = RefDeserializer.Deserialize("'Ref:Boolean 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Boolean @bool));
            Assert.IsTrue(@bool.IsNull);
            Assert.IsTrue(@bool.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Decimal
            result = RefDeserializer.Deserialize("'r:decimal 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Decimal dec));
            Assert.IsTrue(dec.IsNull);
            Assert.IsTrue(dec.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Integer
            result = RefDeserializer.Deserialize("'r:integer 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Integer @int));
            Assert.IsTrue(@int.IsNull);
            Assert.IsTrue(@int.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Duration
            result = RefDeserializer.Deserialize("'r:duration 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Duration dur));
            Assert.IsTrue(dur.IsNull);
            Assert.IsTrue(dur.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Timestamp
            result = RefDeserializer.Deserialize("'r:timestamp 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Timestamp ts));
            Assert.IsTrue(ts.IsNull);
            Assert.IsTrue(ts.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region String
            result = RefDeserializer.Deserialize("'r:string 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.String str));
            Assert.IsTrue(str.IsNull);
            Assert.IsTrue(str.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Symbol
            result = RefDeserializer.Deserialize("'r:symbol 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Symbol sym));
            Assert.IsTrue(sym.IsNull);
            Assert.IsTrue(sym.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Blob
            result = RefDeserializer.Deserialize("'r:blob 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Blob blob));
            Assert.IsTrue(blob.IsNull);
            Assert.IsTrue(blob.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Sequence
            result = RefDeserializer.Deserialize("'r:sequence 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Sequence seq));
            Assert.IsTrue(seq.IsNull);
            Assert.IsTrue(seq.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion

            #region Record
            result = RefDeserializer.Deserialize("'r:record 0xA'");
            Assert.IsTrue(result.Is(out Core.Types.Record rec));
            Assert.IsTrue(rec.IsNull);
            Assert.IsTrue(rec.Attributes.Contains((RefDeserializer.RefMarkerAttribute, "A")));
            #endregion
        }
    }
}
