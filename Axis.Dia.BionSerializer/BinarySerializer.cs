using Axis.Dia.BionSerializer.Deserializers;
using Axis.Dia.BionSerializer.Serializers;

namespace Axis.Dia.BionSerializer
{
    public static class BinarySerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        //public static IResult<byte[]> Serialize(ValuePacket packet, SerializerContext? context = null)
        //{
        //    context ??= new SerializerContext();

        //    foreach (var value in packet.Values)
        //    {
        //        try
        //        {
        //            _ = value switch
        //            {
        //                ListValue list => ReferenceUtil.LinkReferences(list, out _),
        //                RecordValue record => ReferenceUtil.LinkReferences(record, out _),
        //                IDiaReference @ref => ReferenceUtil.LinkReferences(@ref, out _),
        //                _ => value
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            return Result.Of<byte[]>(ex);
        //        }
        //    }

        //    return packet.Values
        //        .Select(value => PayloadSerializer.SerializeDiaValueResult(value, context))
        //        .FoldInto(v => v.Aggregate(Array.Empty<byte>(), ArrayExtensions.ConcatWith));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetStream"></param>
        /// <returns></returns>
        //public static IResult<ValuePacket> Deserialize(Stream packetStream, DeserializerContext? context = null)
        //{
        //    context ??= new DeserializerContext();
        //    var valueList = new List<IResult<IDiaValue>>();
        //    IResult<IDiaValue> result;
        //    while (packetStream.TryDeserializeDiaValueResult(context, out result))
        //    {
        //        valueList.Add(result);
        //    }

        //    if (result.IsErrorResult(out var error))
        //    {
        //        if (error is EndOfStreamException)
        //            return valueList
        //                .Select(result => result.Map(value => value switch
        //                {
        //                    ListValue list => ReferenceUtil.LinkReferences(list, out _),
        //                    RecordValue record => ReferenceUtil.LinkReferences(record, out _),
        //                    IDiaReference @ref => ReferenceUtil.LinkReferences(@ref, out _),
        //                    _ => value
        //                }))
        //                .FoldInto(ValuePacket.Of);

        //        else return result.Map(_ => default(ValuePacket));
        //    }
        //    else return Result.Of<ValuePacket>(new Exception("Fatal error: Packet Deserialization failed"));
        //}
    }
}
