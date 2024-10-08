using Axis.Dia.BionSerializer.Utils;
using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Serializers
{
    public class ByteBuffer
    {
        private readonly MemoryStream memoryStream = new();

        internal Stream Stream => memoryStream;

        internal byte[] StreamData => memoryStream.ToArray();

        public long Count => memoryStream.Length;

        public ByteBuffer Write(byte[] data, int offset, int count)
        {
            ArgumentNullException.ThrowIfNull(nameof(data));

            memoryStream.Write(data, offset, count);
            return this;
        }

        public ByteBuffer Write(byte[] data) => Write(
            data: data.ThrowIfNull(() => new ArgumentNullException(nameof(data))),
            offset: 0,
            count: data.Length);

        public ByteBuffer Write(
            ReadOnlySpan<byte> data,
            int offset,
            int count)
            => Write(data[offset..(offset + count)]);

        public ByteBuffer Write(ReadOnlySpan<byte> data)
        {
            memoryStream.Write(data);
            return this;
        }

        public ByteBuffer Write(ByteChunks chunks) => Write(chunks.ToByteArray());

        public ByteBuffer Write(VarBytes varbytes) => Write(varbytes.ToByteArray());
    }
}
