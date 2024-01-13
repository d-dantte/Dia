using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class StringParser :
        IRootSymbolProvider,
        IJsonConverter<StringValue>
    {
        internal const string SymbolNameStringValue = "string-value";
        internal const string SymbolNameValueContent = "value-content";

        public static string RootSymbol => SymbolNameStringValue;

        public static IResult<StringValue> Parse(CSTNode stringNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(stringNode);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (!SymbolNameStringValue.Equals(stringNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol '{stringNode.SymbolName}', expected '{SymbolNameStringValue}'");

            return stringNode
                .FindNodes(SymbolNameValueContent)
                .FirstOrOptional()
                .Map(node => node.TokenValue())
                .Map(EscapeSequenceGroup.JsonEscapeGroup.Unescape)
                .AsResult()
                .Map(StringValue.Of);
        }

        public static IResult<string> Serialize(StringValue value, SerializerContext context)
        {
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (value.IsNull || value.HasAnnotations())
                return Result.Of<string>(new InvalidOperationException(
                    $"Invalid value. IsNull: '{value.IsNull}', HasAnnotations: '{value.HasAnnotations()}'"));

            return EscapeSequenceGroup.JsonEscapeGroup
                .Escape(value.Value)
                .WrapIn("\"")
                .ApplyTo(Result.Of);
        }
    }
}
