using Axis.Dia.Convert.Axon;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Tests.Convert.Axon
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void SerializeValue_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => AxonSerializer.SerializeValue(null!));

            var intValue = IntValue.Of(34);
            var refValue = ReferenceValue.Of(intValue);

            var result = AxonSerializer.SerializeValue(refValue);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("@0x1", result.Resolve());

            var record = new RecordValue
            {
                ["item_count"] = intValue,
                ["item_count_ref"] = refValue
            };

            result = AxonSerializer.SerializeValue(record);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("{ item_count: #0x134, item_count_ref: @0x1 }", result.Resolve());
        }

        [TestMethod]
        public void ParseValue_Tests()
        {
            var recordText = "{item_count:#0x1::34, item_count_ref:@0x1}";

            Assert.ThrowsException<ArgumentException>(() => AxonSerializer.ParseValue(null));
            Assert.ThrowsException<ArgumentNullException>(() => AxonSerializer.ParseValue(null, new()));
            Assert.ThrowsException<ArgumentException>(() => AxonSerializer.ParseValue(""));

            var result = AxonSerializer.ParseValue(recordText);
            Assert.IsTrue(result.IsDataResult());
        }
    }
}
