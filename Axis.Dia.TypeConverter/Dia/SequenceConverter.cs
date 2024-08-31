using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.TypeConverter.Dia
{
    public class SequenceConverter :
        IDiaConverter,
        IDefaultInstance<SequenceConverter>
    {
        public static SequenceConverter DefaultInstance { get; } = new();

        public bool CanConvert(TypeInfo sourceTypeInfo)
        {
            return sourceTypeInfo.Category switch
            {
                TypeCategory.Sequence
                or TypeCategory.SingleDimensionArray => true,
                _ => false
            };
        }

        public IDiaValue ToDia(
            TypeInfo sourceTypeInfo,
            object? sourceInstance,
            ConverterContext context)
        {
            if (!CanConvert(sourceTypeInfo))
                throw new InvalidOperationException(
                    $"Invalid source-type: '{sourceTypeInfo.Type}' is not a simple type");

            if (sourceInstance is null)
                return Sequence.Null();

            if (context.Tracker.TryAdd(sourceInstance, _ => Sequence.Empty(), out var seq))
            {
                // populate the instance
                var itemTypeInfo = sourceTypeInfo.ItemType!.ToTypeInfo();
                sourceInstance
                    .As<System.Collections.IEnumerable>()
                    .SelectObject(obj => context.ValueConverter.ToDia(itemTypeInfo, obj, context))
                    .Select(ContainerValue.Of)
                    .ForEvery(seq.Add);
            }

            return seq;
        }
    }
}
