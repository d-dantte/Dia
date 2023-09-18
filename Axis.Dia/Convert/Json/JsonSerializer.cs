using Axis.Dia.Contracts;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json
{
    public static class JsonSerializer
    {
        public static IResult<string> Serialize(IDiaValue value, SerializerContext? context = null)
        {
            throw new NotImplementedException();
        }

        public static IResult<IDiaValue> Parse(string text, ParserContext? context = null)
        {
            throw new NotImplementedException();
        }



        internal static IResult<IDiaValue> ParseValue(CSTNode jsonValueNode, ParserContext context)
        {
            throw new NotImplementedException();
        }
    }
}
