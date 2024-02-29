using Axis.Dia.Core.Convert.Axon;
using Axis.Dia.Core.Convert.Axon.Serializers;
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Core.Tests.Convert.Axon.Serializers
{
    [TestClass]
    public class BlobSerializerTest
    {
        [TestMethod]
        public void Serialize_WithDefaultConext_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => BlobSerializer.Serialize(default, default));
        }

        [TestMethod]
        public void Serialize_SingleLine()
        {
            var attributes = Types.Attribute.Of(
                ("stuff", default(string?)),
                ("other", "more-stuff"),
                ("last", default(string?)));
            var blob = Blob.Of(new byte[] { 1, 99, 122, 14 });
            var blobWithAttributes = Blob.Of(
                new byte[] { 1, 99, 122, 14 },
                attributes);
            var context = Options
                .Builder()
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithBlobCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            var text = BlobSerializer.Serialize(blob, context);
            Assert.AreEqual("<AWN6Dg==>", text);
            text = BlobSerializer.Serialize(blob, canonicalContext);
            Assert.AreEqual("'#Blob AWN6Dg=='", text);

            text = BlobSerializer.Serialize(blobWithAttributes, context);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::<AWN6Dg==>", text);
            text = BlobSerializer.Serialize(blobWithAttributes, canonicalContext);
            Assert.AreEqual("'@stuff;@other:more-stuff;@last;'::'#Blob AWN6Dg=='", text);

            text = BlobSerializer.Serialize(default, context);
            Assert.AreEqual("*.blob", text);
            text = BlobSerializer.Serialize(default, canonicalContext);
            Assert.AreEqual($"*.blob", text);
        }

        [TestMethod]
        public void Serialize_Multiline()
        {
            var blob = Enumerable
                .Range(0, 30)
                .Select(@int => (byte)@int)
                .ApplyTo(bytes => Blob.Of(bytes));
            var context = Options
                .Builder()
                .WithIndentationStyle(Options.IndentationStyle.Tabs)
                .WithBlobSinglelineCharacterCount(20)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));
            var canonicalContext = Options
                .Builder()
                .WithIndentationStyle(Options.IndentationStyle.Tabs)
                .WithBlobSinglelineCharacterCount(20)
                .WithBlobCanonicalForm(true)
                .Build()
                .ApplyTo(opts => SerializerContext.Of(opts));

            var text = BlobSerializer.Serialize(blob, context);
            Assert.AreEqual("<AAECAwQFBgcICQoLDA0O>\r\n+ <DxAREhMUFRYXGBkaGxwd>", text);
            text = BlobSerializer.Serialize(blob, canonicalContext);
            Assert.AreEqual("'#Blob AAECAwQFBgcICQoLDA0O'\r\n+ 'DxAREhMUFRYXGBkaGxwd'", text);
        }
    }
}
