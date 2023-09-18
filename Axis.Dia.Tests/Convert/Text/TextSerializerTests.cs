using Axis.Dia.Convert.Text;
using Axis.Dia.Exceptions;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Tests.Convert.Text
{
    [TestClass]
    public class TextSerializerTests
    {
        [TestMethod]
        public void SerializeValue_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => TextSerializer.SerializeValue(null));
            Assert.ThrowsException<ReferenceNormalizationException>(() => TextSerializer.SerializeValue(ReferenceValue.Of(Guid.NewGuid())));

            var intValue = IntValue.Of(34);
            var refValue = ReferenceValue.Of(intValue);

            var result = TextSerializer.SerializeValue(refValue);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("#1::34", result.Resolve());

            var record = new RecordValue
            {
                ["item_count"] = intValue,
                ["item_count_ref"] = refValue
            };

            result = TextSerializer.SerializeValue(record);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("{item_count:#1::34, item_count_ref:@1}", result.Resolve());
        }

        [TestMethod]
        public void ParseValue_Tests()
        {
            var recordText = "{item_count:#1::34, item_count_ref:@1}";

            Assert.ThrowsException<ArgumentException>(() => TextSerializer.ParseValue((string)null));
            Assert.ThrowsException<ArgumentNullException>(() => TextSerializer.ParseValue((CSTNode)null));
            Assert.ThrowsException<ArgumentException>(() => TextSerializer.ParseValue(""));

            var result = TextSerializer.ParseValue(recordText);
            Assert.IsTrue(result.IsDataResult());
        }
    }
}
