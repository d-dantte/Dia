using Axis.Dia.Contracts;

namespace Axis.Dia.Convert.Type.Exceptions
{
    /// <summary>
    /// Raised when the source clr type cannot be converted to an instance of the given <see cref="Dia.Contracts.DiaType"/>.
    /// </summary>
    public class IncompatibleDiaConversionException: Exception
    {
        public DiaType DestinationType { get; }

        public System.Type SourceType { get; }

        public IncompatibleDiaConversionException(System.Type sourceType, DiaType destinationType)
            : base($"An Exception occured while converting the types. Source: {sourceType.FullName}, Destination: {destinationType}")
        {
            SourceType = sourceType;
            DestinationType = destinationType;
        }
    }
}
