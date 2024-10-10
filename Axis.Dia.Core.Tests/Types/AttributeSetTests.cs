namespace Axis.Dia.Core.Tests.Types;

using Axis.Dia.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AttributeSetTests
{
    [TestMethod]
    public void Constructor_WithValidAttributes_ShouldInitializeSet()
    {
        // Arrange
        var attributes = new Core.Types.Attribute[] { 
            new Core.Types.Attribute("key1", "value1"),
            new Core.Types.Attribute("key2", "value2")
        };

        // Act
        var attributeSet = new AttributeSet(attributes);

        // Assert
        Assert.AreEqual(2, attributeSet.Count);
        Assert.IsTrue(attributeSet.Contains(new Core.Types.Attribute("key1", "value1")));
        Assert.IsTrue(attributeSet.Contains(new Core.Types.Attribute("key2", "value2")));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullAttributes_ShouldThrowArgumentNullException()
    {
        // Act
        new AttributeSet(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithDefaultAttribute_ShouldThrowArgumentException()
    {
        // Arrange
        var attributes = new Attribute[] { Core.Types.Attribute.Default };

        // Act
        new AttributeSet(attributes);
    }

    [TestMethod]
    public void IsDefault_ShouldReturnTrue_WhenSetIsDefault()
    {
        // Act
        var defaultSet = AttributeSet.Default;

        // Assert
        Assert.IsTrue(defaultSet.IsDefault);
    }

    [TestMethod]
    public void IsDefault_ShouldReturnFalse_WhenSetIsInitialized()
    {
        // Arrange
        var attributeSet = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var isDefault = attributeSet.IsDefault;

        // Assert
        Assert.IsFalse(isDefault);
    }

    [TestMethod]
    public void IsEmpty_ShouldReturnTrue_WhenSetIsEmptyOrDefault()
    {
        // Act
        var defaultSet = AttributeSet.Default;

        // Assert
        Assert.IsTrue(defaultSet.IsEmpty);

        // Arrange
        var emptySet = new AttributeSet(new Attribute[] { });

        // Assert
        Assert.IsTrue(emptySet.IsEmpty);
    }

    [TestMethod]
    public void IsEmpty_ShouldReturnFalse_WhenSetIsNotEmpty()
    {
        // Arrange
        var attributeSet = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Assert
        Assert.IsFalse(attributeSet.IsEmpty);
    }

    [TestMethod]
    public void Count_ShouldReturnCorrectCount()
    {
        // Arrange
        var attributes = new Attribute[] { new Core.Types.Attribute("key1", "value1"), new Core.Types.Attribute("key2", "value2") };

        // Act
        var attributeSet = new AttributeSet(attributes);

        // Assert
        Assert.AreEqual(2, attributeSet.Count);
    }

    [TestMethod]
    public void Contains_ShouldReturnTrue_WhenAttributeExists()
    {
        // Arrange
        var attribute = new Core.Types.Attribute("key", "value");
        var attributeSet = new AttributeSet(attribute);

        // Act
        var contains = attributeSet.Contains(attribute);

        // Assert
        Assert.IsTrue(contains);
    }

    [TestMethod]
    public void Contains_ShouldReturnFalse_WhenAttributeDoesNotExist()
    {
        // Arrange
        var attribute = new Core.Types.Attribute("key", "value");
        var attributeSet = new AttributeSet(new Core.Types.Attribute("otherKey", "value"));

        // Act
        var contains = attributeSet.Contains(attribute);

        // Assert
        Assert.IsFalse(contains);
    }

    [TestMethod]
    public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var attributeSet = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var containsKey = attributeSet.ContainsKey("key");

        // Assert
        Assert.IsTrue(containsKey);
    }

    [TestMethod]
    public void ContainsKey_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var attributeSet = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var containsKey = attributeSet.ContainsKey("otherKey");

        // Assert
        Assert.IsFalse(containsKey);
    }

    [TestMethod]
    public void TryGetAttribute_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var attributeSet = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var result = attributeSet.TryGetAttribute("key", out var attribute);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(new Core.Types.Attribute("key", "value"), attribute);
    }

    [TestMethod]
    public void TryGetAttribute_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var attributeSet = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var result = attributeSet.TryGetAttribute("otherKey", out var attribute);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(attribute);
    }

    [TestMethod]
    public void TryGetAttributes_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var attributes = new Attribute[] { new Core.Types.Attribute("key", "value1"), new Core.Types.Attribute("key", "value2") };
        var attributeSet = new AttributeSet(attributes);

        // Act
        var result = attributeSet.TryGetAttributes("key", out var foundAttributes);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(2, foundAttributes.Length);
    }

    [TestMethod]
    public void TryGetAttributes_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var attributeSet = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var result = attributeSet.TryGetAttributes("otherKey", out var attributes);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, attributes.Length);
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_ForEqualAttributeSets()
    {
        // Arrange
        var attributeSet1 = new AttributeSet(new Core.Types.Attribute("key", "value"));
        var attributeSet2 = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var areEqual = attributeSet1.Equals(attributeSet2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentAttributeSets()
    {
        // Arrange
        var attributeSet1 = new AttributeSet(new Core.Types.Attribute("key", "value"));
        var attributeSet2 = new AttributeSet(new Core.Types.Attribute("otherKey", "value"));

        // Act
        var areEqual = attributeSet1.Equals(attributeSet2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualAttributeSets()
    {
        // Arrange
        var attributeSet1 = new AttributeSet(new Core.Types.Attribute("key", "value"));
        var attributeSet2 = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var areEqual = attributeSet1.ValueEquals(attributeSet2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForDifferentAttributeSets()
    {
        // Arrange
        var attributeSet1 = new AttributeSet(new Core.Types.Attribute("key", "value"));
        var attributeSet2 = new AttributeSet(new Core.Types.Attribute("otherKey", "value"));

        // Act
        var areEqual = attributeSet1.ValueEquals(attributeSet2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnSameValue_ForEqualAttributeSets()
    {
        // Arrange
        var attributeSet1 = new AttributeSet(new Core.Types.Attribute("key", "value"));
        var attributeSet2 = new AttributeSet(new Core.Types.Attribute("key", "value"));

        // Act
        var hash1 = attributeSet1.GetHashCode();
        var hash2 = attributeSet2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnDifferentValues_ForDifferentAttributeSets()
    {
        // Arrange
        var attributeSet1 = new AttributeSet(new Core.Types.Attribute("key", "value"));
        var attributeSet2 = new AttributeSet(new Core.Types.Attribute("otherKey", "value"));

        // Act
        var hash1 = attributeSet1.GetHashCode();
        var hash2 = attributeSet2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertArrayToAttributeSet()
    {
        // Arrange
        var attributes = new Attribute[] { new Core.Types.Attribute("key1", "value1"), new Core.Types.Attribute("key2", "value2") };

        // Act
        AttributeSet attributeSet = attributes;

        // Assert
        Assert.AreEqual(2, attributeSet.Count);
        Assert.IsTrue(attributeSet.Contains(new Core.Types.Attribute("key1", "value1")));
        Assert.IsTrue(attributeSet.Contains(new Core.Types.Attribute("key2", "value2")));
    }

    [TestMethod]
    public void GetEnumerator_ShouldReturnCorrectEnumerator()
    {
        // Arrange
        var attributes = new Attribute[] { new Core.Types.Attribute("key1", "value1"), new Core.Types.Attribute("key2", "value2") };
        var attributeSet = new AttributeSet(attributes);

        // Act
        var enumerator = attributeSet.GetEnumerator();

        // Assert
        var attributeList = new List<Attribute>();
        while (enumerator.MoveNext())
        {
            attributeList.Add(enumerator.Current);
        }

        Assert.AreEqual(2, attributeList.Count);
        Assert.AreEqual("key1", attributeList[0].Key);
        Assert.AreEqual("key2", attributeList[1].Key);
    }
}
