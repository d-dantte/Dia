using Axis.Dia.Core.Types;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Json.Tests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void Serialize_WhenValueIsRecord()
        {
            var serializer = new Serializer();
            var record = new Record
            {
                ["first"] = 34,
                ["second"] = 3.45m,
                ["third"] = Blob.Null("att", ("k", "v")),
                ["foruth"] = DateTimeOffset.Now,
                ["fifth"] = new Sequence
                {
                    "bleh", TimeSpan.FromSeconds(5454.345),
                    Symbol.Of("xyz", "att1", "att2")
                }
            };
            record["self"] = record;

            var jobj = serializer.Serialize(record);
            Console.WriteLine(jobj.ToString());
        }

        [TestMethod]
        public void Bleh()
        {
            var jobj = new JObject();
            jobj["*"] = true;
        }
    }
}
