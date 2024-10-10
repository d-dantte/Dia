using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;
using Axis.Luna.Extensions;

namespace Axis.Dia.TypeConverter.Clr
{
    public class EnumConverter : IClrConverter
    {
        public static EnumConverter DefaultInstance { get; } = new();

        public bool CanConvert(DiaType sourceType, TypeInfo destinationTypeInfo)
        {
            if (destinationTypeInfo.IsDefault)
                throw new ArgumentException(
                    $"Invalid {nameof(destinationTypeInfo)}: default");

            return DiaType.Symbol.Equals(sourceType)
                && TypeCategory.Enum.Equals(destinationTypeInfo.Category);
        }

        public object? ToClr(
            IDiaValue sourceInstance,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(sourceInstance);

            if (!CanConvert(sourceInstance.Type, destinationTypeInfo))
                throw new InvalidOperationException(
                    $"Invalid Dia - Enum conversion [source: {sourceInstance.Type}, destination: {destinationTypeInfo.Type}]");

            var symbol = sourceInstance.As<Core.Types.Symbol>();

            if (symbol.IsNull)
                return null;

            if (!Enum.TryParse(destinationTypeInfo.Type, symbol.Value, true, out var result))
                throw new FormatException(
                    $"Invalid symbol format: The symbol cannot be converted to an enum '{symbol.Value}'");

            return result;
        }
    }
}
