using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class BoolParser :
        IRootSymbolProvider,
        IJsonConverter<BoolValue>
    {
        internal const string SymbolNameBoolValue = "bool-value";

        public static string RootSymbol => SymbolNameBoolValue;

        public static IResult<BoolValue> Parse(CSTNode boolNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(boolNode);
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            return boolNode.SymbolName switch
            {
                SymbolNameBoolValue => Result
                    .Of(boolNode.TokenValue)
                    .Map(bool.Parse)
                    .Map(v => BoolValue.Of(v)),

                _ => Result.Of<BoolValue>(new InvalidOperationException(
                    $"Invalid symbol name: '{boolNode.SymbolName}', expected '{SymbolNameBoolValue}'"))
            };
        }

        public static IResult<string> Serialize(BoolValue value, SerializerContext context)
        {
            context.ThrowIfDefault(new ArgumentException($"Invalid {nameof(context)} instance"));

            if (value.IsNull || value.HasAnnotations())
                return Result.Of<string>(new InvalidOperationException(
                    $"Invalid value. IsNull: '{value.IsNull}', HasAnnotations: '{value.HasAnnotations()}'"));

            return Result.Of(() => value.Value!.Value.ToString().ToLower());
        }
    }
}
