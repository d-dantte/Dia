using System.Numerics;

namespace Axis.Dia.BionSerializer.Utils
{
    public static class Extensions
    {
        public static IEnumerable<BigInteger> Repeat(this BigInteger count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            for (var index = BigInteger.Zero; index < count; index++)
            {
                yield return index;
            }
        }
    }
}
