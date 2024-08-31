using Axis.Dia.Utils.EscapeSequences;

namespace Axis.Dia.Tests.Utils.EscapeSequences
{
    [TestClass]
    public class MultilineStringEscapeSequenceGroupTests
    {
        #region Escape
        [TestMethod]
        public void Escape_WithNoEscapeSequence_ReturnsInputString()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the lazy dog";
            var result = sesg.Escape(input);

            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void Escape_WithDoubleQuote_ShouldEscapeTheQuotes()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the lazy dog";
            var result = sesg.Escape($"\"{input}\"");

            Assert.AreEqual($@"\""{input}\""", result);
        }

        [TestMethod]
        public void Escape_WithMultipleEscapeChars_ShouldEscapeTheQuotes()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The \a\bquick \f\tbrown fox jumps \a\b\f\t\v\n\rover the \\\"lazy dog";
            var result = sesg.Escape(input);

            Assert.AreEqual("The \\a\\bquick \\f\\tbrown fox jumps \\a\\b\\f\\t\\v\\n\\rover the \\\\\\\"lazy dog", result);
        }
        #endregion


        #region Unescape
        [TestMethod]
        public void Unescape_WithDoubleQuote_ShouldUnecapeTheQuotes()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox \\\"jumps\\\" over the lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox \"jumps\" over the lazy dog", result);
        }

        [TestMethod]
        public void Unescape_WithMultipleEscapeChars_ShouldUnecapeTheQuotes()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The \\a\\bquick \\f\\tbrown fox jumps \\a\\b\\f\\t\\v\\n\\rover the \\\\\\\"lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The \a\bquick \f\tbrown fox jumps \a\b\f\t\v\n\rover the \\\"lazy dog", result);
        }

        [TestMethod]
        public void UnescapeGreedyLines_ShouldSwallowTheWhitespaces()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = @"The quick brown fox jumps \
                          over the lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over the lazy dog", result);
        }

        [TestMethod]
        public void Escape_WithNonPrintableAsciiChar_ShouldEscapeWithHex2()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the ¾ lazy dog";
            var result = sesg.Escape(input);

            Assert.AreEqual("The quick brown fox jumps over the \\xbe lazy dog", result);
        }

        [TestMethod]
        public void Unescape_WithNonPrintableHex2Escape_ShouldUnecape()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the \\xbe lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over the ¾ lazy dog", result);
        }

        [TestMethod]
        public void Escape_WithNonNonAsciiChar_ShouldEscapeWithHex4()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the ጚ lazy dog";
            var result = sesg.Escape(input);

            Assert.AreEqual("The quick brown fox jumps over the \\u131a lazy dog", result);
        }

        [TestMethod]
        public void Unescape_WithNonHex4Escape_ShouldUnecape()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox jumps over the \\u131a lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over the ጚ lazy dog", result);
        }

        [TestMethod]
        public void Unescape_WithMultipleEscapes_ShouldUnescape()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = "The quick brown fox jumps over\\n\\xbeated the \\u131aaverage lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over\n¾ated the ጚaverage lazy dog", result);
        }



        [TestMethod]
        public void Unescape_WithMultiline_andNoAlignment_ShouldUnescape()
        {
            var sesg = new MultilineStringEscapeSequenceGroup();
            var input = @"The quick brown fox jumps over \

                          stuffs and the \u131aaverage lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over stuffs and the ጚaverage lazy dog", result);
        }


        [TestMethod]
        public void Unescape_WithMultiline_andAlignment_ShouldUnescape()
        {
            var sesg = new MultilineStringEscapeSequenceGroup(15);
            var input = @"\
               The quick brown fox jumps over \

                          stuffs and the \u131aaverage lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over            stuffs and the ጚaverage lazy dog", result);
        }


        [TestMethod]
        public void Unescape_WithMultiline_andAlignment_andReplacements_ShouldUnescape()
        {
            var sesg = new MultilineStringEscapeSequenceGroup(15);
            var input = @"\
               The quick brown fox jumps over \^

                          stuffs and the \u131aaverage lazy dog";
            var result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown fox jumps over \r\n           stuffs and the ጚaverage lazy dog", result);


            input = @"\
               The quick brown 'fox' jumps over \^nrl

                          stuffs and the \u131aaverage lazy dog";
            result = sesg.Unescape(input);

            Assert.AreEqual("The quick brown 'fox' jumps over \n\r\r\n           stuffs and the ጚaverage lazy dog", result);
        }
        #endregion
    }
}
