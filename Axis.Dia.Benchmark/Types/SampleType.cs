namespace Axis.Dia.Benchmark.Types
{
    internal class SampleType
    {
        public int Age { get; set; }
        public decimal Balance { get; set; }
        public double Weight { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public SampleType2 Person { get; set; }

        public bool Status { get; set; }

        public byte[] Image { get; set; }

        public List<string> Things { get; } = new List<string>
        {
            "Cars",
            "Groceries",
            "Aeroplanes",
            "Misc"
        };

        public Dictionary<string, string> Colors { get; } = new Dictionary<string, string>
        {
            ["favorite"] = "black",
            ["hated"] = "yellow"
        };
    }

    internal class SampleType2
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset Dob { get; set; }
    }
}
