using Axis.Dia.Core;

namespace Axis.Dia.Axon.Serializers
{
    public class ValueSerializer: ISerializer<ContainerValue>
    {
        public static string Serialize(ContainerValue value, SerializerContext context)
        {
            return value.Type switch
            {
                DiaType.Bool => BooleanSerializer.Serialize(value.AsBool(), context),
                DiaType.Blob => BlobSerializer.Serialize(value.AsBlob(), context),
                DiaType.Int => IntegerSerializer.Serialize(value.AsInteger(), context),
                DiaType.Decimal => DecimalSerializer.Serialize(value.AsDecimal(), context),
                DiaType.Duration => DurationSerializer.Serialize(value.AsDuration(), context),
                DiaType.Timestamp => TimestampSerializer.Serialize(value.AsTimestamp(), context),
                DiaType.String => StringSerializer.Serialize(value.AsString(), context),
                DiaType.Symbol => SymbolSerializer.Serialize(value.AsSymbol(), context),
                DiaType.Sequence => SequenceSerializer.Serialize(value.AsSequence(), context),
                DiaType.Record => RecordSerializer.Serialize(value.AsRecord(), context),
                _ => throw new InvalidOperationException($"Invalid value type: unknown - '{value.GetType()}'")
            };
        }
    }
}
