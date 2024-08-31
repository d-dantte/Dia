using Axis.Dia.Core;

namespace Axis.Dia.TypeConverter.Dia
{
    /// <summary>
    /// Contract for converting Clr instances into <see cref="IDiaValue"/> instances
    /// </summary>
    public interface IDiaConverter
    {
        /// <summary>
        /// Checks if the givne pair can be converted to a corresponding <see cref="IDiaValue"/> instance by this converter
        /// </summary>
        /// <param name="sourceTypeInfo">The info of the source type</param>
        /// <returns>True if the conversion can be done, false otherwise</returns>
        bool CanConvert(TypeInfo sourceTypeInfo);

        /// <summary>
        /// Convert the given <paramref name="sourceInstance"/> to an instance of a corresponding <see cref="IDiaValue"/.>
        /// </summary>
        /// <param name="sourceType">The source type info</param>
        /// <param name="sourceInstance">The source instance</param>
        /// <param name="context">The context</param>
        /// <returns>The converted instance result</returns>
        IDiaValue ToDia(TypeInfo sourceType, object? sourceInstance, ConverterContext context);
    }
}
