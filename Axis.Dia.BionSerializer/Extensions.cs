using Axis.Dia.BionSerializer.Utils;
using Axis.Luna.Extensions;
using System.Numerics;

namespace Axis.Dia.BionSerializer
{
    internal static class Extensions
    {
        public static VarBytes ToVarBytes(this int value, bool useSignificantBits = true)
        {
            return value
                .As<BigInteger>()
                .ToVarBytes(useSignificantBits);
        }

        public static VarBytes ToVarBytes(this long value, bool useSignificantBits = true)
        {
            return value
                .As<BigInteger>()
                .ToVarBytes(useSignificantBits);
        }
    }
}
