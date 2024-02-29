using Axis.Dia.Core.Types;

namespace Axis.Dia.Core.Convert.Axon.Serializers
{
    public class ValueSerializer: ISerializer<IDiaValue>
    {
        public static string Serialize(IDiaValue value, SerializerContext context)
        {
            return value switch
            {
                Types.Boolean v => BooleanSerializer.Serialize(v, context),
                Blob v => BlobSerializer.Serialize(v, context),
                Types.Decimal v => DecimalSerializer.Serialize(v, context),
                Integer v => IntegerSerializer.Serialize(v, context),
                Record v => RecordSerializer.Serialize(v, context),
                Sequence v => SequenceSerializer.Serialize(v, context),
                Types.String v => StringSerializer.Serialize(v, context),
                Symbol v => SymbolSerializer.Serialize(v, context),
                Timestamp v => TimestampSerializer.Serialize(v, context),
                null => throw new ArgumentNullException(nameof(value)),
                _ => throw new InvalidOperationException($"Invalid value type: unknown - '{value.GetType()}'")
            };
        }
    }
}
