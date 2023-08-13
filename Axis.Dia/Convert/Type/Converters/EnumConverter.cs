using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Convert.Type.Converters
{
    /// <summary>
    /// Converts dia symbols into the clr enums, and vice-versa.
    /// </summary>
    internal class EnumConverter: IClrConverter, IDiaConverter
    {
        #region IClrConverter
        public bool CanConvert(DiaType sourceType, System.Type destinationType)
        {
            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            return destinationType.IsEnum && DiaType.Symbol.Equals(sourceType);
        }

        public IResult<object?> ToClr(
            IDiaValue sourceInstance,
            System.Type destinationType,
            TypeConverterContext context)
        {
            if (destinationType is null)
                return Result.Of<object?>(new ArgumentNullException(nameof(destinationType)));

            return sourceInstance is SymbolValue symbol && symbol.IsIdentifier
                ? Result.Of<object?>(Enum.Parse(destinationType, symbol.Value!))
                : Result.Of<object?>(new ArgumentException($"Invalid value: cannot convert '{sourceInstance}' to enum"));
        }
        #endregion

        #region IDiaConverter
        public bool CanConvert(System.Type sourceType)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            return sourceType.IsEnum;
        }

        public IResult<IDiaValue> ToDia(System.Type sourceType, object? sourceInstance, TypeConverterContext options)
        {
            if (sourceInstance is null)
                return Result.Of<IDiaValue>(SymbolValue.Null());

            if (!sourceInstance.GetType().IsEnum)
                return Result.Of<IDiaValue>(new ArgumentException($"Supplied {nameof(sourceInstance)} is not an enum"));

            return Result.Of<IDiaValue>(SymbolValue.Of(sourceInstance.ToString()));
        }
        #endregion
    }
}
