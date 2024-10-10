using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Typhon.Clr
{
    public class ValueConverter : IClrConverter
    {
        public static ValueConverter DefaultInstance { get; } = new();

        internal ValueConverter() { }

        public bool CanConvert(
            DiaType sourceType,
            TypeInfo destinationTypeInfo)
            => true;

        public object? ToClr(
            IDiaValue sourceInstance,
            TypeInfo destinationTypeInfo,
            ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(sourceInstance);
            ArgumentNullException.ThrowIfNull(context);

            if (!context.ConverterManager.TryGetConverter(sourceInstance.Type, destinationTypeInfo, out var converter))
                throw new InvalidOperationException(
                    $"Invalid conversion: no converter found for [SourceDiaType: {sourceInstance.Type}, DestinationTypeInfo: {destinationTypeInfo}");

            return converter!.ToClr(sourceInstance, destinationTypeInfo, context);
        }
    }
}
