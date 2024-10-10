
using Axis.Dia.Core.Types;
using Axis.Luna.Extensions;

using Axis.Luna.Numerics;
using System.Collections.Immutable;

namespace Axis.Dia.Core.Tests.Types;

[TestClass]
public class DecimalTests
{
    [TestMethod]
    public void Constructor_WithValidBigDecimalAndAttributes_ShouldInitializeDecimal()
    {
        // Arrange
        BigDecimal? value = new BigDecimal(123.45m);
        var attributes = new Core.Types.Attribute[] { new Core.Types.Attribute("key", "value") };

        // Act
        var decimalValue = new Core.Types.Decimal(value, attributes);

        // Assert
        Assert.IsFalse(decimalValue.IsNull);
        Assert.IsFalse(decimalValue.IsDefault);
        Assert.AreEqual(value, decimalValue.Value);
        Assert.AreEqual(1, decimalValue.Attributes.Count);
        Assert.IsTrue(decimalValue.Attributes.Contains(new Core.Types.Attribute("key", "value")));
    }

    [TestMethod]
    public void Constructor_WithNullBigDecimal_ShouldSetValueToNull()
    {
        // Arrange
        var attributes = new Core.Types.Attribute[] { new Core.Types.Attribute("key", "value") };

        // Act
        var decimalValue = new Core.Types.Decimal(null, attributes);

        // Assert
        Assert.IsTrue(decimalValue.IsNull);
        Assert.AreEqual(1, decimalValue.Attributes.Count);
        Assert.AreEqual("value", decimalValue.Attributes.First().Value);
    }

    [TestMethod]
    public void Constructor_WithNoAttributes_ShouldInitializeEmptyAttributes()
    {
        // Act
        var decimalValue = new Core.Types.Decimal(new BigDecimal(123.45m));

        // Assert
        Assert.AreEqual(0, decimalValue.Attributes.Count);
    }

    [TestMethod]
    public void Default_ShouldReturnDecimalWithDefaultState()
    {
        // Act
        var defaultDecimal = Core.Types.Decimal.Default;

        // Assert
        Assert.IsTrue(defaultDecimal.IsDefault);
        Assert.IsTrue(defaultDecimal.IsNull);
        Assert.IsNull(defaultDecimal.Value);
        Assert.AreEqual(AttributeSet.Default, defaultDecimal.Attributes);
    }

    [TestMethod]
    public void Null_ShouldReturnDecimalWithNullValue()
    {
        // Arrange
        var attributes = new Core.Types.Attribute[] { new Core.Types.Attribute("key", "value") };

        // Act
        var nullDecimal = Core.Types.Decimal.Null(attributes);

        // Assert
        Assert.IsTrue(nullDecimal.IsNull);
        Assert.AreEqual(1, nullDecimal.Attributes.Count);
        Assert.AreEqual("value", nullDecimal.Attributes.First().Value);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualDecimals()
    {
        // Arrange
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m));
        var decimal2 = new Core.Types.Decimal(new BigDecimal(123.45m));

        // Act
        var areEqual = decimal1.ValueEquals(decimal2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForDifferentDecimals()
    {
        // Arrange
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m));
        var decimal2 = new Core.Types.Decimal(new BigDecimal(543.21m));

        // Act
        var areEqual = decimal1.ValueEquals(decimal2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_WhenDecimalIsNullAndOtherIsNot()
    {
        // Arrange
        var nullDecimal = Core.Types.Decimal.Null();
        var decimalValue = new Core.Types.Decimal(new BigDecimal(123.45m));

        // Act
        var areEqual = nullDecimal.ValueEquals(decimalValue);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForBothNullDecimals()
    {
        // Arrange
        var nullDecimal1 = Core.Types.Decimal.Null();
        var nullDecimal2 = Core.Types.Decimal.Null();

        // Act
        var areEqual = nullDecimal1.ValueEquals(nullDecimal2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_ForEqualDecimals()
    {
        // Arrange
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m));
        var decimal2 = new Core.Types.Decimal(new BigDecimal(123.45m));

        // Act
        var areEqual = decimal1.Equals(decimal2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentDecimals()
    {
        // Arrange
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m));
        var decimal2 = new Core.Types.Decimal(new BigDecimal(543.21m));

        // Act
        var areEqual = decimal1.Equals(decimal2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnSameValue_ForEqualDecimals()
    {
        // Arrange
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m));
        var decimal2 = new Core.Types.Decimal(new BigDecimal(123.45m));

        // Act
        var hash1 = decimal1.GetHashCode();
        var hash2 = decimal2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnDifferentValues_ForDifferentDecimals()
    {
        // Arrange
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m));
        var decimal2 = new Core.Types.Decimal(new BigDecimal(543.21m));

        // Act
        var hash1 = decimal1.GetHashCode();
        var hash2 = decimal2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertBigDecimalToDecimal()
    {
        // Arrange
        BigDecimal? value = new BigDecimal(123.45m);

        // Act
        Core.Types.Decimal decimalValue = value;

        // Assert
        Assert.AreEqual(value, decimalValue.Value);
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_ForNonNullDecimal()
    {
        // Arrange
        var decimalValue = new Core.Types.Decimal(new BigDecimal(123.45m));

        // Act
        var result = decimalValue.ToString();

        // Assert
        Assert.AreEqual("[@Decimal 1.2345E2]", result);
    }

    [TestMethod]
    public void ToString_ShouldReturnWildcardForNullDecimal()
    {
        // Arrange
        var nullDecimal = Core.Types.Decimal.Null();

        // Act
        var result = nullDecimal.ToString();

        // Assert
        Assert.AreEqual("[@Decimal *]", result);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualDecimalsWithAttributes()
    {
        // Arrange
        var attributes = new Core.Types.Attribute[] { new Core.Types.Attribute("key", "value") };
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m), attributes);
        var decimal2 = new Core.Types.Decimal(new BigDecimal(123.45m), attributes);

        // Act
        var areEqual = decimal1.ValueEquals(decimal2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForDecimalsWithDifferentAttributes()
    {
        // Arrange
        var decimal1 = new Core.Types.Decimal(new BigDecimal(123.45m), new Core.Types.Attribute("key", "value1"));
        var decimal2 = new Core.Types.Decimal(new BigDecimal(123.45m), new Core.Types.Attribute("key", "value2"));

        // Act
        var areEqual = decimal1.ValueEquals(decimal2);

        // Assert
        Assert.IsFalse(areEqual);
    }
}
