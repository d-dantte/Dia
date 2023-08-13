using Axis.Dia.Utils.EscapeSequences;

namespace Axis.Dia.Tests.Utils.EscapeSequences
{
    [TestClass]
    public class ClobEscapeSequenceGroupTests
    {
        #region Escape
        [TestMethod]
        public void Escape_WithNoEscapeSequence_ReturnsInputString()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the lazy dog";
            var result = sesg.Escape(input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Escape_WithNonEscapedChars_ShouldIgnoreTheChars()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "\nThe quick brown \tfox jumps over \rthe \"lazy \'dog";
            var result = sesg.Escape(input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Escape_WithMultipleEscapeChars_ShouldEscapeTheQuotes()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The multiple things\a \b \f \v >> to escape";
            var result = sesg.Escape(input);

            Assert.AreEqual("The multiple things\\a \\b \\f \\v \\>> to escape", result);
        }
        #endregion

        #region Unescape
        [TestMethod]
        public void Unescape_WithNoEScapeSequence_ShouldReturnInputString()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Unescape_WithMultipleEscapeChars_ShouldUnecapeTheQuotes()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The multiple things\\a \\b \\f \\v \\>> to escape";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The multiple things\a \b \f \v >> to escape", result);
        }

        [TestMethod]
        public void UnescapeGreedyLines_ShouldSwallowTheWhitespaces()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = @"The quick brown fox jumps \
                          over the lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over the lazy dog", result);
        }

        [TestMethod]
        public void Escape_WithNonPrintableAsciiChar_ShouldLeaveIntact()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the ¾ lazy dog";
            var result = sesg.Escape(input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Unescape_WithNonPrintableHex2Escape_ShouldUnecape()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the \\xbe lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over the ¾ lazy dog", result);
        }

        [TestMethod]
        public void Escape_WithNonNonAsciiChar_ShouldEscapeWithHex4()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the ጚ lazy dog";
            var result = sesg.Escape(input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Unescape_WithNonHex4Escape_ShouldUnecape()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the \\u131a lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over the ጚ lazy dog", result);
        }

        [TestMethod]
        public void Unescape_WithMultipleEscapes_ShouldUnescape()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = "The quick brown fox jumps over\\n\\xbeated the \\u131aaverage lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over\n¾ated the ጚaverage lazy dog", result);
        }



        [TestMethod]
        public void Unescape_WithMultiline_andNoAlignment_ShouldUnescape()
        {
            var sesg = new ClobEscapeSequenceGroup();
            var input = @"The quick brown fox jumps over \

                          stuffs and the \u131aaverage lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over stuffs and the ጚaverage lazy dog", result);
        }
        #endregion
    }
}
