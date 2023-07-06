using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;

namespace Axis.Dia.IO.Binary
{
    /// <summary>
    /// Represents the inability to successfully deserialize a <see cref="IDiaValue"/>. Deserialization happens in 2 phases:
    /// <list type="number">
    ///     <item>
    ///         Deserializing the <see cref="TypeMetadata"/>. A failure at this stage due to an <see cref="EndOfStreamException"/> means
    ///         there is no more data to even begin the deserialization of a value. A failure due to any other exception, e.g
    ///         <see cref="PartialReadException"/>, is interpreted as a failure due to a <see cref="ValueDeserializationException"/>,
    ///         and transformed accordingly.
    ///     </item>
    ///     <item>
    ///         Using the <see cref="TypeMetadata"/> to deserialize the <see cref="IDiaValue"/>. Any failure at this stage is interpreted
    ///         as a failure due to <see cref="ValueDeserializationException"/> and transformed accordingly.
    ///     </item>
    /// </list>
    /// </summary>
    public class ValueDeserializationException: Exception
    {
        public ValueDeserializationException(Exception cause)
        : base("Deserialziation failed.", cause)
        {
        }
    }
}
