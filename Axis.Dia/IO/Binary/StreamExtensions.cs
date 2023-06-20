using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.IO.Binary
{
    public static class StreamExtensions
    {
        public static IResult<byte> ReadByteResult(this Stream stream)
        {
            return Result
                .Of(() => stream.ReadByte())
                .Map(@byte => @byte >= 0 ? (byte)@byte : throw new EndOfStreamException());
        }

        public static bool TryReadByteResult(
            this Stream stream,
            out IResult<byte> result)
            => (result = ReadByteResult(stream)) is IResult<byte>.DataResult;


        public static IResult<byte[]> ReadExactBytesResult(this Stream stream, int byteCount)
        {
            return Result.Of(() =>
            {
                var bytes = new byte[byteCount];
                var readCount = stream.Read(bytes);
                return readCount != byteCount
                    ? throw new EndOfStreamException()
                    : bytes;
            });
        }

        public static bool TryReadExactBytesrESULT(
            this Stream stream,
            int byteCount,
            out IResult<byte[]> result)
            => (result = ReadExactBytesResult(stream, byteCount)) is IResult<byte[]>.DataResult;


        public static IResult<VarBytes> ReadVarBytesResult(this Stream stream)
        {
            return Result.Of(() =>
            {
                int data;
                var bytes = new List<byte>();

                do
                {
                    data = stream.ReadByte();

                    if (data < 0)
                    {
                        var ex = new EndOfStreamException();

                        if (bytes.Count > 0)
                            ex.Data[PartialDataKey] = bytes.ToArray();

                        throw ex;
                    }

                    bytes.Add((byte)data);
                }
                while (bytes[^1].IsSet(7));

                return VarBytes.Of(bytes, false);
            });
        }

        public static bool TryReadVarBytesResult(
            this Stream stream,
            out IResult<VarBytes> result)
            => (result = ReadVarBytesResult(stream)) is IResult<VarBytes>.DataResult;


        private static readonly string PartialDataKey = "Axis.Dia.IO.Binary.VarBytePartialData";
        public static byte[]? PartialData(this EndOfStreamException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return exception.Data.TryGetValue(PartialDataKey, out var data)
                ? (byte[])data
                : null;
        }
    }
}
