using Axis.Dia.Core.Contracts;

namespace Axis.Dia.TypeConverter.Dia
{
    public class ValueConverter : IDiaConverter
    {
        public static ValueConverter DefaultInstance { get; } = new();

        public bool CanConvert(TypeInfo sourceTypeInfo) => true;

        public IDiaValue ToDia(TypeInfo sourceType, object? sourceInstance, ConverterContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (sourceType.IsDefault)
                throw new ArgumentException($"Invalid {nameof(sourceType)}: default");

            if (!context.ConverterManager.TryGetConverter(sourceType, out var converter))
                throw new InvalidOperationException(
                    $"Invalid source-type: no converter found for type '{sourceType}'");

            return converter!.ToDia(sourceType, sourceInstance, context);
        }
    }
}
