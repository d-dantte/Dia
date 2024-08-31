using Axis.Dia.Core;

namespace Axis.Dia.TypeConverter.Dia
{
    public class EnumConverter : IDiaConverter
    {
        internal static EnumConverter DefaultInstance { get; } = new();

        public bool CanConvert(TypeInfo sourceTypeInfo)
        {
            return CanConvert(sourceTypeInfo, out _);
        }

        private static bool CanConvert(TypeInfo sourceTypeInfo, out Type actualType)
        {
            if (sourceTypeInfo.IsDefault)
                throw new ArgumentException(
                    $"Invalid {nameof(sourceTypeInfo)}: default");

            return sourceTypeInfo.Type.IsEnumType(out actualType);
        }

        public IDiaValue ToDia(TypeInfo sourceTypeInfo, object? sourceInstance, Dia.ConverterContext options)
        {
            if (!CanConvert(sourceTypeInfo, out var actualSourceType))
                throw new InvalidOperationException(
                    $"Invalid Enum - Dia conversion [source: {sourceTypeInfo.Type}, instance: {sourceInstance?.GetType()}]");

            if (sourceInstance is null)
                return Core.Types.Symbol.Null();

            var instanceType = sourceInstance!.GetType();
            if (!instanceType.IsEnumType(out var actualInstanceType))
                throw new InvalidOperationException(
                    $"Invalid instance-type: '{instanceType}' is not an enum");

            if (!actualSourceType.Equals(actualInstanceType))
                throw new InvalidOperationException(
                    $"Type mismatch [expected: {actualSourceType}, actual: {actualInstanceType}]");

            return Core.Types.Symbol.Of(sourceInstance.ToString());
        }
    }
}
