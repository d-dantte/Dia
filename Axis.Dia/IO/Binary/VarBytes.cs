using Axis.Dia.Utils;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Axis.Dia.IO.Binary
{    
    /// <summary>
    /// 
    /// 
    /// </summary>
    public readonly struct VarBytes :
        IEnumerable<byte>,
        IDefaultValueProvider<VarBytes>
    {
        #region Fields
        private readonly BitSequence _data;
        #endregion

        #region Properties
        public int Length
        {
            get
            {
                if (_data.Length < 0)
                    return -1;

                var divrem = Math.DivRem(_data.Length, 8);
                return divrem.Quotient + (divrem.Remainder > 0 ? 1 : 0);
            }
        }
        #endregion

        #region Construction
        private VarBytes(BitSequence data, bool isRawData)
        {
            _data = isRawData
                ? AddOverflow(data)
                : ValidateVarByte(data.ToByteArray());
        }

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="bits">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(BitSequence bits, bool isRawData = true) => new VarBytes(bits, isRawData);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="byteArray">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(IEnumerable<byte> byteArray, bool isRawData = true) => new VarBytes(byteArray.ToArray(), isRawData);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="byteArray">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(byte[] byteArray, bool isRawData) => new VarBytes(byteArray, isRawData);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="rawByteArray"></param>
        /// <returns></returns>
        public static VarBytes Of(byte[] rawByteArray) => Of(rawByteArray, true);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="byteArray">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(ArraySegment<byte> byteArray, bool isRawData = true) => new VarBytes(byteArray.ToArray(), isRawData);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="byteArray">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(Span<byte> byteArray, bool isRawData = true) => new VarBytes(byteArray.ToArray(), isRawData);
        #endregion

        #region Implicits
        public static implicit operator VarBytes(BitSequence bits) => Of(bits);
        public static implicit operator VarBytes(byte[] byteArray) => Of(byteArray);
        public static implicit operator VarBytes(ArraySegment<byte> byteArray) => Of(byteArray);
        public static implicit operator VarBytes(Span<byte> byteArray) => Of(byteArray);
        #endregion

        #region Indexers and Slicers
        public byte this[int index]
        {
            get
            {
                ValidateState();
                return _data.ByteAt(index * 8);
            }
        }

        public byte[] Slice(int index, int length)
        {
            ValidateState();
            return _data
                .ToByteArray()
                .Slice(index, length)
                .ToArray();
        }
        #endregion

        #region DefaultProvider
        public bool IsDefault => _data.IsDefault;

        public static VarBytes Default => default;
        #endregion

        #region overrides
        public override string ToString() => $"VarBytes {_data}";

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is VarBytes other
                && _data.Equals(other._data);
        }

        public override int GetHashCode() => _data.GetHashCode();

        public static bool operator ==(VarBytes a, VarBytes b) => a.Equals(b);
        public static bool operator !=(VarBytes a, VarBytes b) => !a.Equals(b);
        #endregion

        #region API
        /// <summary>
        /// Converts this var-bytes into a byte array, removing the overflow bits, and stitching
        /// the original data back together.
        /// </summary>
        /// <returns>the byte array</returns>
        public byte[]? ToByteArray()
        {
            if (IsDefault)
                return null;

            return _data
                .ApplyTo(RemoveOverflow)
                .ToByteArray();
        }
        #endregion

        #region IEnumerable
        public IEnumerator<byte> GetEnumerator()
        {
            ValidateState();

            return ((IEnumerable<byte>)_data.ToByteArray()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region Helpers
        private static byte[] ValidateVarByte(byte[] data)
        {
            if (IsVarByteArray(data))
                return data;

            else throw new ArgumentException("Invalid VarByte array");
        }

        private void ValidateState()
        {
            if (IsDefault)
                throw new InvalidOperationException("VarBytes is in invalid state");
        }

        /// <summary>
        /// splits the bit array into groups of 7 bits, inserting a 1 bit between consecutive bit groups,
        /// or a zero at the last bit of the last byte, if "count" is a multiple of 7.
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        private static BitSequence AddOverflow(BitSequence bits)
        {
            return BitSequence.Of(AddOverflowBits(bits));
        }

        private static IEnumerable<bool> AddOverflowBits(BitSequence bits)
        {
            int count = 0;
            var lastBitIndex = bits.Length - 1;
            for (int cnt = 0; cnt < bits.Length; cnt++)
            {
                yield return bits[cnt];
                count++;

                if (count == 7)
                {
                    yield return cnt < lastBitIndex;
                    count = 0;
                }
            }
        }

        private static BitSequence RemoveOverflow(BitSequence bits)
        {
            return BitSequence.Of(bits.SkipEveryNth(8));
        }

        private static BitSequence AppendMagicByte(BitSequence bits)
        {
            var (_, remainder) = Math.DivRem(bits.Length, 8);

            var lastBits = bits[^remainder..];

            // the last bit is a full byte with overflow bit set
            if (lastBits.Length == 8 && lastBits[7])
                return bits.AppendBits(0);

            return bits;
        }

        //private static BitSequence RemoveMagicByte(BitSequence bits)
        //{
        //    var (_, remainder) = Math.DivRem(bits.Length, 8);

        //    var lastBits = bits[^remainder..];

        //    if (lastBits.All(b => !b))
        //        return bits[..^remainder];

        //    return bits;
        //}


        /// <summary>
        /// validate the data - make sure no byte follows a previous byte with its overflow bit unset.
        /// <para>
        /// Note: being the last byte and having the overflow set means that a zero byte is the actual last byte.
        /// encoding it this way saves an additional byte from being used.
        /// </para>
        /// </summary>
        /// <param name="data">the byte array to validate</param>
        public static bool IsVarByteArray(byte[] data)
        {
            if (data is null)
                return false;

            var lastByte = data.Length - 1;
            for (int cnt = 0; cnt < data.Length; cnt++)
            {
                var isLastByteIndex = cnt == lastByte;
                var hasOverflow = data[cnt].IsSet(7);

                if (!hasOverflow && !isLastByteIndex)
                    return false;
            }

            return true;
        }
        #endregion
    }
}
