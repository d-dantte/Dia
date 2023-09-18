namespace Axis.Dia.Convert.Type.Exceptions
{
    /// <summary>
    /// Raised when the source clr type cannot be converted into any <see cref="Dia.Contracts.DiaType"/> instances.
    /// </summary>
    public class UnknownClrSourceTypeException: Exception
    {
        public System.Type SourceType { get; }

        public UnknownClrSourceTypeException(System.Type sourceType)
            :base($"The given clr type '{sourceType.FullName}' cannot be converted into any Dia type")
        {
            SourceType = sourceType;
        }
    }
}
