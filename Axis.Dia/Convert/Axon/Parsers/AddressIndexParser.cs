using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon.Parsers
{
    internal static class AddressIndexParser
    {
        public static IResult<int> Parse(CSTNode addressIndexNode)
        {
            ArgumentNullException.ThrowIfNull(addressIndexNode);

            return Result.Of(() => int.Parse(
                addressIndexNode.TokenValue()[2..],
                System.Globalization.NumberStyles.HexNumber));
        }
    }
}
