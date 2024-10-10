using Axis.Dia.Axon.Deserializers;

namespace Axis.Dia.Axon.Tests.Deserializers
{
    [TestClass]
    public class AttributeDeserializerTests
    {
        [TestMethod]
        public void Deserialize_Tests()
        {
            var text = "@flag; @key:value;@flag2;  @k:vp;";

            var set = AttributeDeserializer.Deserialize(text);
            Assert.AreEqual(4, set.Length);
            Assert.IsTrue(set.Contains("flag"));
            Assert.IsTrue(set.Contains("flag2"));
            Assert.IsTrue(set.Contains(("key", "value")));
            Assert.IsTrue(set.Contains(("k", "vp")));
        }
    }
}
