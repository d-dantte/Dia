using Axis.Dia.Contracts;
using Axis.Dia.Convert.Axon.Parsers;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Convert.Axon
{
    public static class AxonSerializer
    {
        #region SymbolNames
        public const string SymbolNamePacket = "packet";
        public const string SymbolNameDiaValue = "dia-value";
        public const string SymbolNameDiaRef = "dia-ref";
        public const string SymbolNameAddressIndex = "address-index";
        public const string SymbolNameValueAddress = "value-address";
        #endregion

        #region Packet
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IResult<string> SerializePacket(
            SerializerContext? context,
            params IDiaValue[] values)
            => SerializePacket(ValuePacket.Of(values), context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static IResult<string> SerializePacket(ValuePacket packet, SerializerContext? context = null)
        {
            context ??= new SerializerContext();
            return packet.Values
                .Select(v => SerializeValue(v, context))
                .Fold()
                .Map(valueTexts => valueTexts.Aggregate(
                    string.Empty,
                    (prev, next) => ApplyPacketValueSeparator(prev, next)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diaText"></param>
        /// <returns></returns>
        public static IResult<ValuePacket> ParsePacket(string text, ParserContext? context = null)
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
        public static IResult<ValuePacket> ParsePacket(CSTNode node, ParserContext? context = null)
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
        /// Serialize the given value
        /// </summary>
        /// <param name="value">the value to serialize</param>
        /// <param name="context">the context governing the serialization</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<string> SerializeValue(IDiaValue value, SerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(value);
            ReferenceUtil.LinkReferences(value, out var linkedReferences);

            context ??= new SerializerContext();
            context.Value.BuildAddressIndices(linkedReferences!);

            return InternalSerializeValue(value, context ?? new SerializerContext());
        }

        internal static IResult<string> InternalSerializeValue(IDiaValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);

            try
            {
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
                    ReferenceValue refValue => ReferenceParser.Serialize(refValue, context),
                    _ => throw new InvalidOperationException($"Invalid Dia Type: {value.GetType()}")
                };
            }
            catch(Exception e)
            {
                return Result.Of<string>(e);
            }
        }

        /// <summary>
        /// Parse the given text into the appropriate <see cref="IDiaValue"/>
        /// </summary>
        /// <param name="text">the text to parse</param>
        /// <param name="context">the parser context</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<IDiaValue> ParseValue(string text, ParserContext? context = null)
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
        /// Parse the given <see cref="CSTNode"/>. The node must conform to the axon grammar.
        /// </summary>
        /// <param name="valueNode">The node</param>
        /// <returns>The parse result</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<IDiaValue> ParseValue(CSTNode valueNode, ParserContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(valueNode);

            return InternalParseValue(valueNode, context ?? new ParserContext());
        }


        internal static IResult<IDiaValue> InternalParseValue(CSTNode valueNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(valueNode);

            return valueNode.SymbolName switch
            {
                SymbolNameDiaValue => InternalParseValue(valueNode.FirstNode(), context),
                BoolParser.SymbolNameDiaBool => BoolParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                IntParser.SymbolNameDiaInt => IntParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                DecimalParser.SymbolNameDiaDecimal => DecimalParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                InstantParser.SymbolNameDiaInstant => InstantParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                StringParser.SymbolNameDiaString => StringParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                SymbolParser.SymbolNameDiaSymbol => SymbolParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                BlobParser.SymbolNameDiaBlob => BlobParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                ClobParser.SymbolNameDiaClob => ClobParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                ListParser.SymbolNameDiaList => ListParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                RecordParser.SymbolNameDiaRecord => RecordParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                ReferenceParser.SymbolNameDiaRef => ReferenceParser.Parse(valueNode, context).MapAs<IDiaValue>(),
                _ => throw new ArgumentException($"Invalid Root value symbol name: {valueNode.SymbolName}")
            };
        }

        #endregion

        private static string ApplyPacketValueSeparator(string previous, string next)
        {
            return $"{previous}{Environment.NewLine}{next}";
        }
    }
}
