using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Core.Benchmark.Tests
{
    [MemoryDiagnoser(false)]
    public class ToStringTests
    {
        private Types.Record diaRecord;
        private SerializerContext diaContext;

        private JObject? jobject;

        [GlobalSetup]
        public void Setup()
        {
            var now = DateTimeOffset.Now;
            diaRecord = new Types.Record
            {
                ["the bool"] = true,
                ["data"] = Array.Empty<byte>(),
                ["count"] = 56,
                [Record.PropertyName.Of("weight", "killograms")] = 544.2m,
                ["created"] = now,
                ["name"] = "something stringy",
                ["tags"] = Types.Symbol.Of("a symbol"),
                ["scopes"] = new Types.Record
                {
                    ["first"] = true,
                    ["second"] = new Sequence
                    {
                        false
                    }
                }
            };
            diaContext = Options
                .Builder()
                .Build()
                .ApplyTo(SerializerContext.Of);

            jobject = new JObject
            {
                ["the bool"] = true,
                ["data"] = Array.Empty<byte>(),
                ["count"] = 56,
                ["weight::killograms"] = 544.2m,
                ["created"] = now,
                ["name"] = "something stringy",
                ["tags"] = "a symbol",
                ["scopes"] = new JObject
                {
                    ["first"] = true,
                    ["second"] = new JArray
                    {
                        false
                    }
                }
            };
        }

        [Benchmark]
        public void DiaToString()
        {
            var text = RecordSerializer.Serialize(this.diaRecord, this.diaContext);
        }

        [Benchmark]
        public void JsonToString()
        {
            var text = jobject!.ToString();
        }
    }
}
