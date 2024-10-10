namespace Axis.Dia.Core.Tests.Types;

using Axis.Dia.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

[TestClass]
public class RecordTests
{
    [TestMethod]
    public void DefaultRecord_ShouldBeDefault()
    {
        var record = Record.Default;
        Assert.IsTrue(record.IsDefault);
    }

    [TestMethod]
    public void RecordWithProperties_ShouldNotBeDefault()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);
        Assert.IsFalse(record.IsDefault);
        Assert.AreEqual(1, record.Count);
    }

    [TestMethod]
    public void AddingProperties_ShouldIncreaseCount()
    {
        var record = new Record();
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));

        record.Add(property);

        Assert.AreEqual(1, record.Count);
        Assert.IsFalse(record.IsEmpty);
    }

    [TestMethod]
    public void RemovingProperties_ShouldDecreaseCount()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        record.Remove(property);

        Assert.AreEqual(0, record.Count);
        Assert.IsTrue(record.IsEmpty);
    }

    [TestMethod]
    public void AddingAndRemovingMultipleProperties()
    {
        var record = new Record();
        var property1 = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var property2 = new Record.Property("age", new DiaValue(30));

        record.Add(property1);
        record.Add(property2);

        Assert.AreEqual(2, record.Count);

        record.Remove(property1);
        Assert.AreEqual(1, record.Count);

        record.Remove(property2);
        Assert.AreEqual(0, record.Count);
    }

    [TestMethod]
    public void AddingDuplicateProperty_ShouldOverwrite()
    {
        var record = new Record();
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var newProperty = new Record.Property("name", new DiaValue(Symbol.Of("Jane Doe")));

        record.Add(property);
        record.Add(newProperty);

        Assert.AreEqual(1, record.Count);
        Assert.AreEqual(newProperty.Value, record["name"]);
    }

    [TestMethod]
    public void SettingProperty_ShouldUpdateValue()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        var updatedValue = new DiaValue(Symbol.Of("Jane Doe"));
        record.SetProperty("name", updatedValue);

        Assert.AreEqual(updatedValue, record["name"]);
    }

    [TestMethod]
    public void GettingProperty_ShouldReturnValue()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.AreEqual(property.Value, record["name"]);
    }

    [TestMethod]
    public void ContainsProperty_ShouldReturnTrueForExisting()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.IsTrue(record.ContainsProperty("name"));
    }

    [TestMethod]
    public void ContainsProperty_ShouldReturnFalseForNonExisting()
    {
        var record = new Record();
        Assert.IsFalse(record.ContainsProperty("name"));
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrueForEqualRecords()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record1 = new Record(property);
        var record2 = new Record(property);

        Assert.IsTrue(record1.ValueEquals(record2));
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalseForDifferentRecords()
    {
        var record1 = new Record(new Record.Property("name", new DiaValue(Symbol.Of("John Doe"))));
        var record2 = new Record(new Record.Property("name", new DiaValue(Symbol.Of("Jane Doe"))));

        Assert.IsFalse(record1.ValueEquals(record2));
    }

    [TestMethod]
    public void RefEquals_ShouldReturnTrueForSameReference()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.IsTrue(record.RefEquals(record));
    }

    [TestMethod]
    public void RefEquals_ShouldReturnFalseForDifferentInstances()
    {
        var record1 = new Record(new Record.Property("name", new DiaValue(Symbol.Of("John Doe"))));
        var record2 = new Record(new Record.Property("name", new DiaValue(Symbol.Of("John Doe"))));

        Assert.IsFalse(record1.RefEquals(record2));
    }

    [TestMethod]
    public void ToString_ShouldReturnExpectedFormat()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        var expectedOutput = $"[@{record.Type} atts: 0, props: 1]";
        Assert.AreEqual(expectedOutput, record.ToString());

        record = Record.Default;
        expectedOutput = $"[@{record.Type} atts: *, props: *]";
        Assert.AreEqual(expectedOutput, record.ToString());

    }

    [TestMethod]
    public void TryGet_ShouldReturnTrueAndValueForExistingProperty()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.IsTrue(record.TryGet("name", out var value));
        Assert.AreEqual(property.Value, value);
    }

    [TestMethod]
    public void TryGet_ShouldReturnFalseForNonExistingProperty()
    {
        var record = new Record();

        Assert.IsFalse(record.TryGet("name", out var value));
        Assert.IsNull(value);
    }

    [TestMethod]
    public void AddAll_ShouldAddMultipleProperties()
    {
        var record = new Record();
        var property1 = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var property2 = new Record.Property("age", new DiaValue(30));

        record.AddAll(property1, property2);

        Assert.AreEqual(2, record.Count);
    }

    [TestMethod]
    public void RemoveAll_ShouldRemoveMultipleProperties()
    {
        var property1 = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var property2 = new Record.Property("age", new DiaValue(30));
        var record = new Record(property1, property2);

        record.RemoveAll(property1, property2);

        Assert.AreEqual(0, record.Count);
    }

    [TestMethod]
    public void AddItem_ShouldThrowExceptionForDefaultRecord()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));

        Assert.ThrowsException<InvalidOperationException>(
            () => Record.Default.AddItem(Record.Property.Default));
    }

    [TestMethod]
    public void SetProperty_ShouldThrowExceptionForDefaultRecord()
    {
        var record = Record.Default;

        Assert.ThrowsException<InvalidOperationException>(
            () => record.SetProperty("name", new DiaValue(Symbol.Of("John Doe"))));
    }

    [TestMethod]
    public void SetProperty_ShouldThrowExceptionForDefaultPropertyName()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.ThrowsException<ArgumentException>(() => record.SetProperty(new Record.PropertyName(), new DiaValue(Symbol.Of("Value"))));
    }

    [TestMethod]
    public void SetProperty_ShouldThrowExceptionForDefaultValue()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.ThrowsException<ArgumentException>(() => record.SetProperty("name", DiaValue.Default));
    }

    [TestMethod]
    public void TryAdd_ShouldReturnTrueForNewProperty()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record();

        Assert.IsTrue(record.TryAdd(property));
        Assert.AreEqual(1, record.Count);
    }

    [TestMethod]
    public void TryAdd_ShouldReturnFalseForExistingProperty()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.IsFalse(record.TryAdd(property));
    }

    [TestMethod]
    public void TryRemove_ShouldReturnTrueAndRemoveExistingProperty()
    {
        var property = new Record.Property("name", new DiaValue(Symbol.Of("John Doe")));
        var record = new Record(property);

        Assert.IsTrue(record.TryRemove("name", out var removedProperty));
        Assert.AreEqual(property, removedProperty);
        Assert.AreEqual(0, record.Count);
    }

    [TestMethod]
    public void TryRemove_ShouldReturnFalseForNonExistingProperty()
    {
        var record = new Record();

        Assert.IsFalse(record.TryRemove("name", out var removedProperty));
        Assert.IsTrue(removedProperty.IsDefault);
    }
}
