namespace Axis.Dia.Core.Tests.Types;

using Axis.Dia.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

[TestClass]
public class IntegerTests
{
    private static readonly Attribute[] TestAttributes = new Attribute[] {
        Attribute.Of("abcd"),
        Attribute.Of("stuff", "value")
    };

    [TestMethod]
    public void Constructor_WithValueAndAttributes_ShouldSetFields()
    {
        var value = new BigInteger(123);
        var integer = new Integer(value, TestAttributes);

        Assert.AreEqual(value, integer.Value);
        CollectionAssert.AreEqual(TestAttributes, integer.Attributes.ToArray());
    }

    [TestMethod]
    public void Constructor_WithNullValue_ShouldInitializeToNull()
    {
        var integer = new Integer(null, TestAttributes);

        Assert.IsTrue(integer.IsNull);
        Assert.IsFalse(integer.IsDefault);
    }

    [TestMethod]
    public void DefaultProperty_ShouldReturnDefaultInstance()
    {
        var defaultInteger = Integer.Default;

        Assert.IsTrue(defaultInteger.IsDefault);
        Assert.IsTrue(defaultInteger.IsNull);
    }

    [TestMethod]
    public void IsNull_ShouldReturnTrue_WhenValueIsNull()
    {
        var integer = new Integer(null, TestAttributes);
        Assert.IsTrue(integer.IsNull);
    }

    [TestMethod]
    public void ToString_ShouldReturnFormattedString()
    {
        var value = new BigInteger(123);
        var integer = new Integer(value, TestAttributes);

        Assert.AreEqual($"[@{DiaType.Int} {value}]", integer.ToString());
    }

    [TestMethod]
    public void Equals_ValidComparison_ShouldReturnTrue()
    {
        var value = new BigInteger(123);
        var integer1 = new Integer(value, TestAttributes);
        var integer2 = new Integer(value, TestAttributes);

        Assert.IsTrue(integer1.Equals(integer2));
        Assert.IsTrue(integer1 == integer2);
    }

    [TestMethod]
    public void Equals_DifferentValues_ShouldReturnFalse()
    {
        var integer1 = new Integer(new BigInteger(123), TestAttributes);
        var integer2 = new Integer(new BigInteger(456), TestAttributes);

        Assert.IsFalse(integer1.Equals(integer2));
        Assert.IsFalse(integer1 == integer2);
    }

    [TestMethod]
    public void ValueEquals_EqualInstances_ShouldReturnTrue()
    {
        var value = new BigInteger(123);
        var integer1 = new Integer(value, TestAttributes);
        var integer2 = new Integer(value, TestAttributes);

        Assert.IsTrue(integer1.ValueEquals(integer2));
    }

    [TestMethod]
    public void ValueEquals_DifferentAttributes_ShouldReturnFalse()
    {
        var value = new BigInteger(123);
        var integer1 = new Integer(value, TestAttributes);
        var integer2 = new Integer(value, new Attribute[] { /* Different attributes */ });

        Assert.IsFalse(integer1.ValueEquals(integer2));
    }

    [TestMethod]
    public void GetHashCode_SameValues_ShouldReturnSameHash()
    {
        var value = new BigInteger(123);
        var integer1 = new Integer(value, TestAttributes);
        var integer2 = new Integer(value, TestAttributes);

        Assert.AreEqual(integer1.GetHashCode(), integer2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_DifferentValues_ShouldReturnDifferentHash()
    {
        var integer1 = new Integer(new BigInteger(123), TestAttributes);
        var integer2 = new Integer(new BigInteger(456), TestAttributes);

        Assert.AreNotEqual(integer1.GetHashCode(), integer2.GetHashCode());
    }

    [TestMethod]
    public void ImplicitConversion_ShouldConvertFromBigInteger()
    {
        BigInteger value = 123;
        Integer integer = value;

        Assert.AreEqual(value, integer.Value);
    }
}