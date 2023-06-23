using Axis.Luna.Extensions;

namespace Axis.Dia.IO.Binary
{
    /// <summary>
    /// When a byte array is supplied to the "Deserialization" process, it is expected that the entirety of the array
    /// MUST be used in creating the intended type. If the intended type is created, and all of the bytes aren't used,
    /// this exception is thrown - an indication that some runtime miscalculation has occured.
    /// </summary>
    public class PartialConsumptionException: Exception
    {
        public long ConsumedByteCount { get; }

        public long TotalByteCount { get; }

        public PartialConsumptionException(long consumedByteCount, long totalByteCount)
        : base($"The was only partially consumed. Expected consumption: {totalByteCount}, Actual consumption: {consumedByteCount}")
        {
            ConsumedByteCount = consumedByteCount.ThrowIf(
                c => c < 0,
                _ => new ArgumentOutOfRangeException(nameof(consumedByteCount)));

            TotalByteCount = totalByteCount.ThrowIf(
                c => c <= 0,
                _ => new ArgumentOutOfRangeException(nameof(totalByteCount)));
        }
    }
}
