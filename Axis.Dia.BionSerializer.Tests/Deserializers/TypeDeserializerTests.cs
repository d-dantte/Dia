using Axis.Dia.BionSerializer.Deserializers;
using Axis.Dia.BionSerializer.Serializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Common;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Tests.Deserializers
{
    [TestClass]
    public class TypeDeserializerTests
    {
        [TestMethod]
        public void DeserializeTypeMetadata_Tests()
        {

        }

        [TestMethod]
        public void DeserializeAttributeSet_Tests()
        {
            var typeDeserializer = DeserializerContext.CompoundDeserializer.DefaultInstance;
            var typeSerializer = TypeSerializer.DefaultInstance;

            ArrayUtil
                .Of(
                    AttributeSet.Of(("abc", "def")),
                    AttributeSet.Of(("abc", "def"), "ghi"),
                    AttributeSet.Of(("abc", "def"), "ghi", "jkl"),
                    AttributeSet.Of(("abc", "def"), "ghi", "jkl", "mnop", "qrs", "stuff", ("x34", "bleh")))
                .ForEvery(attset =>
                {
                    var scontext = new SerializerContext();
                    typeSerializer.SerializeAttributeSet(attset, scontext);

                    scontext.Buffer.Stream.Seek(0, SeekOrigin.Begin);
                    var result = typeDeserializer.DeserializeAttributeSet(scontext.Buffer.Stream, new DeserializerContext());

                    Assert.AreEqual(attset, result);
                });
        }
    }
}
