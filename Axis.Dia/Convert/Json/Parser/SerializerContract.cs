using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    internal interface ISerializer<TValue>
    {
        IResult<string> Serialize(TValue value, SerializerContext context);
    }

    internal interface IParser<TValue>
    {
        IResult<TValue> Parse(CSTNode jsonNode, ParserContext context);
    }

    internal interface IJsonConverter<TValue> : ISerializer<TValue>, IParser<TValue>
    {
    }

    internal interface IRootSymbolProvider
    {
        abstract static string RootSymbol { get; }
    }
}
