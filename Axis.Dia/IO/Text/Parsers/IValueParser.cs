using Axis.Dia.Contracts;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.IO.Text.Parsers
{
    public interface IValueSerializer<TDiaValue>
    where TDiaValue : IDiaValue
    {
        /// <summary>
        /// Serialize the given <see cref="TDiaValue"/> into it's string representation.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="context">The serializer context</param>
        /// <returns>The text representation of the supplied value</returns>
        abstract static IResult<string> Serialize(TDiaValue value, TextSerializerContext? context = null);

        /// <summary>
        /// Deserialize the given text into it's <see cref="TDiaValue"/> instance.
        /// </summary>
        /// <param name="symbolNode">The symbol node</param>
        /// <param name="context">The serializer contet</param>
        /// <returns>The <see cref="TDiaValue"/> instance.</returns>
        abstract static IResult<TDiaValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null);

        /// <summary>
        /// root-symbol name of the value's grammar production
        /// </summary>
        abstract static string GrammarSymbol { get; }
    }
}
