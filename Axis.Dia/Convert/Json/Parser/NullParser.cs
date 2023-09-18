using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Json.Parser
{
    /// <summary>
    /// By default this is translated into a null record.
    /// </summary>
    internal class NullParser :
        IParser<IDiaValue>,
        IRootSymbolProvider
    {
        public static string RootSymbol => "null";

        public IResult<IDiaValue> Parse(CSTNode nullNode, ParserContext context)
        {
            ArgumentNullException.ThrowIfNull(nullNode);
            ArgumentNullException.ThrowIfNull(context);

            return Result.Of(RecordValue.Null()).MapAs<IDiaValue>();
        }
    }
}
