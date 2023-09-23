using Axis.Dia.Contracts;
using Axis.Dia.Convert.Json.Parser;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Recognizers.Results;

namespace Axis.Dia.Convert.Json
{
    public static class JsonSerializer
    {
        internal const string SymbolNameRoot = "root";
        internal const string SymbolNameBoolValue = "bool-value";
        internal const string SymbolNameNumberValue = "number-value";
        internal const string SymbolNameStringValue = "string-value";
        internal const string SymbolNameNull = "null";
        internal const string SymbolNameEncodedValue = "encoded-value";
        internal const string SymbolNameObject = "object";
        internal const string SymbolNameList = "array";

        #region Serialize
        public static IResult<string> Serialize(IDiaValue value, SerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(value);
            value.LinkReferences();

            context ??= new SerializerContext();
            context.Value.BuildAddressIndices(value);

            return InternalSerializeValue(value, context ?? new SerializerContext());
        }

        internal static IResult<string> InternalSerializeValue(IDiaValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(context);

            try
            {
                if (value is ListValue list)
                    return ListParser.Serialize(list, context);

                else if (value is RecordValue record)
                    return RecordParser.Serialize(record, context);

                else if (value.IsNull || value.HasAnnotations())
                    return EncodedValueParser.Serialize(value, context);

                else
                {
                    return value switch
                    {
                        InstantValue
                        or ClobValue
                        or BlobValue
                        or SymbolValue
                        or ReferenceValue => EncodedValueParser.Serialize(value, context),

                        DecimalValue or IntValue => NumberParser.Serialize(value, context),

                        BoolValue boolValue => BoolParser.Serialize(boolValue, context),
                        StringValue stringValue => StringParser.Serialize(stringValue, context),

                        _ => throw new InvalidOperationException($"Invalid Dia Type: {value.GetType()}")
                    };
                }
            }
            catch (Exception e)
            {
                return Result.Of<string>(e);
            }
        }
        #endregion

        #region Parse
        public static IResult<IDiaValue> Parse(string text, ParserContext? context = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"Invalid text: '{text}'");

            var parseResult = GrammarUtil.Grammar
                .GetRecognizer(SymbolNameRoot)
                .Recognize(text);

            var result = parseResult switch
            {
                SuccessResult success => ParseValue(success.Symbol.FirstNode(), context ?? new()),
                null => Result.Of<IDiaValue>(new Exception("Unknown Error")),
                _ => Result.Of<IDiaValue>(new ParseException(parseResult))
            };

            return result.Map(value => value.LinkReferences());
        }


        internal static IResult<IDiaValue> ParseValue(CSTNode jsonValueNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(jsonValueNode);
            ArgumentNullException.ThrowIfNull(context);

            var typeValue = jsonValueNode.FirstNode();
            return typeValue.SymbolName switch
            {
                SymbolNameNull => NullParser.Parse(typeValue, context),
                SymbolNameBoolValue => BoolParser.Parse(typeValue, context).MapAs<IDiaValue>(),
                SymbolNameNumberValue => NumberParser.Parse(typeValue, context),
                SymbolNameStringValue => StringParser.Parse(typeValue, context).MapAs<IDiaValue>(),
                SymbolNameEncodedValue => EncodedValueParser.Parse(typeValue, context),
                SymbolNameList => ListParser.Parse(typeValue, context).MapAs<IDiaValue>(),
                SymbolNameObject => RecordParser.Parse(typeValue, context).MapAs<IDiaValue>(),

                _ => Result.Of<IDiaValue>(new FormatException(
                    $"Invalid JSON value: {jsonValueNode.TokenValue()}",
                    new InvalidOperationException($"Invalid json-value symbol: '{jsonValueNode.SymbolName}'.")))
            };
        }
        #endregion
    }
}
