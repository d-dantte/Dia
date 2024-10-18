using Axis.Dia.Core.Contracts;
using Axis.Dia.Json.Path;
using Axis.Dia.Json.Serializers;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Json
{
    public class Serializer
    {
        public JObject Serialize(IDiaValue dia)
        {
            ArgumentNullException.ThrowIfNull(dia);

            var jobj = new JObject();
            var path = new ValuePath();
            var context = new SerializerContext();

            jobj["dia"] = dia switch
            {
                Core.Types.Blob value => ValueSerializer.SerializeBlob(value, path, context),
                Core.Types.Boolean value => ValueSerializer.SerializeBool(value, path, context),
                Core.Types.Decimal value => ValueSerializer.SerializeDecimal(value, path, context),
                Core.Types.Duration value => ValueSerializer.SerializeDuration(value, path, context),
                Core.Types.Integer value => ValueSerializer.SerializeInteger(value, path, context),
                Core.Types.Record value => ValueSerializer.SerializeRecord(value, path, context),
                Core.Types.Sequence value => ValueSerializer.SerializeSequence(value, path, context),
                Core.Types.String value => ValueSerializer.SerializeString(value, path, context),
                Core.Types.Symbol value => ValueSerializer.SerializeSymbol(value, path, context),
                Core.Types.Timestamp value => ValueSerializer.SerializeTimestamp(value, path, context),
                _ => throw new InvalidOperationException(
                    $"Invalid dia value: {dia}")
            };
            jobj["metadata"] = context.GenerateMetadata();

            return jobj;
        }

        public IDiaValue Deserialize(JObject jsonContainer)
        {
            throw new NotImplementedException();
        }
    }
}
