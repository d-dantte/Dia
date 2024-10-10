namespace Axis.Dia.Core.Tests.Types;

using Axis.Dia.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class SequenceTests
{
    private readonly DiaValue _defaultValue = DiaValue.Default;
    private readonly DiaValue _testValue1 = Symbol.Of("value1");
    private readonly DiaValue _testValue2 = Timestamp.Of(DateTimeOffset.Now);
    private readonly DiaValue _testValue3 = Core.Types.Boolean.True;

    private Core.Types.Attribute[] CreateTestAttributes()
    {
        return new Core.Types.Attribute[] {
            Core.Types.Attribute.Of("abcd")
        };
    }

    [TestMethod]
    public void Constructor_WithItemsAndAttributes_ShouldSetFields()
    {
        // Arrange
        var attributes = CreateTestAttributes();
        var items = new List<DiaValue> { _testValue1, _testValue2 };

        // Act
        var sequence = new Sequence(items, attributes);

        // Assert
        Assert.AreEqual(2, sequence.Count);
        CollectionAssert.AreEqual(items.ToArray(), sequence.Value!.ToArray());
        Assert.IsTrue(AttributeSet.Of(attributes).ValueEquals(sequence.Attributes));
    }

    [TestMethod]
    public void Constructor_WithNullItems_ShouldInitializeEmptySequence()
    {
        // Arrange
        var attributes = CreateTestAttributes();

        // Act
        var sequence = new Sequence(null, attributes);

        // Assert
        Assert.AreEqual(0, sequence.Count);
        Assert.IsTrue(sequence.IsEmpty);
        Assert.IsTrue(sequence.Attributes.ValueEquals(attributes));
    }

    [TestMethod]
    public void DefaultProperty_ShouldReturnDefaultInstance()
    {
        // Arrange
        var defaultSequence = Sequence.Default;

        // Act
        var isDefault = defaultSequence.IsDefault;
        var isNull = defaultSequence.IsNull;

        // Assert
        Assert.IsTrue(isDefault);
        Assert.IsTrue(isNull);
    }

    [TestMethod]
    public void IsDefault_ShouldReturnTrue_WhenItemsAndAttributesAreDefault()
    {
        // Arrange
        var sequence = Sequence.Default;

        // Act
        var isDefault = sequence.IsDefault;

        // Assert
        Assert.IsTrue(isDefault);
    }

    [TestMethod]
    public void IsNull_ShouldReturnTrue_WhenItemsAreNull()
    {
        // Arrange
        var sequence = Sequence.Null();

        // Act
        var isNull = sequence.IsNull;

        // Assert
        Assert.IsTrue(isNull);
    }

    [TestMethod]
    public void IsEmpty_ShouldReturnTrue_WhenNoItems()
    {
        // Arrange
        var sequence = new Sequence();

        // Act
        var isEmpty = sequence.IsEmpty;

        // Assert
        Assert.IsTrue(isEmpty);
    }

    [TestMethod]
    public void AddItem_ShouldIncreaseCount()
    {
        // Arrange
        var sequence = new Sequence();
        var initialCount = sequence.Count;

        // Act
        sequence.Add(_testValue1);

        // Assert
        Assert.AreEqual(initialCount + 1, sequence.Count);
        Assert.AreEqual(_testValue1, sequence.Value!.First());
    }

    [TestMethod]
    public void RemoveItem_ShouldDecreaseCount()
    {
        // Arrange
        var sequence = new Sequence(_testValue1, _testValue2);
        var initialCount = sequence.Count;

        // Act
        sequence.Remove(_testValue1);

        // Assert
        Assert.AreEqual(initialCount - 1, sequence.Count);
        Assert.IsFalse(sequence.Value!.Contains(_testValue1));
    }

    [TestMethod]
    public void ValueEquals_SameItemsAndAttributes_ShouldReturnTrue()
    {
        // Arrange
        var attributes = CreateTestAttributes();
        var sequence1 = new Sequence(new[] { _testValue1, _testValue2 }, attributes);
        var sequence2 = new Sequence(new[] { _testValue1, _testValue2 }, attributes);

        // Act
        var areEqual = sequence1.ValueEquals(sequence2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_DifferentItems_ShouldReturnFalse()
    {
        // Arrange
        var sequence1 = new Sequence(new[] { _testValue1 });
        var sequence2 = new Sequence(new[] { _testValue2 });

        // Act
        var areEqual = sequence1.ValueEquals(sequence2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void TryRemoveAt_ValidIndex_ShouldReturnTrueAndRemoveItem()
    {
        // Arrange
        var sequence = new Sequence(_testValue1, _testValue2);

        // Act
        var result = sequence.TryRemoveAt(0, out var removedItem);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(_testValue1, removedItem);
        Assert.AreEqual(1, sequence.Count);
        Assert.IsFalse(sequence.Value!.Contains(_testValue1));
    }

    [TestMethod]
    public void TryRemoveAt_InvalidIndex_ShouldReturnFalse()
    {
        // Arrange
        var sequence = new Sequence(_testValue1, _testValue2);

        // Act
        var result = sequence.TryRemoveAt(5, out var removedItem);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(removedItem);
    }

    [TestMethod]
    public void GetEnumerator_ShouldReturnAllItems()
    {
        // Arrange
        var sequence = new Sequence(new[] { _testValue1, _testValue2 });

        // Act
        var items = sequence.ToList();

        // Assert
        CollectionAssert.AreEqual(new[] { _testValue1, _testValue2 }, items);
    }

    [TestMethod]
    public void ImplicitConversion_ShouldConvertFromDiaValueArray()
    {
        // Arrange
        var items = new[] { _testValue1, _testValue2 };

        // Act
        Sequence sequence = items;

        // Assert
        Assert.AreEqual(items.Length, sequence.Count);
        CollectionAssert.AreEqual(items, sequence.Value!.ToArray());
    }

    [TestMethod]
    public void Set_ValidIndex_ShouldUpdateItem()
    {
        // Arrange
        var sequence = new Sequence(_testValue1, _testValue2);

        // Act
        sequence.Set(1, _testValue3);

        // Assert
        Assert.AreEqual(_testValue3, sequence[1]);
    }

    [TestMethod]
    public void Set_InvalidValue_ShouldThrowException()
    {
        // Arrange
        var sequence = new Sequence(_testValue1, _testValue2);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => sequence.Set(1, _defaultValue));
    }
}