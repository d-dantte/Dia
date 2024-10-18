using Axis.Dia.Json.Path;
using Axis.Luna.Result;

namespace Axis.Dia.Json.Tests.Path
{
    [TestClass]
    public class ValuePathTests
    {
        [TestMethod]
        public void ValuePath_Constructor_ShouldThrowIfNullSegments()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ValuePath((ISegment[])null!));

            Assert.ThrowsException<ArgumentNullException>(
                () => new ValuePath((string[])null!));
        }

        [TestMethod]
        public void ValuePath_Constructor_ShouldThrowIfInvalidSegment()
        {
            Assert.ThrowsException<FormatException>(
                () => new ValuePath(":abc"));
            Assert.ThrowsException<ArgumentException>(
                () => new ValuePath(""));
            Assert.ThrowsException<FormatException>(
                () => new ValuePath("@"));
            Assert.ThrowsException<FormatException>(
                () => new ValuePath("%abcd1234"));
        }

        [TestMethod]
        public void ValuePath_Constructor_ShouldAcceptValidSegments()
        {
            var path = new ValuePath(":3", "@abc");
            Assert.AreEqual("/:3/@abc", path.ToString());
        }

        [TestMethod]
        public void ValuePath_ImplicitOperator_ShouldParseNotation()
        {
            ValuePath path = "/@abc/@def/@h i \\/ h";
            Assert.AreEqual("/@abc/@def/@h i \\/ h", path.ToString());
        }

        [TestMethod]
        public void ValuePath_Of_ShouldReturnParsedValuePath()
        {
            var path = ValuePath.Of("/@segment1/@segment2");
            Assert.AreEqual("/@segment1/@segment2", path.ToString());
        }

        [TestMethod]
        public void ValuePath_Equals_ShouldReturnTrueForEqualPaths()
        {
            var path1 = new ValuePath("@segment1", "@segment2");
            var path2 = new ValuePath("@segment1", "@segment2");
            Assert.IsTrue(path1.Equals(path2));
            Assert.IsTrue(path1.Equals((object)path2));
        }

        [TestMethod]
        public void ValuePath_Equals_ShouldReturnFalseForDifferentPaths()
        {
            var path1 = new ValuePath("@segment1", "@segment2");
            var path2 = new ValuePath("@segment1", "@segment3");
            Assert.IsFalse(path1.Equals(path2));
        }

        [TestMethod]
        public void ValuePath_TryParse_ShouldReturnTrueAndSetResultForValidPath()
        {
            string text = "/@segment1/@segment2";
            var success = ValuePath.TryParse(text, out var result);

            Assert.IsTrue(success);
            Assert.AreEqual("/@segment1/@segment2", result.Resolve());
        }

        [TestMethod]
        public void ValuePath_TryParse_ShouldThrowArgumentException()
        {
            string text = "";
            Assert.ThrowsException<ArgumentException>(
                () => ValuePath.TryParse(text, out var result));
        }

        [TestMethod]
        public void ValuePath_Parse_ShouldReturnParsedResult()
        {
            var result = ValuePath.Parse("/@segment1/:67");
            Assert.AreEqual("/@segment1/:67", result.Resolve());
        }

        [TestMethod]
        public void ValuePath_ToString_ShouldReturnFormattedString()
        {
            var path = new ValuePath("@segment1", ":12");
            Assert.AreEqual("/@segment1/:12", path.ToString());
        }

        [TestMethod]
        public void ValuePath_ToString_ShouldReturnAsteriskForDefaultPath()
        {
            Assert.AreEqual("*", new ValuePath().ToString());
            Assert.AreEqual("*", default(ValuePath).ToString());
        }

        [TestMethod]
        public void ValuePath_GetHashCode_ShouldReturnZeroForDefaultPath()
        {
            var path = new ValuePath();
            Assert.AreEqual(0, path.GetHashCode());
        }

        [TestMethod]
        public void ValuePath_GetHashCode_ShouldReturnNonZeroForValidPath()
        {
            var path = new ValuePath("@segment1", "@segment2");
            Assert.AreNotEqual(0, path.GetHashCode());
        }

        [TestMethod]
        public void ValuePath_Split_ShouldCorrectlySplitStringIntoSegments()
        {
            var segments = ValuePath.Split("/@segment1/:67").ToArray();
            Assert.AreEqual(2, segments.Length);
            Assert.AreEqual("@segment1", segments[0]);
            Assert.AreEqual(":67", segments[1]);
        }

        [TestMethod]
        public void ValuePath_Split_ShouldHandleEscapedForwardSlashes()
        {
            var segments = ValuePath.Split("/@segment1\\/segment2/@segment3").ToArray();
            Assert.AreEqual(2, segments.Length);
            Assert.AreEqual("@segment1\\/segment2", segments[0]);
            Assert.AreEqual("@segment3", segments[1]);
        }
    }

}
