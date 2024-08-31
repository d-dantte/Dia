using Axis.Dia.Axon.Deserializers;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.AxonSerializer.Deserializers;
using Axis.Dia.Core;

namespace Axis.Dia.Axon
{
    public class Converter :
        IValueSerializer<IDiaValue>,
        IValueDeserializer<IDiaValue>
    {
        public static string Serialize(
            IDiaValue value,
            SerializerContext context)
            => ValueSerializer.Serialize(ContainerValue.Of(value), context);

        public static IDiaValue Deserialize(string text, DeserializerContext? context = null)
        {
            context ??= new();

            var value = ValueDeserializer.Deserialize(text, context);

            context.ExecuteResolvers();
            return value;
        }
    }
}
