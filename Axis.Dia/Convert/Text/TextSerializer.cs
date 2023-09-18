using Axis.Dia.Contracts;
using Axis.Dia.Convert.Text.Parsers;
using Axis.Dia.Types;
using Axis.Dia.Utils;
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
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<string> SerializeValue(IDiaValue value, SerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(value);
            value.NormalizeReferences();

            context ??= new SerializerContext();
            context.TrackReferences(value);

            return InternalSerializeValue(value, context ?? new SerializerContext());
        }

        public static IResult<string> InternalSerializeValue(IDiaValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);

            try
            {
                var @ref = value switch
                {
                    IDiaAddressProvider addressProvider => ReferenceValue.Of(addressProvider),
                    IDiaReference reference => reference,
                    _ => throw new InvalidOperationException($"Invalid vaue found: '{value}'")
                };

                if (context.TryGetRefInfo(@ref, out var refInfo) && refInfo!.IsSerialized)
                    return Result.Of($"@{refInfo.Index}");

                else
                {
                    var result = @ref.Value switch
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
                        _ => throw new InvalidOperationException($"Invalid Dia Type: {value.GetType()}")
                    };

                    if (refInfo is not null && !refInfo.IsSerialized)
                    {
                        result = result.Map(text => $"#{refInfo.Index}::{text}");
                        refInfo.Serialized();
                    }

                    return result;
                }
            }
            catch(Exception e)
            {
                return Result.Of<string>(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="context"></param>
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<IDiaValue> ParseValue(CSTNode valueNode, ParserContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(valueNode);

            return InternalParseValue(valueNode, context ?? new ParserContext());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IResult<IDiaValue> InternalParseValue(CSTNode valueNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(valueNode);

            // extract refIndex from the CSTNode first, if it is a ReferenceValue node
            if (TryGetRefIndex(valueNode, out var index))
            {
                if (context.TryGetRefAddress(index, out var address))
                    return Result.Of<IDiaValue>(ReferenceValue.Of(address));

                else return Result.Of<IDiaValue>(new InvalidOperationException(
                    $"The supplied ref index has not been deserialized: '{index}'"));
            }
            else
            {
                var address = TryGetValueAddressIndex(valueNode, out index)
                    ? context.AllocateAddress(index)
                    : (Guid?)null;

                // relocate values to new address
                return valueNode.SymbolName switch
                {
                    SymbolNameDiaValue => InternalParseValue(valueNode.FirstNode(), context),
                    BoolParser.SymbolNameDiaBool => BoolParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    IntParser.SymbolNameDiaInt => IntParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    DecimalParser.SymbolNameDiaDecimal => DecimalParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    InstantParser.SymbolNameDiaInstant => InstantParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    StringParser.SymbolNameDiaString => StringParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    SymbolParser.SymbolNameDiaSymbol => SymbolParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    BlobParser.SymbolNameDiaBlob => BlobParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    ClobParser.SymbolNameDiaClob => ClobParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    ListParser.SymbolNameDiaList => ListParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    RecordParser.SymbolNameDiaRecord => RecordParser
                        .Parse(valueNode, context)
                        .Map(value => address is not null
                            ? value.RelocateValue(address.Value)
                            : value)
                        .MapAs<IDiaValue>(),

                    _ => throw new ArgumentException($"Invalid Root value symbol name: {valueNode.SymbolName}")
                };
            }
        }
        #endregion

        private static string ApplyPacketValueSeparator(string previous, string next)
        {
            return $"{previous}{Environment.NewLine}{next}";
        }

        /// <summary>
        /// Gets the ref index if this is a <see cref="ReferenceValue"/> node
        /// </summary>
        /// <param name="node">The "dia-value" node</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        private static bool TryGetRefIndex(CSTNode valueNode, out int index)
        {
            if (!SymbolNameDiaRef.Equals(valueNode.SymbolName))
            {
                index = -1;
                return false;
            }

            // else
            var addressIndexNode = valueNode
                .FindNodes(SymbolNameAddressIndex)
                .First();

            index = int.Parse(addressIndexNode.TokenValue());
            return true;
        }

        /// <summary>
        /// Gets the ref index if this is not a <see cref="ReferenceValue"/> node
        /// </summary>
        /// <param name="node">The "dia-value" node</param>
        /// <param name="index">The index</param>
        /// <returns></returns>
        private static bool TryGetValueAddressIndex(CSTNode node, out int index)
        {
            var valueNode = node.FirstNode();
            if (!SymbolNameValueAddress.Equals(valueNode.SymbolName))
            {
                index = -1;
                return false;
            }

            // else
            var addressIndexNode = valueNode
                .FindNodes(SymbolNameAddressIndex)
                .FirstOrDefault();

            if (addressIndexNode is not null)
            {
                index = int.Parse(addressIndexNode.TokenValue());
                return true;
            }
            else
            {
                index = -1;
                return false;
            }
        }
    }
}
