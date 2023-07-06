namespace Axis.Dia.IO.Binary
{
    /// <summary>
    /// This exception signals that the total number of bytes expected to be read were not read before encountering 
    /// the end of the stream.
    /// </summary>
    public class PartialReadException : Exception
    {
        public byte[] PartialData { get; }

        public PartialReadException(byte[] partialData)
        : base($"Expected byte-count could not be read from the stream. Partial read count: {partialData?.Length ?? -1}")
        {
            PartialData = partialData ?? throw new ArgumentNullException(nameof(partialData));
        }
    }
}
