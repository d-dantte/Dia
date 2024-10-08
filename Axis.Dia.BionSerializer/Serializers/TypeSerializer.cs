using Axis.Dia.BionSerializer.Types;
using Axis.Dia.Core;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class TypeSerializer :
        ITypeSerializer<IDiaType>,
        IDefaultInstance<TypeSerializer>
    {
        public static TypeSerializer DefaultInstance { get; } = new();

        public void SerializeType(IDiaType value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value is Core.Types.Attribute att)
                DiaAttributeSerializer.DefaultInstance.SerializeType(att, context);

            else if (value is Core.Types.Boolean @bool)
                DiaBooleanSerializer.DefaultInstance.SerializeType(@bool, context);

            else if (value is Core.Types.Blob blob)
                DiaBlobSerializer.DefaultInstance.SerializeType(blob, context);

            else if (value is Core.Types.Decimal @decimal)
                DiaDecimalSerializer.DefaultInstance.SerializeType(@decimal, context);

            else if (value is Core.Types.Duration duration)
                DiaDurationSerializer.DefaultInstance.SerializeType(duration, context);

            else if (value is Core.Types.Integer integer)
                DiaIntegerSerializer.DefaultInstance.SerializeType(integer, context);

            else if (value is Core.Types.String @string)
                DiaStringSerializer.DefaultInstance.SerializeType(@string, context);

            else if (value is Core.Types.Symbol symbol)
                DiaSymbolSerializer.DefaultInstance.SerializeType(symbol, context);

            else if (value is Core.Types.Timestamp timestamp)
                DiaTimestampSerializer.DefaultInstance.SerializeType(timestamp, context);

            else if (value is Core.Types.Sequence seq)
                DiaSequenceSerializer.DefaultInstance.SerializeType(seq, context);

            else if (value is Core.Types.Record rec)
                DiaRecordSerializer.DefaultInstance.SerializeType(rec, context);

            else if (value is Reference @ref)
                DiaReferenceSerializer.DefaultInstance.SerializeType(@ref, context);

            else
                throw new NotImplementedException();
        }

        public void SerializeAttributeSet(AttributeSet attributes, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (attributes.IsEmpty)
                return;

            // Write the attribute count as a varbyte (chances of having over a hundred attributes are slim, so using a varbyte saves space)
            if (!attributes.IsEmpty)
                attributes.Count
                    .ApplyTo(@int => (BigInteger)@int)
                    .ApplyTo(@int => @int.ToByteArray(true))
                    .Consume(array => context.Buffer.Write(array));

            var attributeSerializer = DiaAttributeSerializer.DefaultInstance;
            attributes.ForEvery(att => attributeSerializer.SerializeType(att, context));
        }
    }
}
