using Axis.Dia.Contracts;
using Axis.Luna.Common.Results;

namespace Axis.Dia.Convert.Type
{
    /// <summary>
    /// Contract for converting <see cref="IDiaValue"/> instances into Clr instances
    /// </summary>
    public interface IClrConverter
    {
        /// <summary>
        /// Checks if the given pair can be converted to a CLR type by this converter.
        /// </summary>
        /// <param name="sourceType">The value to be converted</param>
        /// <param name="destinationType">The destination type</param>
        /// <returns>True if the conversion can be done, false otherwise</returns>
        bool CanConvert(DiaType sourceType, System.Type destinationType);


        /// <summary>
        /// Convert the given <paramref name="sourceInstance"/> to an instance of the given <paramref name="destinationType"/>.
        /// </summary>
        /// <param name="destinationType">The destination type</param>
        /// <param name="sourceInstance">The value to be converted</param>
        /// <param name="context">The context</param>
        /// <returns>The converted instance result</returns>
        IResult<object?> ToClr(IDiaValue sourceInstance, System.Type destinationType, Clr.ConverterContext context);
    }

    /// <summary>
    /// Contract for converting Clr instances into <see cref="IDiaValue"/> instances
    /// </summary>
    public interface IDiaConverter
    {
        /// <summary>
        /// Checks if the givne pair can be converted to a corresponding <see cref="IDiaValue"/> instance by this converter
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if the conversion can be done, false otherwise</returns>
        bool CanConvert(System.Type sourceType);

        /// <summary>
        /// Convert the given <paramref name="sourceInstance"/> to an instance of a corresponding <see cref="IDiaValue"/.>
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="sourceInstance">The source instance</param>
        /// <param name="context">The context</param>
        /// <returns>The converted instance result</returns>
        IResult<IDiaValue> ToDia(System.Type sourceType, object? sourceInstance, Dia.ConverterContext context);
    }
}
