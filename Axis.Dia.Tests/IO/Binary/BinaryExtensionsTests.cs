using Axis.Dia.IO.Binary;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Luna.Common;
using Axis.Luna.Common.Utils;
using System.Numerics;

namespace Axis.Dia.Tests.IO.Binary
{
    [TestClass]
    public class BinaryExtensionsTests
    {
        [TestMethod]
        public void AppendBits_Tests()
        {
            BitSequence seq = ArrayUtil.Of(true, true, false, false);
            var appended = seq.AppendBits(true, false, true);
            Assert.AreEqual(7, appended.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(
                ArrayUtil.Of(true, false, true),
                appended[^3..]));
        }

        [TestMethod]
        public void AppendBits_FromBytes_Tests()
        {
            BitSequence seq = ArrayUtil.Of(true, true, false, false);
            var appended = seq.AppendBits(4);
            Assert.AreEqual(12, appended.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(
                ArrayUtil.Of(false, false, true, false),
                appended[4..8]));
        }

        [TestMethod]
        public void ToVarBytes_FromCMeta_Tests()
        {
            var tmeta = TypeMetadata.Of(
                Contracts.DiaType.Int,
                TypeMetadata.MetadataFlags.None,
                CustomMetadata.Of(208),
                CustomMetadata.Of(10));

            var varbytes = tmeta.CustomMetadata.ToVarBytes();
            Assert.AreEqual(2, varbytes.Length);
            Assert.AreEqual(208, varbytes[0]);
            Assert.AreEqual(10, varbytes[1]);
        }

        [TestMethod]
        public void ToBigInteger_Tests()
        {
            var tmeta = TypeMetadata.Of(
                Contracts.DiaType.Int,
                TypeMetadata.MetadataFlags.None,
                CustomMetadata.Of(208),
                CustomMetadata.Of(10));

            var @int = tmeta.CustomMetadata.ToBigInteger();
            Assert.AreEqual(1360, @int);
        }

        [TestMethod]
        public void ToCustomMetadata_Tests()
        {
            BigInteger @int = 344123;
            var cmeta = @int.ToCustomMetadata();
            var result = cmeta.ToBigInteger();
            Assert.AreEqual(@int, result);
        }

        [TestMethod]
        public void BigInt_ToVarByte_Tests()
        {
            var i = new BigInteger(192);
            var arr = i.ToByteArray();
            var varr = i.ToVarBytes(false);
            var r = varr.ToBigInteger();
            Assert.AreEqual(i, r);
        }

        [TestMethod]
        public void GetSignificantBitLength_Tests()
        {
            for (int cnt = 0; cnt < 500; cnt++)
            {
                var bint = new BigInteger(-cnt);
                Console.WriteLine($"Value: {cnt}, Regular bit length: {bint.GetBitLength()}, Sig bit length: {bint.GetSignificantBitLength(out _)}");
            }
        }

        [TestMethod]
        public void ToBigInteger_WithRange_Tests()
        {
            var cmetaArray = new byte[] { 195, 211, 188, 29}
                .Select(CustomMetadata.Of)
                .ToArray();

            var result = cmetaArray.ToBigInteger(3..5);
            Assert.AreEqual(0, result);

            result = cmetaArray.ToBigInteger(3..6);
            Assert.AreEqual(0, result);

            result = cmetaArray.ToBigInteger(3..7);
            Assert.AreEqual(8, result);

            result = cmetaArray.ToBigInteger(6..9);
            Assert.AreEqual(7, result);

            result = cmetaArray.ToBigInteger(..22);
            Assert.AreEqual(3090883, result);
        }

        [TestMethod]
        public void ToVarBytes_FromBigInteger_Tests()
        {
            var i = new BigInteger(-3);
            var vb = i.ToVarBytes();
            var barr = vb.ToByteArray();
            var i2 = new BigInteger(barr, false);
        }
    }
}
