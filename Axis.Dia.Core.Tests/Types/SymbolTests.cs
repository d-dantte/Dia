namespace Axis.Dia.Core.Tests.Types;

using Axis.Luna.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


[TestClass]
public class SymbolTests
{
    private static readonly Core.Types.Attribute[] TestAttributes = new Core.Types.Attribute[] { Core.Types.Attribute.Of("abcd") };

    [TestMethod]
    public void Constructor_WithValueAndAttributes_ShouldSetFields()
    {
        // Arrange
        var value = "Hello, World!";
        var stringValue = new Core.Types.Symbol(value, TestAttributes);

        // Act
        var actualValue = stringValue.Value;
        var actualAttributes = stringValue.Attributes.ToArray();

        // Assert
        Assert.AreEqual(value, actualValue);
        CollectionAssert.AreEqual(TestAttributes, actualAttributes);
    }

    [TestMethod]
    public void Constructor_WithNullValue_ShouldInitializeToNull()
    {
        // Arrange
        var stringValue = new Core.Types.Symbol(null, TestAttributes);

        // Act
        var isNull = stringValue.IsNull;

        // Assert
        Assert.IsTrue(isNull);
        Assert.IsFalse(stringValue.IsDefault);
    }

    [TestMethod]
    public void DefaultProperty_ShouldReturnDefaultInstance()
    {
        // Arrange
        var defaultSymbol = Core.Types.Symbol.Default;

        // Act
        var isDefault = defaultSymbol.IsDefault;
        var isNull = defaultSymbol.IsNull;

        // Assert
        Assert.IsTrue(isDefault);
        Assert.IsTrue(isNull);
    }

    [TestMethod]
    public void IsNull_ShouldReturnTrue_WhenValueIsNull()
    {
        // Arrange
        var stringValue = new Core.Types.Symbol(null, TestAttributes);

        // Act
        var isNull = stringValue.IsNull;

        // Assert
        Assert.IsTrue(isNull);
    }

    [TestMethod]
    public void ToSymbol_ShouldReturnFormattedSymbol()
    {
        // Arrange
        var value = "Hello, World!";
        var stringValue = new Core.Types.Symbol(value, TestAttributes);

        // Act
        var result = stringValue.ToString();

        // Assert
        Assert.IsTrue(result.StartsWith($"[@{DiaType.Symbol} length: {value.Length}, value: "));
    }

    [TestMethod]
    public void Equals_ValidComparison_ShouldReturnTrue()
    {
        // Arrange
        var value = "Hello, World!";
        var stringValue1 = new Core.Types.Symbol(value, TestAttributes);
        var stringValue2 = new Core.Types.Symbol(value, TestAttributes);

        // Act
        var areEqual = stringValue1.Equals(stringValue2);
        var areEqualOperator = stringValue1 == stringValue2;

        // Assert
        Assert.IsTrue(areEqual);
        Assert.IsTrue(areEqualOperator);
    }

    [TestMethod]
    public void Equals_DifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var stringValue1 = new Core.Types.Symbol("Hello, World!", TestAttributes);
        var stringValue2 = new Core.Types.Symbol("Goodbye, World!", TestAttributes);

        // Act
        var areEqual = stringValue1.Equals(stringValue2);
        var areEqualOperator = stringValue1 == stringValue2;

        // Assert
        Assert.IsFalse(areEqual);
        Assert.IsFalse(areEqualOperator);
    }

    [TestMethod]
    public void ValueEquals_EqualInstances_ShouldReturnTrue()
    {
        // Arrange
        var value = "Hello, World!";
        var stringValue1 = new Core.Types.Symbol(value, TestAttributes);
        var stringValue2 = new Core.Types.Symbol(value, TestAttributes);

        // Act
        var areEqual = stringValue1.ValueEquals(stringValue2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_DifferentAttributes_ShouldReturnFalse()
    {
        // Arrange
        var value = "Hello, World!";
        var stringValue1 = new Core.Types.Symbol(value, TestAttributes);
        var stringValue2 = new Core.Types.Symbol(value, new Core.Types.Attribute[] { /* Different attributes */ });

        // Act
        var areEqual = stringValue1.ValueEquals(stringValue2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void GetHashCode_SameValues_ShouldReturnSameHash()
    {
        // Arrange
        var value = "Hello, World!";
        var stringValue1 = new Core.Types.Symbol(value, TestAttributes);
        var stringValue2 = new Core.Types.Symbol(value, TestAttributes);

        // Act
        var hash1 = stringValue1.GetHashCode();
        var hash2 = stringValue2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_DifferentValues_ShouldReturnDifferentHash()
    {
        // Arrange
        var stringValue1 = new Core.Types.Symbol("Hello, World!", TestAttributes);
        var stringValue2 = new Core.Types.Symbol("Goodbye, World!", TestAttributes);

        // Act
        var hash1 = stringValue1.GetHashCode();
        var hash2 = stringValue2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void ImplicitConversion_ShouldConvertFromSymbol()
    {
        // Arrange
        string value = "Hello, World!";
        Core.Types.Symbol stringValue = value;

        // Act
        var actualValue = stringValue.Value;

        // Assert
        Assert.AreEqual(value, actualValue);
    }
}