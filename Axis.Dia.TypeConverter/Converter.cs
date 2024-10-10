using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;

namespace Axis.Dia.TypeConverter
{
    /// <summary>
    /// Entry-point into the converter Library.
    /// </summary>
    public class Converter
    {
        #region Clr
        public static object? ToClr(IDiaValue diaValue, Type destinationType, Clr.Options? options = null)
        {
            ArgumentNullException.ThrowIfNull(diaValue);
            ArgumentNullException.ThrowIfNull(destinationType);

            return Clr.ValueConverter.DefaultInstance.ToClr(
                diaValue,
                destinationType.ToTypeInfo(),
                new Clr.ConverterContext(options ?? Clr.Options.NewBuilder().Build()));
        }

        public static TDestinationType? ToClr<TDestinationType>(
            IDiaValue diaValue,
            Clr.Options? options = null)
            => ToClr(diaValue, typeof(TDestinationType), options).As<TDestinationType?>();
        #endregion

        #region Dia
        public static IDiaValue ToDia(Type sourceType, object? value, Dia.Options? options = null)
        {
            ArgumentNullException.ThrowIfNull(sourceType);

            return Dia.ValueConverter.DefaultInstance.ToDia(
                sourceInstance: value,
                sourceType: sourceType.ToTypeInfo(),
                context: new Dia.ConverterContext(options ?? Dia.Options.NewBuilder().Build()));
        }

        public static IDiaValue ToDia<TSourceType>(
            TSourceType value,
            Dia.Options? options = null)
            => ToDia(typeof(TSourceType), value, options);
        #endregion
    }
}
