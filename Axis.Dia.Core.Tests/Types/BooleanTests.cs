using Axis.Dia.Core.Types;
using System.Collections.Immutable;

namespace Axis.Dia.Core.Tests.Types;
[TestClass]
public class BooleanTests
{
    [TestMethod]
    public void Constructor_WithValidBoolAndAttributes_ShouldInitializeBoolean()
    {
        // Arrange
        bool? value = true;
        var attributes = new  Core.Types.Attribute[] { new  Core.Types.Attribute("key", "value") };

        // Act
        var boolean = new Core.Types.Boolean(value, attributes);

        // Assert
        Assert.IsFalse(boolean.IsNull);
        Assert.IsFalse(boolean.IsDefault);
        Assert.AreEqual(true, boolean.Value);
        Assert.AreEqual(1, boolean.Attributes.Count);
        Assert.IsTrue(boolean.Attributes.Contains(new  Core.Types.Attribute("key", "value")));
    }

    [TestMethod]
    public void Constructor_WithNullBool_ShouldSetValueToNull()
    {
        // Arrange
        var attributes = new  Core.Types.Attribute[] { new  Core.Types.Attribute("key", "value") };

        // Act
        var boolean = new Core.Types.Boolean(null, attributes);

        // Assert
        Assert.IsTrue(boolean.IsNull);
        Assert.AreEqual(1, boolean.Attributes.Count);
        Assert.AreEqual("value", boolean.Attributes.First().Value);
    }

    [TestMethod]
    public void Constructor_WithNoAttributes_ShouldInitializeEmptyAttributes()
    {
        // Act
        var boolean = new Core.Types.Boolean(true);

        // Assert
        Assert.AreEqual(0, boolean.Attributes.Count);
    }

    [TestMethod]
    public void Default_ShouldReturnBooleanWithDefaultState()
    {
        // Act
        var defaultBoolean = Core.Types.Boolean.Default;

        // Assert
        Assert.IsTrue(defaultBoolean.IsDefault);
        Assert.IsTrue(defaultBoolean.IsNull);
        Assert.IsNull(defaultBoolean.Value);
        Assert.AreEqual(AttributeSet.Default, defaultBoolean.Attributes);
    }

    [TestMethod]
    public void Null_ShouldReturnBooleanWithNullValue()
    {
        // Arrange
        var attributes = new  Core.Types.Attribute[] { new  Core.Types.Attribute("key", "value") };

        // Act
        var nullBoolean = Core.Types.Boolean.Null(attributes);

        // Assert
        Assert.IsTrue(nullBoolean.IsNull);
        Assert.AreEqual(1, nullBoolean.Attributes.Count);
        Assert.AreEqual("value", nullBoolean.Attributes.First().Value);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualBooleans()
    {
        // Arrange
        var boolean1 = new Core.Types.Boolean(true);
        var boolean2 = new Core.Types.Boolean(true);

        // Act
        var areEqual = boolean1.ValueEquals(boolean2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForDifferentBooleans()
    {
        // Arrange
        var boolean1 = new Core.Types.Boolean(true);
        var boolean2 = new Core.Types.Boolean(false);

        // Act
        var areEqual = boolean1.ValueEquals(boolean2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_WhenBooleanIsNullAndOtherIsNot()
    {
        // Arrange
        var nullBoolean = Core.Types.Boolean.Null();
        var boolean = new Core.Types.Boolean(true);

        // Act
        var areEqual = nullBoolean.ValueEquals(boolean);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForBothNullBooleans()
    {
        // Arrange
        var nullBoolean1 = Core.Types.Boolean.Null();
        var nullBoolean2 = Core.Types.Boolean.Null();

        // Act
        var areEqual = nullBoolean1.ValueEquals(nullBoolean2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_ForEqualBooleans()
    {
        // Arrange
        var boolean1 = new Core.Types.Boolean(true);
        var boolean2 = new Core.Types.Boolean(true);

        // Act
        var areEqual = boolean1.Equals(boolean2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentBooleans()
    {
        // Arrange
        var boolean1 = new Core.Types.Boolean(true);
        var boolean2 = new Core.Types.Boolean(false);

        // Act
        var areEqual = boolean1.Equals(boolean2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnSameValue_ForEqualBooleans()
    {
        // Arrange
        var boolean1 = new Core.Types.Boolean(true);
        var boolean2 = new Core.Types.Boolean(true);

        // Act
        var hash1 = boolean1.GetHashCode();
        var hash2 = boolean2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnDifferentValues_ForDifferentBooleans()
    {
        // Arrange
        var boolean1 = new Core.Types.Boolean(true);
        var boolean2 = new Core.Types.Boolean(false);

        // Act
        var hash1 = boolean1.GetHashCode();
        var hash2 = boolean2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertBoolToBoolean()
    {
        // Arrange
        bool? value = true;

        // Act
        Core.Types.Boolean boolean = value;

        // Assert
        Assert.AreEqual(value, boolean.Value);
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_ForNonNullBoolean()
    {
        // Arrange
        var boolean = new Core.Types.Boolean(true);

        // Act
        var result = boolean.ToString();

        // Assert
        Assert.AreEqual("[@Bool True]", result);
    }

    [TestMethod]
    public void ToString_ShouldReturnWildcardForNullBoolean()
    {
        // Arrange
        var nullBoolean = Core.Types.Boolean.Null();

        // Act
        var result = nullBoolean.ToString();

        // Assert
        Assert.AreEqual("[@Bool *]", result);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualBooleansWithAttributes()
    {
        // Arrange
        var attributes = new  Core.Types.Attribute[] { new  Core.Types.Attribute("key", "value") };
        var boolean1 = new Core.Types.Boolean(true, attributes);
        var boolean2 = new Core.Types.Boolean(true, attributes);

        // Act
        var areEqual = boolean1.ValueEquals(boolean2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForBooleansWithDifferentAttributes()
    {
        // Arrange
        var boolean1 = new Core.Types.Boolean(true, new  Core.Types.Attribute("key", "value1"));
        var boolean2 = new Core.Types.Boolean(true, new  Core.Types.Attribute("key", "value2"));

        // Act
        var areEqual = boolean1.ValueEquals(boolean2);

        // Assert
        Assert.IsFalse(areEqual);
    }
}

