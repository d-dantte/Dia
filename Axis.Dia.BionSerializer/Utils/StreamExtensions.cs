using Axis.Luna.Extensions;

namespace Axis.Dia.BionSerializer.Utils
{
    internal static class StreamExtensions
    {
        #region Read single byte
        internal static byte ReadByte(this Stream stream)
        {
            if (TryReadByte(stream, out var byteResult))
                return byteResult;

            throw new EndOfStreamException();
        }

        internal static bool TryReadByte(
            this Stream stream,
            out byte result)
        {
            ArgumentNullException.ThrowIfNull(stream);

            (result, bool @return) = stream.ReadByte() switch
            {
                -1 => ((byte)0, false),
                int x => ((byte)x, true)
            };

            return @return;
        }
        #endregion

        #region Read VarBytes
        internal static VarBytes ReadVarBytes(this Stream stream)
        {
            _ = TryReadVarBytes(stream, out var result);
            return result;
        }

        internal static bool TryReadVarBytes(
            this Stream stream,
            out VarBytes result)
        {
            int data;
            var bytes = new List<byte>();
            result = default;

            do
            {
                data = stream.ReadByte();

                if (data < 0)
                    return false;

                bytes.Add((byte)data);
            }
            while (bytes[^1].IsSet(7));

            result = VarBytes.Of(bytes, false);
            return true;
        }
        #endregion

        #region Read ByteChunks
        internal static ByteChunks ReadChunks(this Stream stream)
        {
            _ = TryReadChunks(stream, out var result);
            return result;
        }

        internal static bool TryReadChunks(this Stream stream, out ByteChunks result)
        {
            ArgumentNullException.ThrowIfNull(stream);

            result = default;
            var chunks = new List<byte[]>();

            while (stream.TryReadChunkHeader(out var header))
            {
                // evaluate byte count
                var byteCount = header.IsOVerflow switch
                {
                    true => ByteChunks.MaxSectionDataCount,
                    _ => header.HeaderValue
                };

                // create the buffer array
                var data = new byte[byteCount + 2];

                // set the header bytes
                data[0] = header.Header[0];
                data[1] = header.Header[1];

                // copy the data bytes
                var count = stream.Read(data, 2, byteCount);

                // validate
                if (count != byteCount)
                    return false;

                // persist
                chunks.Add(data);

                if (!header.IsOVerflow)
                    break;
            }

            result = chunks
                .SelectMany()
                .ToArray()
                .ApplyTo(bytes => ByteChunks.Of(bytes, false));
            return true;
        }

        internal static bool TryReadChunkHeader(
            this Stream stream,
            out (byte[] Header, ushort HeaderValue, bool IsOVerflow) header)
        {
            var position = stream.Position;

            header = default;
            var headerBytes = new byte[2];
            int count = stream.Read(headerBytes, 0, 2);

            if (count != 2)
            {
                if (stream.CanSeek)
                    stream.Seek(position, SeekOrigin.Begin);

                return false;
            }

            var byteCount = BitConverter.ToUInt16(headerBytes, 0);
            header = (
                headerBytes,
                byteCount,
                byteCount switch
                {
                    ByteChunks.HeaderOverflow => true,
                    _ => false
                });
            return true;
        }
        #endregion
    }
}
