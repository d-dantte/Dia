using Axis.Dia.Contracts;

namespace Axis.Dia.Convert.Type.Exceptions
{
    /// <summary>
    /// Raised when the source <see cref="Dia.Contracts.DiaType"/> cannot be converted to the given destination clr type
    /// </summary>
    public class IncompatibleClrConversionException: Exception
    {
        public DiaType SourceType { get; }

        public System.Type DestinationType { get; }

        public IncompatibleClrConversionException(DiaType sourceType, System.Type destinationType)
            : base($"An Exception occured while converting the types. Source: {sourceType}, Destination: {destinationType.FullName}")
        {
            SourceType = sourceType;
            DestinationType = destinationType;
        }
    }
}
