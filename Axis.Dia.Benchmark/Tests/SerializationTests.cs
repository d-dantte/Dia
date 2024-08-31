using Axis.Dia.Benchmark.Types;
using Axis.Dia.Contracts;
using Axis.Luna.Extensions;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Benchmark.Tests
{
    public class SerializationTests
    {
        private static SampleType? data = null;
        private static IDiaValue? dia = null;

        [Benchmark]
        public object Dia_Clr_To_Record()
        {
            return Convert.Type.TypeConverter.ToDia(data);
        }

        [Benchmark]
        public object Dia_Record_To_Text()
        {
            return Convert.Axon.AxonSerializer
                .SerializeValue(dia!)
                .Resolve();
        }

        [Benchmark]
        public object Dia_Clr_To_Text()
        {
            return Convert.Type.TypeConverter
                .ToDia(data)
                .Bind(value => Convert.Axon.AxonSerializer.SerializeValue(value))
                .Resolve();
        }

        [Benchmark]
        public void Newtonsoft_Clr_To_JObject()
        {
            var jobj = JObject.FromObject(data!);
        }

        [Benchmark]
        public object Newtonsoft_Clr_To_Text()
        {
            return JsonConvert.SerializeObject(data);
        }

        static SerializationTests()
        {
            data = new SampleType
            {
                Age = 4,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco "
                    + "laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, "
                    + "sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Balance = 54.6m,
                Image = new byte[100].With(barr => new Random().NextBytes(barr)),
                Name = "my name",
                Status = true,
                Weight = 88.2d,
                Person = new SampleType2
                {
                    Dob = DateTimeOffset.Now,
                    FirstName = "Test",
                    LastName = "Hump",
                }

            };

            dia = Convert.Type.TypeConverter.ToDia(data).Resolve();
        }
    }
}
