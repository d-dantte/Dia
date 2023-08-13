using Axis.Dia.Contracts;
using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Convert.Text
{
    public static class TextSerializer
    {
        #region SymbolNames
        public const string SymbolNamePacket = "packet";
        public const string SymbolNameDiaValue = "dia-value";
        #endregion

        #region Packet
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IResult<string> SerializePacket(
            TextSerializerContext? context,
            params IDiaValue[] values)
            => SerializePacket(ValuePacket.Of(values), context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static IResult<string> SerializePacket(ValuePacket packet, TextSerializerContext? context = null)
        {
            context ??= new TextSerializerContext();
            return packet.Values
                .Select(v => SerializeValue(v, context))
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
        public static IResult<ValuePacket> ParsePacket(string text, TextSerializerContext? context = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"Invalid text: '{text}'");

            var parseResult = GrammarUtil.Grammar
                .GetRecognizer(SymbolNamePacket)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => ParsePacket(success.Symbol, context),
                null => Result.Of<ValuePacket>(new Exception("Unknown Error")),
                _ => Result.Of<ValuePacket>(new ParseException(parseResult))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<ValuePacket> ParsePacket(CSTNode node, TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(node);

            return node
                .FindNodes(SymbolNameDiaValue)
                .Select(valueNode => ParseValue(valueNode, context))
                .Fold()
                .Map(ValuePacket.Of);
        }
        #endregion

        #region Value
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<string> SerializeValue(IDiaValue value, TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(value);

            return value switch
            {
                BoolValue boolValue => BoolParser.Serialize(boolValue, context),
                IntValue intValue => IntParser.Serialize(intValue, context),
                DecimalValue decimalValue => DecimalParser.Serialize(decimalValue, context),
                InstantValue instantValue => InstantParser.Serialize(instantValue, context),
                SymbolValue symbolValue => SymbolParser.Serialize(symbolValue, context),
                StringValue stringValue => StringParser.Serialize(stringValue, context),
                BlobValue blobValue => BlobParser.Serialize(blobValue, context),
                ClobValue clobValue => ClobParser.Serialize(clobValue, context),
                ListValue listValue => ListParser.Serialize(listValue, context),
                RecordValue recordValue => RecordParser.Serialize(recordValue, context),
                _ => throw new ArgumentException($"Invalid Dia Type: {value.GetType()}")
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<IDiaValue> ParseValue(string text, TextSerializerContext? context = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"Invalid text: '{text}'");

            var parseResult = GrammarUtil.Grammar
                .GetRecognizer(SymbolNameDiaValue)
                .Recognize(text);

            return parseResult switch
            {
                SuccessResult success => ParseValue(success.Symbol.FirstNode(), context),
                null => Result.Of<IDiaValue>(new Exception("Unknown Error")),
                _ => Result.Of<IDiaValue>(new ParseException(parseResult))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<IDiaValue> ParseValue(CSTNode valueNode, TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(valueNode);

            return valueNode.SymbolName switch
            {
                SymbolNameDiaValue => ParseValue(valueNode.FirstNode(), context),
                BoolParser.SymbolNameDiaBool => BoolParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                IntParser.SymbolNameDiaInt => IntParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                DecimalParser.SymbolNameDiaDecimal => DecimalParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                InstantParser.SymbolNameDiaInstant => InstantParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                StringParser.SymbolNameDiaString => StringParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                SymbolParser.SymbolNameDiaSymbol => SymbolParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                BlobParser.SymbolNameDiaBlob => BlobParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                ClobParser.SymbolNameDiaClob => ClobParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                ListParser.SymbolNameDiaList => ListParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                RecordParser.SymbolNameDiaRecord => RecordParser.Parse(valueNode, context).Map(r => (IDiaValue)r),
                _ => throw new ArgumentException($"Invalid Root value symbol name: {valueNode.SymbolName}")
            };
        }
        #endregion

        private static string ApplyPacketValueSeparator(string previous, string next, TextSerializerContext context)
        {
            return $"{previous}{Environment.NewLine}{next}";
        }
    }
}
