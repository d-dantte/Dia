using Axis.Dia.Convert.Axon;
using Axis.Dia.Exceptions;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Tests.Convert.Axon
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void SerializeValue_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => AxonSerializer.SerializeValue(null));
            Assert.ThrowsException<ReferenceLinkageException>(() => AxonSerializer.SerializeValue(ReferenceValue.Of(Guid.NewGuid())));

            var intValue = IntValue.Of(34);
            var refValue = ReferenceValue.Of(intValue);

            var result = AxonSerializer.SerializeValue(refValue);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("#1::34", result.Resolve());

            var record = new RecordValue
            {
                ["item_count"] = intValue,
                ["item_count_ref"] = refValue
            };

            result = AxonSerializer.SerializeValue(record);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("{item_count:#1::34, item_count_ref:@1}", result.Resolve());
        }

        [TestMethod]
        public void ParseValue_Tests()
        {
            var recordText = "{item_count:#1::34, item_count_ref:@1}";

            Assert.ThrowsException<ArgumentException>(() => AxonSerializer.ParseValue((string)null));
            Assert.ThrowsException<ArgumentNullException>(() => AxonSerializer.ParseValue((CSTNode)null));
            Assert.ThrowsException<ArgumentException>(() => AxonSerializer.ParseValue(""));

            var result = AxonSerializer.ParseValue(recordText);
            Assert.IsTrue(result.IsDataResult());
        }
    }
}
