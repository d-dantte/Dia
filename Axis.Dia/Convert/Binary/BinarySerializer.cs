using Axis.Dia.Contracts;
using Axis.Dia.Convert.Binary.Serializers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Convert.Binary
{
    public static class BinarySerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static IResult<byte[]> Serialize(ValuePacket packet, BinarySerializerContext? context = null)
        {
            context ??= new BinarySerializerContext();
            return packet.Values
                .Select(value => PayloadSerializer.SerializeDiaValueResult(value, context))
                .Fold()
                .Map(v => v.Aggregate(
                    Array.Empty<byte>(),
                    ArrayExtensions.ConcatWith));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetStream"></param>
        /// <returns></returns>
        public static IResult<ValuePacket> Deserialize(Stream packetStream, BinarySerializerContext? context = null)
        {
            context ??= new BinarySerializerContext();
            var valueList = new List<IResult<IDiaValue>>();
            IResult<IDiaValue> result;
            while (packetStream.TryDeserializeDiaValueResult(context, out result))
            {
                valueList.Add(result);
            }

            if (result is IResult<IDiaValue>.ErrorResult errorResult)
            {
                if (errorResult.Cause().InnerException is EndOfStreamException)
                    return valueList.Fold().Map(ValuePacket.Of);

                else return result.Map(_ => default(ValuePacket));
            }
            else return Result.Of<ValuePacket>(new Exception("Fatal error: Packet Deserialization failed"));
        }
    }
}
