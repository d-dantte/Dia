using Axis.Luna.Extensions;
using Axis.Luna.Result;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Axis.Dia.Json.Tests.Path
{
    using ValueIndex = Axis.Dia.Json.Path.Index;

    [TestClass]
    public class ValueIndexTests
    {
        [TestMethod]
        public void ValueIndex_ImplicitOperator_ShouldConvertIntToValueIndex()
        {
            ValueIndex index = 15;
            Assert.AreEqual(15, index.Value);
        }

        [TestMethod]
        public void ValueIndex_ImplicitOperator_ShouldConvertStringToValueIndex()
        {
            ValueIndex index = ":25";
            Assert.AreEqual(25, index.Value);
        }

        [TestMethod]
        public void ValueIndex_Of_ShouldReturnCorrectValueIndex()
        {
            var index = ValueIndex.Of(5);
            Assert.AreEqual(5, index.Value);
        }

        [TestMethod]
        public void ValueIndex_TryParse_ShouldReturnTrueForValidValueIndexNotation()
        {
            string validText = ":12";
            var success = ValueIndex.TryParse(validText, out var result);
            Assert.IsTrue(success);
            Assert.AreEqual(12, result.Resolve());
        }

        [TestMethod]
        public void ValueIndex_TryParse_ShouldReturnFalseForInvalidPrefix()
        {
            string invalidText = "12";
            var success = ValueIndex.TryParse(invalidText, out var result);
            Assert.IsFalse(success);
            Assert.IsTrue(result.IsErrorResult(out var exception));
            Assert.IsInstanceOfType<FormatException>(exception);
        }

        [TestMethod]
        public void ValueIndex_TryParse_ShouldReturnFalseForInvalidValueIndexValue()
        {
            string invalidText = ":abc";
            var success = ValueIndex.TryParse(invalidText, out var result);
            Assert.IsFalse(success);
            Assert.IsTrue(result.IsErrorResult(out var exception));
            Assert.IsInstanceOfType<FormatException>(exception);
        }

        [TestMethod]
        public void ValueIndex_Parse_ShouldReturnParsedValueIndex()
        {
            var result = ValueIndex.Parse(":8");
            Assert.AreEqual(8, result.Resolve());
        }

        [TestMethod]
        public void ValueIndex_Equals_ShouldReturnTrueForEqualValueIndexes()
        {
            var index1 = ValueIndex.Of(20);
            var index2 = ValueIndex.Of(20);
            Assert.IsTrue(index1.Equals(index2));
            Assert.IsTrue(index1.Equals((object)index2));
        }

        [TestMethod]
        public void ValueIndex_Equals_ShouldReturnFalseForDifferentValueIndexes()
        {
            var index1 = ValueIndex.Of(20);
            var index2 = ValueIndex.Of(30);
            Assert.IsFalse(index1.Equals(index2));
        }

        [TestMethod]
        public void ValueIndex_GetHashCode_ShouldReturnSameValueForEqualValueIndexes()
        {
            var index1 = ValueIndex.Of(50);
            var index2 = ValueIndex.Of(50);
            Assert.AreEqual(index1.GetHashCode(), index2.GetHashCode());
        }

        [TestMethod]
        public void ValueIndex_GetHashCode_ShouldReturnDifferentValuesForDifferentValueIndexes()
        {
            var index1 = ValueIndex.Of(50);
            var index2 = ValueIndex.Of(100);
            Assert.AreNotEqual(index1.GetHashCode(), index2.GetHashCode());
        }

        [TestMethod]
        public void ValueIndex_CompareTo_ShouldReturnZeroForEqualValueIndexes()
        {
            var index1 = ValueIndex.Of(25);
            var index2 = ValueIndex.Of(25);
            Assert.AreEqual(0, index1.CompareTo(index2));
        }

        [TestMethod]
        public void ValueIndex_CompareTo_ShouldReturnNegativeForSmallerValueIndex()
        {
            var index1 = ValueIndex.Of(10);
            var index2 = ValueIndex.Of(20);
            Assert.IsTrue(index1.CompareTo(index2) < 0);
        }

        [TestMethod]
        public void ValueIndex_CompareTo_ShouldReturnPositiveForLargerValueIndex()
        {
            var index1 = ValueIndex.Of(30);
            var index2 = ValueIndex.Of(20);
            Assert.IsTrue(index1.CompareTo(index2) > 0);
        }

        [TestMethod]
        public void ValueIndex_ToString_ShouldReturnFormattedString()
        {
            var index = ValueIndex.Of(15);
            Assert.AreEqual(":15", index.ToString());
        }

        [TestMethod]
        public void ValueIndex_IsDefault_ShouldReturnTrueForDefaultValueIndex()
        {
            var index = new ValueIndex();
            Assert.IsTrue(index.IsDefault);
        }

        [TestMethod]
        public void ValueIndex_IsDefault_ShouldReturnFalseForNonDefaultValueIndex()
        {
            var index = ValueIndex.Of(5);
            Assert.IsFalse(index.IsDefault);
        }

        [TestMethod]
        public void ValueIndex_EqualityOperators_ShouldReturnTrueForEqualValueIndexes()
        {
            var index1 = ValueIndex.Of(15);
            var index2 = ValueIndex.Of(15);
            Assert.IsTrue(index1 == index2);
        }

        [TestMethod]
        public void ValueIndex_EqualityOperators_ShouldReturnFalseForDifferentValueIndexes()
        {
            var index1 = ValueIndex.Of(10);
            var index2 = ValueIndex.Of(20);
            Assert.IsTrue(index1 != index2);
        }

        [TestMethod]
        public void ValueIndex_ComparisonOperators_ShouldWorkCorrectly()
        {
            var index1 = ValueIndex.Of(10);
            var index2 = ValueIndex.Of(20);
            Assert.IsTrue(index1 < index2);
            Assert.IsTrue(index1 <= index2);
            Assert.IsFalse(index1 > index2);
            Assert.IsFalse(index1 >= index2);

            var index3 = ValueIndex.Of(20);
            Assert.IsTrue(index2 >= index3);
            Assert.IsTrue(index2 <= index3);
            Assert.IsFalse(index1 == index2);
        }
    }

}
