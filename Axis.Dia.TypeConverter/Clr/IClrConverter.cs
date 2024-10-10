using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.TypeConverter.Clr
{
    /// <summary>
    /// Contract for converting <see cref="IDiaValue"/> instances into Clr instances
    /// </summary>
    public interface IClrConverter
    {
        /// <summary>
        /// Checks if the given pair can be converted to a CLR type by this converter.
        /// </summary>
        /// <param name="sourceType">The type info to be converted</param>
        /// <param name="destinationTypeInfo">The destination type info</param>
        /// <returns>True if the conversion can be done, false otherwise</returns>
        bool CanConvert(DiaType sourceType, TypeInfo destinationTypeInfo);

        /// <summary>
        /// Convert the given <paramref name="sourceInstance"/> to an instance of the given <paramref name="destinationType"/>.
        /// </summary>
        /// <param name="sourceInstance">The value to be converted</param>
        /// <param name="destinationTypeInfo">The destination type info</param>
        /// <param name="context">The context</param>
        /// <returns>The converted instance result</returns>
        object? ToClr(IDiaValue sourceInstance, TypeInfo destinationTypeInfo, Clr.ConverterContext context);
    }
}
