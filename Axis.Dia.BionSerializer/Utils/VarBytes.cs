﻿using Axis.Luna.BitSequence;
using Axis.Luna.Common;
using Axis.Luna.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Axis.Dia.Bion.Utils
{
    /// <summary>
    /// Structure that represents a variable number of consecutive bytes. Each byte has the 8th (index 7) byte designated as an overflow flag that if set, means another byte follows.
    /// </summary>
    public readonly struct VarBytes :
        IEnumerable<byte>,
        IEquatable<VarBytes>,
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
        /// <returns>The created instance</returns>
        public static VarBytes Of(BitSequence bits) => new(bits, true);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="bits">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(BitSequence bits, bool isRawData = true) => new(bits, isRawData);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="byteArray">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(IEnumerable<byte> byteArray, bool isRawData = true)
        {
            ArgumentNullException.ThrowIfNull(byteArray);

            return new VarBytes(byteArray.ToArray(), isRawData);
        }

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="byteArray">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(byte[] byteArray, bool isRawData)
        {
            ArgumentNullException.ThrowIfNull(byteArray);

            return new(byteArray, isRawData);
        }

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
        public static VarBytes Of(ArraySegment<byte> byteArray, bool isRawData = true) => new(byteArray.ToArray(), isRawData);

        /// <summary>
        /// Creates a new <see cref="VarBytes"/>
        /// </summary>
        /// <param name="byteArray">the data to create the instance from</param>
        /// <param name="isRawData">indicates if the data is raw bytes, or already converted to var-bytes</param>
        /// <returns>The created instance</returns>
        public static VarBytes Of(Span<byte> byteArray, bool isRawData = true) => new(byteArray.ToArray(), isRawData);
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
                if (IsDefault)
                    throw new IndexOutOfRangeException();

                return _data.ByteAt(index * 8);
            }
        }

        public byte[] Slice(int index, int length)
        {
            if (IsDefault && index == 0 && length == 0)
                return [];

            return _data
                .ToByteArray()
                .AsSpan()
                .Slice(index, length)
                .ToArray();
        }
        #endregion

        #region DefaultProvider
        public bool IsDefault => _data.IsDefault;

        public static VarBytes Default => default;
        #endregion

        #region overrides
        public override string ToString() => $"VarBytes: {_data}";

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is VarBytes other && Equals(other);
        }

        public bool Equals(VarBytes varbytes)
        {
            return _data.Equals(varbytes._data);
        }

        public override int GetHashCode() => _data.GetHashCode();

        public static bool operator ==(VarBytes a, VarBytes b) => a.Equals(b);
        public static bool operator !=(VarBytes a, VarBytes b) => !a.Equals(b);
        #endregion

        #region API
        public bool IsEmpty => IsDefault;

        public static VarBytes Empty => default;

        /// <summary>
        /// Converts this var-bytes into a byte array, removing the overflow bits, and stitching
        /// the original data back together.
        /// </summary>
        /// <returns>the byte array</returns>
        public byte[] ToRawByteArray()
        {
            if (IsDefault)
                return [];

            return _data
                .ApplyTo(RemoveOverflow)
                .ToByteArray();
        }

        public byte[] ToByteArray()
        {
            if (IsDefault)
                return [];

            return _data.ToByteArray();
        }

        /// <summary>
        /// Empty <see cref="VarBytes"/> instances yield "0"; all others are calculated accordingly
        /// <para>
        /// Note, because of the sign, the raw bytes making up the <see cref="BigInteger"/> are interpreted differently.
        /// E.g
        /// <code>
        /// new BigInteger(new byte[]{192}, false); // this yields -64
        /// new BigInteger(new byte[]{192}, true);  // this yields 192
        /// new BigInteger(new byte[]{192, 0}, false);  // this yields 192
        /// new BigInteger(new byte[]{192, 0}, true);   // this yields 192
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="isUnsigned">indicates if the expected value is unsigned</param>
        /// <returns></returns>
        public BigInteger ToBigInteger(bool isUnsigned = false)
        {
            if (IsDefault)
                return BigInteger.Zero;

            return new BigInteger(ToRawByteArray(), isUnsigned);
        }
        #endregion

        #region IEnumerable
        public IEnumerator<byte> GetEnumerator()
        {
            return IsDefault
                ? Enumerable.Empty<byte>().GetEnumerator()
                : ((IEnumerable<byte>)_data.ToByteArray()).GetEnumerator();
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
            if (bits.IsEmpty)
                yield break;

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
            return BitSequence.Of(bits.SkipEvery(8));
        }

        /// <summary>
        /// Validate the data - make sure no byte follows a previous byte with its overflow bit unset.
        /// </summary>
        /// <param name="data">the byte array to validate</param>
        public static bool IsVarByteArray(byte[] data)
        {
            if (data is null)
                return false;

            if (data.IsEmpty())
                return true;

            if (data[^1].IsSet(7))
                return false;

            for (int cnt = data.Length - 2; cnt >= 0; cnt--)
            {
                if (!data[cnt].IsSet(7))
                    return false;
            }

            return true;
        }
        #endregion
    }
}
