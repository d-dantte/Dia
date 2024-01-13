using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Common.Numerics;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using System.Numerics;

namespace Axis.Dia.Convert.Json.Parser
{
    internal class NumberParser :
        IRootSymbolProvider,
        IJsonConverter<IDiaValue>
    {
        internal const string SymbolNameNumberValue = "number-value";
        internal const string SymbolNameScientificDecimal = "scientific-decimal";
        internal const string SymbolNameRegularDecimal = "regular-decimal";
        internal const string SymbolNameIntNumber= "int-number";

        public static string RootSymbol => SymbolNameNumberValue;

        public static IResult<IDiaValue> Parse(CSTNode numberNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(numberNode);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (!SymbolNameNumberValue.Equals(numberNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol '{numberNode.SymbolName}', expected '{SymbolNameNumberValue}'");

            var numberImplNode = numberNode.FirstNode();
            return numberImplNode?.SymbolName switch
            {
                SymbolNameIntNumber => Result
                    .Of(numberImplNode.TokenValue())
                    .Map(BigInteger.Parse)
                    .Map(v => IntValue.Of(v))
                    .MapAs<IDiaValue>(),

                SymbolNameScientificDecimal
                or SymbolNameRegularDecimal => Result
                    .Of(numberImplNode.TokenValue())
                    .Bind(BigDecimal.Parse)
                    .Map(v => DecimalValue.Of(v))
                    .MapAs<IDiaValue>(),

                _ => Result.Of<IDiaValue>(new InvalidOperationException(
                    $"Invalid symbol '{numberImplNode?.SymbolName ?? "null"}'"))
            };
        }

        public static IResult<string> Serialize(IDiaValue value, SerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(value);
            context.ThrowIfDefault(_ => new ArgumentException($"Invalid {nameof(context)}: default"));

            if (value.IsNull || value.HasAnnotations())
                return Result.Of<string>(new InvalidOperationException(
                    $"Invalid value. IsNull: '{value.IsNull}', HasAnnotations: '{value.HasAnnotations()}'"));

            return value switch
            {
                IntValue @int => Result.Of(@int.Value!.Value.ToString()),
                DecimalValue @decimal => Result.Of(context.Options.Decimals.UseExponentNotation switch
                {
                    true => @decimal.Value!.Value.ToScientificString(context.Options.Decimals.MaxPrecision),
                    false => @decimal.Value!.Value.ToNonScientificString(context.Options.Decimals.MaxPrecision)
                }),
                _ => throw new InvalidOperationException($"Invalid dia value type: '{value.Type}'")
            };
        }
    }
}
