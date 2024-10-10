using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Core;
using Axis.Dia.Core.Contracts;

namespace Axis.Dia.Axon
{
    public class Serializer :
        IValueSerializer<IDiaValue>,
        IValueDeserializer<IDiaValue>
    {
        public static string Serialize(
            IDiaValue value,
            SerializerContext context)
            => ValueSerializer.Serialize(DiaValue.Of(value), context);

        public static IDiaValue Deserialize(string text, DeserializerContext? context = null)
        {
            context ??= new();

            var value = ValueDeserializer.Deserialize(text, context);

            context.ExecuteResolvers();
            return value;
        }
    }
}
