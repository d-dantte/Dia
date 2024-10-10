namespace Axis.Dia.Typhon.Clr
{
    public class ConverterContext
    {
        public Options Options { get; }

        public ConverterManager ConverterManager { get; }

        public InstanceTracker Tracker { get; }

        public ContextMetadata Metadata { get; }

        public ValueConverter ValueConverter { get; }

        public ConverterContext(Options options)
        {
            ArgumentNullException.ThrowIfNull(options);

            Options = options;
            Tracker = new();
            Metadata = new();
            ValueConverter = ValueConverter.DefaultInstance;
            ConverterManager = new ConverterManager(options);
        }
    }
}
