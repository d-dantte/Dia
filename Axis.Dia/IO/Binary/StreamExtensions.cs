using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.IO.Binary
{
    public static class StreamExtensions
    {
        #region Read single byte
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
        #endregion


        #region Read Exact Bytes
        public static IResult<byte[]> ReadExactBytesResult(this Stream stream, int byteCount)
        {
            return Result.Of(() =>
            {
                var bytes = new byte[byteCount];
                var readCount = stream.Read(bytes);
                return
                    readCount == byteCount ? bytes :
                    readCount == 0 ? throw new EndOfStreamException() :
                    throw new PartialReadException(bytes[0..readCount]);
            });
        }

        public static bool TryReadExactBytesResult(
            this Stream stream,
            int byteCount,
            out IResult<byte[]> result)
            => (result = ReadExactBytesResult(stream, byteCount)) is IResult<byte[]>.DataResult;
        #endregion


        #region Read VarBytes
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
                        if (bytes.Count == 0)
                            throw new EndOfStreamException();

                        else throw new PartialReadException(bytes.ToArray());
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
        #endregion


        //private static readonly string PartialDataKey = "Axis.Dia.IO.Binary.VarBytePartialData";
        //public static byte[]? PartialData(this EndOfStreamException exception)
        //{
        //    ArgumentNullException.ThrowIfNull(exception);

        //    return exception.Data.TryGetValue(PartialDataKey, out var data)
        //        ? (byte[])data
        //        : null;
        //}
    }
}
