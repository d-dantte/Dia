namespace Axis.Dia.Convert.Type.Exceptions
{
    /// <summary>
    /// Raised when the clr source type is not compatible with the given clr source instance (actual type).
    /// Compatibility in this case refers to being the same type, or a derived type of the source type.
    /// </summary>
    internal class TypeMismatchException: Exception
    {
        public System.Type? ExpectedType { get; }

        public System.Type? ActualType { get; }


        public TypeMismatchException(System.Type? expectedType, System.Type? actualType)
            : base($"The given types do not match: Expected: {expectedType?.FullName ?? "null"}, Actual: {actualType?.FullName ?? "null"}")
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }
    }
}
