using Axis.Dia.Contracts;
using Axis.Dia.Convert.Type.Exceptions;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Type.Converters
{
    /// <summary>
    /// Converts dia symbols into the clr enums, and vice-versa.
    /// <para/>
    /// This converter needs not participate in reference tracking during Clr conversion, because any reference to the
    /// enum type will be dereferenced and converted, bearing in mind that since all enum types are self-contained, there is
    /// no chance of a cyclic reference error occuring.
    /// </summary>
    internal class EnumTypeConverter: IClrConverter, IDiaConverter
    {
        #region IClrConverter
        public bool CanConvert(DiaType sourceType, System.Type destinationType)
        {
            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            return destinationType.IsEnum && DiaType.Symbol.Equals(sourceType);
        }

        public IResult<object?> ToClr(IDiaValue sourceInstance, System.Type destinationType, Clr.ConverterContext context)
        {
            if (sourceInstance is null)
                throw new ArgumentNullException(nameof(sourceInstance));

            if (destinationType is null)
                throw new ArgumentNullException(nameof(destinationType));

            if (!CanConvert(sourceInstance.Type, destinationType))
                return Result.Of<object?>(new IncompatibleClrConversionException(
                    sourceInstance.Type,
                    destinationType));

            var symbol = sourceInstance.As<SymbolValue>();
            if (!symbol.IsIdentifier)
                throw new ArgumentException($"Invalid symbol format: '{symbol.Value}'");

            return Result.Of<object?>(() => Enum.Parse(destinationType, symbol.Value!));
        }
        #endregion

        #region IDiaConverter
        public bool CanConvert(System.Type sourceType)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            return sourceType.IsEnum;
        }

        public IResult<IDiaValue> ToDia(System.Type sourceType, object? sourceInstance, Dia.ConverterContext options)
        {
            if (sourceType is null)
                throw new ArgumentNullException(nameof(sourceType));

            if(!sourceType.IsEnum)
                throw new ArgumentException($"Supplied {nameof(sourceType)} is not an enum");

            if (sourceInstance is null)
                return Result.Of<IDiaValue>(SymbolValue.Null());

            if (!sourceType.Equals(sourceInstance!.GetType()))
                return Result.Of<IDiaValue>(new TypeMismatchException(sourceType, sourceInstance!.GetType()));

            return Result.Of<IDiaValue>(SymbolValue.Of(sourceInstance.ToString()));
        }
        #endregion
    }
}
