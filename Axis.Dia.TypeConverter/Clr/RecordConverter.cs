using Axis.Dia.Core;

namespace Axis.Dia.TypeConverter.Clr
{
    public class RecordConverter :
        IClrConverter,
        IDefaultInstance<RecordConverter>
    {
        public static RecordConverter DefaultInstance { get; } = new();

        public bool CanConvert(DiaType sourceType, TypeInfo destinationTypeInfo)
        {
            throw new NotImplementedException();
        }

        public object? ToClr(IDiaValue sourceInstance, TypeInfo destinationTypeInfo, ConverterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
