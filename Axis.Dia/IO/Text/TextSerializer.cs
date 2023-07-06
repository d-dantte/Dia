using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.IO.Text
{
    public static class TextSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IResult<string> SerializePacket(
            params IDiaValue[] values)
            => SerializePacket(ValuePacket.Of(values));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static IResult<string> SerializePacket(ValuePacket packet, TextSerializerContext? context = null)
        {
            context ??= new TextSerializerContext();
            return packet.Values
                .Select(Serialize)
                .Fold()
                .Map(valueTexts => valueTexts.Aggregate(
                    string.Empty,
                    (prev, next) => ApplyPacketValueSeparator(prev, next, context)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diaText"></param>
        /// <returns></returns>
        public static IResult<ValuePacket> DeserializePacket(string diaText)
        {

        }

        public static IResult<string> Serialize(IDiaValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return value switch
            {
                BoolValue boolValue => throw new NotImplementedException(),
                IntValue intValue => throw new NotImplementedException(),
                DecimalValue decimalValue => throw new NotImplementedException(),
                InstantValue instantValue => throw new NotImplementedException(),
                SymbolValue symbolValue => throw new NotImplementedException(),
                StringValue stringValue => throw new NotImplementedException(),
                BlobValue blobValue => throw new NotImplementedException(),
                ClobValue clobValue => throw new NotImplementedException(),
                ListValue listValue => throw new NotImplementedException(),
                RecordValue recordValue => throw new NotImplementedException(),
                _ => throw new ArgumentException($"Invalid Dia Type: {value.GetType()}")
            };
        }

        public static IResult<IDiaValue> DeserializeDiaValue(string value)
        {

        }

        private static string ApplyPacketValueSeparator(string previous, string next, TextSerializerContext context)
        {

        }
    }
}
