using Axis.Dia.IO.Binary;
using Axis.Luna.Common.Utils;
using System.Numerics;

namespace Axis.Dia.Tests.IO.Binary
{
    [TestClass]
    public class VarBytesTests
    {
        /// <summary>
        /// before overflow
        /// 0000 0111 -> 7
        /// 0110 1001 -> 105
        /// </summary>
        private static short RawValue = 26887;

        ///<summary>
        /// after overflow
        /// 1000 0111 -> 135
        /// 1101 0010 -> 210
        /// 0000 0001 -> 1
        /// </summary>
        private static BigInteger OverflowValue = 119431;

        [TestMethod]
        public void Of_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => VarBytes.Of(null));


            var varbytes = VarBytes.Of(
                (IEnumerable<byte>)BitConverter.GetBytes(RawValue));
            var result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            varbytes = VarBytes.Of(
                (IEnumerable<byte>)OverflowValue.ToByteArray(),
                false);
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            Assert.ThrowsException<ArgumentException>(
                () => VarBytes.Of((IEnumerable<byte>)BitConverter.GetBytes(RawValue), false));


            varbytes = VarBytes.Of(
                BitConverter.GetBytes(RawValue));
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            varbytes = VarBytes.Of(
                OverflowValue.ToByteArray(),
                false);
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            Assert.ThrowsException<ArgumentException>(
                () => VarBytes.Of(BitConverter.GetBytes(RawValue), false));


            varbytes = VarBytes.Of(
                new ArraySegment<byte>(BitConverter.GetBytes(RawValue)));
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            varbytes = VarBytes.Of(
                new ArraySegment<byte>(OverflowValue.ToByteArray()),
                false);
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            Assert.ThrowsException<ArgumentException>(
                () => VarBytes.Of(new ArraySegment<byte>(BitConverter.GetBytes(RawValue)), false));


            varbytes = VarBytes.Of(
                new Span<byte>(BitConverter.GetBytes(RawValue)));
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            varbytes = VarBytes.Of(
                new Span<byte>(OverflowValue.ToByteArray()),
                false);
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            Assert.ThrowsException<ArgumentException>(
                () => VarBytes.Of(new Span<byte>(BitConverter.GetBytes(RawValue)), false));
        }

        [TestMethod]
        public void Implicits_Tests()
        {
            Assert.ThrowsException<ArgumentNullException>(() => VarBytes.Of(null));

            VarBytes varbytes = BitConverter.GetBytes(RawValue);
            var result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            varbytes = new ArraySegment<byte>(BitConverter.GetBytes(RawValue));
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);

            varbytes = new Span<byte>(BitConverter.GetBytes(RawValue));
            result = new BigInteger(varbytes.ToArray());
            Assert.AreEqual(OverflowValue, result);
        }

        [TestMethod]
        public void Length_Tests()
        {
            VarBytes varbyte = BitConverter.GetBytes(RawValue);
            Assert.AreEqual(3, varbyte.Length);
        }

        [TestMethod]
        public void Default_Tests()
        {
            VarBytes varbytes = default;
            Assert.IsTrue(varbytes.IsDefault);

            varbytes = ArrayUtil.Of<byte>(0, 1, 2, 3);
            Assert.IsFalse(varbytes.IsDefault);
        }

        [TestMethod]
        public void Indexer_Tests()
        {
            VarBytes varbytes = BitConverter.GetBytes(RawValue);
            Assert.AreEqual((byte)135, varbytes[0]);
            Assert.AreEqual((byte)210, varbytes[1]);
            Assert.AreEqual((byte)1, varbytes[2]);
        }

        [TestMethod]
        public void Slice_Tests()
        {
            VarBytes varbytes = BitConverter.GetBytes(RawValue);
            byte[] sliced = varbytes[0..2];
            Assert.AreEqual(2, sliced.Length);
            Assert.AreEqual((byte)135, sliced[0]);
            Assert.AreEqual((byte)210, sliced[1]);
        }

        [TestMethod]
        public void ToByteArray_Tests()
        {
            VarBytes varbytes = BitConverter.GetBytes(RawValue);
            var rawBytes = varbytes.ToByteArray();
            var result = BitConverter.ToInt16(rawBytes);
            Assert.AreEqual(RawValue, result);
        }

        [TestMethod]
        public void Equals_Tests()
        {
            VarBytes varbytes = BitConverter.GetBytes(RawValue);
            VarBytes varbytes2 = BitConverter.GetBytes(RawValue);
            VarBytes varbytes3 = OverflowValue.ToByteArray();
            Assert.AreEqual(varbytes, varbytes);
            Assert.IsTrue(varbytes.Equals(varbytes));
            Assert.AreEqual(varbytes, varbytes2);

            Assert.IsTrue(varbytes.Equals(varbytes2));
            Assert.IsTrue(varbytes == varbytes2);
            Assert.AreEqual(varbytes, varbytes2);

            Assert.IsFalse(varbytes.Equals(varbytes3));
            Assert.IsFalse(varbytes == varbytes3);
            Assert.AreNotEqual(varbytes, varbytes3);
        }
    }
}
