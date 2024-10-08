using Axis.Dia.BionSerializer.Utils;

namespace Axis.Dia.BionSerializer.Tests.Utils
{
    [TestClass]
    public class StreamExtensionTests
    {
        [TestMethod]
        public void ReadChunks_Tests()
        {
            var bytes = new byte[ByteChunks.MaxSectionDataCount * 3];
            Random.Shared.NextBytes(bytes);
            var chunks = ByteChunks.Of(bytes);

            var memory = new MemoryStream();
            memory.Write(chunks.ToByteArray());
            memory.Seek(0, SeekOrigin.Begin);
            var result = memory.ReadChunks();

            Assert.IsTrue(chunks.ValueEquals(result));
            CollectionAssert.AreEqual(bytes, result.ToRawBytes());


            bytes = new byte[bytes.Length + 5];
            Random.Shared.NextBytes(bytes);
            chunks = ByteChunks.Of(bytes);

            memory = new MemoryStream();
            memory.Write(chunks.ToByteArray());
            memory.Seek(0, SeekOrigin.Begin);
            result = memory.ReadChunks();

            Assert.IsTrue(chunks.ValueEquals(result));
            CollectionAssert.AreEqual(bytes, result.ToRawBytes());
        }
    }
}
