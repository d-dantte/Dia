using Axis.Dia.Json.Path;
using Axis.Luna.Result;

namespace Axis.Dia.Json.Tests.Path
{
    [TestClass]
    public class PropertyTests
    {
        [TestMethod]
        public void Property_ImplicitOperator_ShouldReturnProperty()
        {
            Property property = "@testProperty";
            Assert.AreEqual("@testProperty", property.ToString());
        }

        [TestMethod]
        public void Property_Of_ShouldReturnParsedProperty()
        {
            var property = Property.Of("@name");
            Assert.AreEqual("@name", property.ToString());
        }

        [TestMethod]
        public void Property_TryParse_ShouldReturnTrueForValidNotation()
        {
            string validText = "@name";
            var success = Property.TryParse(validText, out var result);
            Assert.IsTrue(success);
            Assert.AreEqual("@name", result.Resolve().ToString());
        }

        [TestMethod]
        public void Property_TryParse_ShouldReturnFalseForInvalidPrefix()
        {
            string invalidText = "name";
            var success = Property.TryParse(invalidText, out var result);
            Assert.IsFalse(success);
            Assert.IsTrue(result.IsErrorResult(out var exception));
            Assert.IsInstanceOfType<FormatException>(exception);
        }

        [TestMethod]
        public void Property_Parse_ShouldReturnParsedProperty()
        {
            var result = Property.Parse("@test");
            Assert.AreEqual("@test", result.Resolve().ToString());
        }

        [TestMethod]
        public void Property_UnescapeString_ShouldHandleEscapedSlashes()
        {
            string input = "test\\/name";
            string unescaped = Property.UnescapeString(input);
            Assert.AreEqual("test/name", unescaped);
        }

        [TestMethod]
        public void Property_UnescapeString_ShouldReturnNullForNullInput()
        {
            string unescaped = Property.UnescapeString(null!);
            Assert.IsNull(unescaped);
        }

        [TestMethod]
        public void Property_Equals_ShouldReturnTrueForEqualProperties()
        {
            var property1 = Property.Of("@name");
            var property2 = Property.Of("@name");
            Assert.IsTrue(property1.Equals(property2));
            Assert.IsTrue(property1.Equals((object)property2));
        }

        [TestMethod]
        public void Property_Equals_ShouldReturnFalseForDifferentProperties()
        {
            var property1 = Property.Of("@name1");
            var property2 = Property.Of("@name2");
            Assert.IsFalse(property1.Equals(property2));
        }

        [TestMethod]
        public void Property_GetHashCode_ShouldReturnSameValueForEqualProperties()
        {
            var property1 = Property.Of("@name");
            var property2 = Property.Of("@name");
            Assert.AreEqual(property1.GetHashCode(), property2.GetHashCode());
        }

        [TestMethod]
        public void Property_GetHashCode_ShouldReturnDifferentValuesForDifferentProperties()
        {
            var property1 = Property.Of("@name1");
            var property2 = Property.Of("@name2");
            Assert.AreNotEqual(property1.GetHashCode(), property2.GetHashCode());
        }

        [TestMethod]
        public void Property_ToString_ShouldReturnFormattedString()
        {
            var property = Property.Of("@name");
            Assert.AreEqual("@name", property.ToString());
        }

        [TestMethod]
        public void Property_ToString_ShouldEscapeForwardSlashes()
        {
            var property = Property.Of("@name/with/slash");
            Assert.AreEqual("@name\\/with\\/slash", property.ToString());
        }

        [TestMethod]
        public void Property_ToString_ShouldReturnAsteriskForDefaultProperty()
        {
            var property = new Property();
            Assert.AreEqual("*", property.ToString());
        }

        [TestMethod]
        public void Property_IsDefault_ShouldReturnTrueForDefaultProperty()
        {
            var property = new Property();
            Assert.IsTrue(property.IsDefault);
        }

        [TestMethod]
        public void Property_IsDefault_ShouldReturnFalseForNonDefaultProperty()
        {
            var property = Property.Of("@name");
            Assert.IsFalse(property.IsDefault);
        }

        [TestMethod]
        public void Property_EqualityOperators_ShouldReturnTrueForEqualProperties()
        {
            var property1 = Property.Of("@name");
            var property2 = Property.Of("@name");
            Assert.IsTrue(property1 == property2);
        }

        [TestMethod]
        public void Property_EqualityOperators_ShouldReturnFalseForDifferentProperties()
        {
            var property1 = Property.Of("@name1");
            var property2 = Property.Of("@name2");
            Assert.IsTrue(property1 != property2);
        }
    }

}
