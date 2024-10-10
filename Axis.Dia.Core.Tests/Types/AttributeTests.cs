namespace Axis.Dia.Core.Tests.Types;

[TestClass]
public class AttributeTests
{
    [TestMethod]
    public void Constructor_WithKeyAndValue_ShouldSetProperties()
    {
        // Arrange
        string key = "validKey";
        string value = "someValue";

        // Act
        var attribute = new Core.Types.Attribute(key, value);

        // Assert
        Assert.AreEqual(key, attribute.Key);
        Assert.AreEqual(value, attribute.Value);
        Assert.IsTrue(attribute.HasValue);
        Assert.IsFalse(attribute.IsScalar);
        Assert.IsFalse(attribute.IsDefault);
    }

    [TestMethod]
    public void Constructor_WithOnlyKey_ShouldSetKeyAndNullValue()
    {
        // Arrange
        string key = "validKey";

        // Act
        var attribute = new Core.Types.Attribute(key);

        // Assert
        Assert.AreEqual(key, attribute.Key);
        Assert.IsNull(attribute.Value);
        Assert.IsFalse(attribute.HasValue);
        Assert.IsTrue(attribute.IsScalar);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_ShouldThrowException_WhenKeyIsNullOrEmpty()
    {
        // Act
        new Core.Types.Attribute(null!); // null case
        new Core.Types.Attribute(""); // empty string case
    }

    [TestMethod]
    [ExpectedException(typeof(FormatException))]
    public void Constructor_ShouldThrowException_WhenKeyHasInvalidFormat()
    {
        // Arrange
        string invalidKey = "invalid key!";

        // Act
        new Core.Types.Attribute(invalidKey, "value");
    }

    [TestMethod]
    public void Default_ShouldReturnDefaultAttribute()
    {
        // Act
        var defaultAttribute = Core.Types.Attribute.Default;

        // Assert
        Assert.IsTrue(defaultAttribute.IsDefault);
        Assert.IsNull(defaultAttribute.Key);
        Assert.IsNull(defaultAttribute.Value);
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_WithKeyAndValue()
    {
        // Arrange
        var attribute = new Core.Types.Attribute("key", "value");

        // Act
        string result = attribute.ToString();

        // Assert
        Assert.AreEqual("[@Attribute key:value]", result);
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_WithOnlyKey()
    {
        // Arrange
        var attribute = new Core.Types.Attribute("key");

        // Act
        string result = attribute.ToString();

        // Assert
        Assert.AreEqual("[@Attribute key]", result);
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_ForEqualAttributes()
    {
        // Arrange
        var attribute1 = new Core.Types.Attribute("key", "value");
        var attribute2 = new Core.Types.Attribute("key", "value");

        // Act
        bool areEqual = attribute1.Equals(attribute2);

        // Assert
        Assert.IsTrue(areEqual);
        Assert.IsTrue(attribute1 == attribute2);
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentAttributes()
    {
        // Arrange
        var attribute1 = new Core.Types.Attribute("key1", "value1");
        var attribute2 = new Core.Types.Attribute("key2", "value2");

        // Act
        bool areEqual = attribute1.Equals(attribute2);

        // Assert
        Assert.IsFalse(areEqual);
        Assert.IsTrue(attribute1 != attribute2);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualValues()
    {
        // Arrange
        var attribute1 = new Core.Types.Attribute("key", "value");
        var attribute2 = new Core.Types.Attribute("key", "value");

        // Act
        bool areEqual = attribute1.ValueEquals(attribute2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForDifferentValues()
    {
        // Arrange
        var attribute1 = new Core.Types.Attribute("key1", "value1");
        var attribute2 = new Core.Types.Attribute("key2", "value2");

        // Act
        bool areEqual = attribute1.ValueEquals(attribute2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnSameValue_ForEqualAttributes()
    {
        // Arrange
        var attribute1 = new Core.Types.Attribute("key", "value");
        var attribute2 = new Core.Types.Attribute("key", "value");

        // Act
        int hash1 = attribute1.GetHashCode();
        int hash2 = attribute2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnDifferentValues_ForDifferentAttributes()
    {
        // Arrange
        var attribute1 = new Core.Types.Attribute("key1", "value1");
        var attribute2 = new Core.Types.Attribute("key2", "value2");

        // Act
        int hash1 = attribute1.GetHashCode();
        int hash2 = attribute2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertStringToAttribute()
    {
        // Arrange
        string key = "key";

        // Act
        Core.Types.Attribute attribute = key;

        // Assert
        Assert.AreEqual(key, attribute.Key);
        Assert.IsNull(attribute.Value);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertTupleToAttribute()
    {
        // Arrange
        (string key, string? value) tuple = ("key", "value");

        // Act
        Core.Types.Attribute attribute = tuple;

        // Assert
        Assert.AreEqual("key", attribute.Key);
        Assert.AreEqual("value", attribute.Value);
    }

    [TestMethod]
    public void OfMethod_WithKeyAndValue_ShouldReturnCorrectAttribute()
    {
        // Act
        var attribute = Core.Types.Attribute.Of("key", "value");

        // Assert
        Assert.AreEqual("key", attribute.Key);
        Assert.AreEqual("value", attribute.Value);
    }

    [TestMethod]
    public void OfMethod_WithOnlyKey_ShouldReturnCorrectAttribute()
    {
        // Act
        var attribute = Core.Types.Attribute.Of("key");

        // Assert
        Assert.AreEqual("key", attribute.Key);
        Assert.IsNull(attribute.Value);
    }

    [TestMethod]
    public void OfMethod_WithMultipleKeys_ShouldReturnArrayOfAttributes()
    {
        // Act
        var attributes = Core.Types.Attribute.Of("key1", "key2", "key3");

        // Assert
        Assert.AreEqual(3, attributes.Length);
        Assert.AreEqual("key1", attributes[0].Key);
        Assert.AreEqual("key2", attributes[1].Key);
        Assert.AreEqual("key3", attributes[2].Key);
    }

    [TestMethod]
    public void OfMethod_WithMultipleTuples_ShouldReturnArrayOfAttributes()
    {
        // Act
        var attributes = Core.Types.Attribute.Of(("key1", "value1"), ("key2", "value2"));

        // Assert
        Assert.AreEqual(2, attributes.Length);
        Assert.AreEqual("key1", attributes[0].Key);
        Assert.AreEqual("value1", attributes[0].Value);
        Assert.AreEqual("key2", attributes[1].Key);
        Assert.AreEqual("value2", attributes[1].Value);
    }
}
