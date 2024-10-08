using Axis.Luna.BitSequence;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.BionSerializer.Utils
{
    /// <summary>
    /// Immutable self-contained sequence of bytes that has it's length encoded within it's byte sequence.
    /// <para/>
    /// A chunk is split into sections, each with a 2-byte header at the start. The header represents the number of bytes in that section.
    /// </summary>
    public readonly struct ByteChunks:
        IEnumerable<byte>,
        IEquatable<ByteChunks>,
        IDefaultValueProvider<ByteChunks>
    {
        internal const ushort HeaderOverflow = ushort.MaxValue;
        internal const int MaxSectionDataCount = ushort.MaxValue - 1;
        internal const int MaxSectionCount = ushort.MaxValue + 1;

        #region Fields
        private readonly byte[] _data;
        #endregion

        #region Local Properties
        public int Length => _data?.Length ?? 0;

        public int RawLength
        {
            get
            {
                if (IsDefault)
                    return 0;

                var headers = GetHeaderInfo(_data);
                return ((headers.Length - 1) * MaxSectionDataCount) + headers[^1].Value;
            }
        }

        public bool IsEmpty => Length == 2;

        public static ByteChunks Empty => new([]);

        public byte[] ToByteArray()
        {
            var copy = new byte[Length];

            if (!IsDefault || !IsEmpty)
                Array.Copy(_data, copy, copy.Length);

            return copy;
        }
        #endregion

        #region DefaultValueProvider

        public static ByteChunks Default => default;

        public bool IsDefault => _data is null;
        #endregion

        #region IEnumerable
        public IEnumerator<byte> GetEnumerator() => IsDefault switch
        {
            true => Enumerable.Empty<byte>().GetEnumerator(),
            false => _data.AsEnumerable().GetEnumerator()
        };

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        #endregion

        #region API
        public byte this[int index]
        {
            get
            {
                if (IsDefault)
                    throw new InvalidOperationException("Invalid byte-chunk: default");

                return _data[index];
            }
        }

        public byte[] Slice(int index, int length)
        {
            return (IsDefault, index, length) switch
            {
                (true, 0, 0) => [],
                (true, _, _) => throw new InvalidOperationException("Invalid byte-chunk: default"),
                _ => _data
                    .AsSpan()
                    .Slice(index, length)
                    .ToArray()
            };
        }

        public byte[] ToRawBytes()
        {
            if (IsDefault)
                return [];

            var array = new byte[RawLength];
            var index = 0;
            var headers = GetHeaderInfo(_data);
            for (var cnt = 0; cnt < headers.Length; cnt++)
            {
                var header = headers[cnt];
                var isLastHeader = cnt.Equals(headers.Length - 1);
                var count = isLastHeader ? header.Value : MaxSectionDataCount;

                Array.Copy(_data, header.Index + 2, array, index, count);
                index += count;
            }
            return array;
        }

        public bool ValueEquals(ByteChunks other)
        {
            return (IsDefault, other.IsDefault) switch
            {
                (true, true) => true,
                (false, false) => _data.SequenceEqual(other._data),
                _ => false
            };
        }

        public IEnumerable<byte[]> Chunks
        {
            get
            {
                if(IsDefault || IsEmpty)
                    return [];

                var data = _data;
                return ByteChunks
                    .GetHeaderInfo(data)
                    .Select(info =>
                    {
                        var dataIndex = info.Index + 2;
                        return data[dataIndex..(dataIndex + info.Value)];
                    });
            }
        }

        #endregion

        #region Overrides
        public override string ToString()
        {
            var text = IsDefault ? "*" : $"{BitSequence.Of(_data.Take(4).ToArray())}";

            if (RawLength > 2)
                text = "[..., " + text[1..];

            return $"ByteChunk{{{text}}}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_data?.GetHashCode() ?? 0);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is ByteChunks other && Equals(other);
        }

        public bool Equals(ByteChunks other)
        {
            return _data == other._data;
        }

        public static bool operator ==(ByteChunks first, ByteChunks second) => first.Equals(second);
        public static bool operator !=(ByteChunks first, ByteChunks second) => !first.Equals(second);
        #endregion

        #region Internals
        private static (int Index, ushort Value, bool IsOVerflow)[] GetHeaderInfo(byte[]? data)
        {
            if (data is null)
                return [];

            return ByteChunks
                .GetHeaderIndexes(data.Length)
                .Select(index => (Index: index, Value: BitConverter.ToUInt16(data, index)))
                .Select(info =>(info.Index, info.Value, IsOverflow: info.Value switch
                {
                    HeaderOverflow => true,
                    _ => false
                }))
                .ToArray();
        }

        //private static bool IsHeaderIndex(int index)
        //{
        //    return (index % MaxSectionCount) switch
        //    {
        //        0 or 1 => true,
        //        _ => false
        //    };
        //}

        private static int[] GetHeaderIndexes(int chunkLength)
        {
            var indexes = new int[GetHeaderCount(chunkLength)];
            for (int cnt = 0; cnt < indexes.Length; cnt++)
                indexes[cnt] = cnt * MaxSectionCount;

            return indexes;
        }

        private static int GetHeaderCount(int chunkLength)
        {
            if (chunkLength == 0)
                return 1;

            var result = Math.DivRem(chunkLength, MaxSectionCount);
            return result.Quotient + result.Remainder switch
            {
                0 => 0,
                _ => 1
            };
        }

        private static int GetRawSectionCount(int rawLength, out int lastChunkLength)
        {
            var result = Math.DivRem(rawLength, MaxSectionDataCount);

            lastChunkLength = result.Remainder switch
            {
                0 => MaxSectionDataCount,
                _ => result.Remainder
            };

            return result.Quotient + result.Remainder switch
            {
                0 => 0,
                _ => 1
            };
        }
        #endregion

        #region Construction
        public ByteChunks(params byte[] rawBytes)
            : this(rawBytes, true)
        { }

        public ByteChunks(byte[] bytes, bool isRawBytes)
        {
            _data = isRawBytes switch
            {
                true => InitializeRaw(bytes),
                false => ValidateChunk(bytes)
            };
        }

        public ByteChunks(
            IEnumerable<byte> bytes,
            bool isRawBytes = true)
            : this(bytes.ToArray(), isRawBytes)
        { }

        public ByteChunks(
            BitSequence bits,
            bool isRawBytes = true)
            : this(bits.ToByteArray(), isRawBytes)
        { }

        public static ByteChunks Of(params byte[] rawBytes) => new(rawBytes);

        public static ByteChunks Of(byte[] bytes, bool isRawBytes) => new(bytes, isRawBytes);

        public static ByteChunks Of(IEnumerable<byte> bytes, bool isRawBytes = true) => new(bytes, isRawBytes);

        public static ByteChunks Of(BitSequence bits, bool isRawBytes = true) => new(bits, isRawBytes);

        #region Implicits
        public static implicit operator ByteChunks(BitSequence bits) => Of(bits);
        public static implicit operator ByteChunks(byte[] byteArray) => Of(byteArray);
        public static implicit operator ByteChunks(Span<byte> span) => Of(span.ToArray());
        #endregion

        private static byte[] InitializeRaw(byte[] rawBytes)
        {
            ArgumentNullException.ThrowIfNull(rawBytes);

            if (rawBytes.IsEmpty())
                return [0, 0]; // empty header

            var rawChunkCount = GetRawSectionCount(rawBytes.Length, out int lastChunkLength);
            byte[] data = new byte[((MaxSectionDataCount + 2) * (rawChunkCount - 1)) + 2 + lastChunkLength];

            for (int cnt = 0; cnt < rawChunkCount; cnt++)
            {
                var isLastChunk = cnt == rawChunkCount - 1;
                var sourceIndex = cnt * MaxSectionDataCount;
                var destinationIndex = cnt * MaxSectionCount;
                var destinationLength = Math.Min(MaxSectionDataCount, data.Length - (destinationIndex + 2));
                var headerValue = BitConverter.GetBytes(isLastChunk
                    ? (ushort)destinationLength
                    : HeaderOverflow);

                // Copy the header
                Array.Copy(headerValue!, 0, data, destinationIndex, 2);

                // Copy the data
                Array.Copy(rawBytes, sourceIndex, data, destinationIndex + 2, destinationLength);
            }

            return data;
        }

        private static byte[] ValidateChunk(byte[] encodedBytes)
        {
            var info = GetHeaderInfo(encodedBytes);
            var encodedRawByteCount = 0;
            for (int cnt = 0; cnt < info.Length; cnt++)
            {
                encodedRawByteCount += info[cnt].IsOVerflow switch
                {
                    true => MaxSectionDataCount,
                    _ => info[cnt].Value
                };

                if (cnt != info.Length - 1 && info[cnt].Value != HeaderOverflow)
                    throw new InvalidOperationException(
                        $"Invalid header: [header-count: {info.Length}, index: {cnt}, value: {info[cnt].Value}");
            }

            if (encodedBytes.Length != (encodedRawByteCount + (2 * info.Length)))
                throw new InvalidOperationException($"Invalid encoded bytes");

            return encodedBytes;
        }

        #endregion
    }
}
