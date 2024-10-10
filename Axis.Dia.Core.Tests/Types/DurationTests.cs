using Axis.Dia.Core.Types;

namespace Axis.Dia.Core.Tests.Types;

[TestClass]
public class DurationTests
{
    [TestMethod]
    public void Constructor_WithTimeSpan_ShouldInitializeDuration()
    {
        // Arrange
        var timeSpan = TimeSpan.FromMinutes(5);
        var attributes = new Core.Types.Attribute[] { new Core.Types.Attribute("unit", "minutes") };

        // Act
        var duration = new Duration(timeSpan, attributes);

        // Assert
        Assert.IsFalse(duration.IsNull);
        Assert.IsFalse(duration.IsDefault);
        Assert.AreEqual(timeSpan, duration.Value);
        Assert.AreEqual(1, duration.Attributes.Count);
        Assert.IsTrue(duration.Attributes.Contains(new Core.Types.Attribute("unit", "minutes")));
    }

    [TestMethod]
    public void Constructor_WithNullTimeSpan_ShouldSetValueToNull()
    {
        // Arrange
        var attributes = new Core.Types.Attribute[] { new Core.Types.Attribute("unit", "minutes") };

        // Act
        var duration = new Duration((TimeSpan?)null, attributes);

        // Assert
        Assert.IsTrue(duration.IsNull);
        Assert.IsNull(duration.Value);
        Assert.AreEqual(1, duration.Attributes.Count);
        Assert.AreEqual("minutes", duration.Attributes.First().Value);
    }

    [TestMethod]
    public void Constructor_WithNanoSeconds_ShouldInitializeDuration()
    {
        // Arrange
        long nanoSeconds = 300000000000; // 5 minutes in nanoseconds

        // Act
        var duration = new Duration(nanoSeconds);

        // Assert
        Assert.IsFalse(duration.IsNull);
        Assert.AreEqual(nanoSeconds, duration.NanoSeconds);
        Assert.AreEqual(TimeSpan.FromMinutes(5), duration.Value);
    }

    [TestMethod]
    public void Default_ShouldReturnDurationWithDefaultState()
    {
        // Act
        var defaultDuration = Duration.Default;

        // Assert
        Assert.IsTrue(defaultDuration.IsDefault);
        Assert.IsTrue(defaultDuration.IsNull);
        Assert.IsNull(defaultDuration.NanoSeconds);
        Assert.AreEqual(AttributeSet.Default, defaultDuration.Attributes);
    }

    [TestMethod]
    public void Null_ShouldReturnDurationWithNullValue()
    {
        // Arrange
        var attributes = new Core.Types.Attribute[] { new Core.Types.Attribute("unit", "minutes") };

        // Act
        var nullDuration = Duration.Null(attributes);

        // Assert
        Assert.IsTrue(nullDuration.IsNull);
        Assert.AreEqual(1, nullDuration.Attributes.Count);
        Assert.AreEqual("minutes", nullDuration.Attributes.First().Value);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualDurations()
    {
        // Arrange
        var duration1 = new Duration(TimeSpan.FromMinutes(5));
        var duration2 = new Duration(TimeSpan.FromMinutes(5));

        // Act
        var areEqual = duration1.ValueEquals(duration2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForDifferentDurations()
    {
        // Arrange
        var duration1 = new Duration(TimeSpan.FromMinutes(5));
        var duration2 = new Duration(TimeSpan.FromMinutes(10));

        // Act
        var areEqual = duration1.ValueEquals(duration2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForBothNullDurations()
    {
        // Arrange
        var nullDuration1 = Duration.Null();
        var nullDuration2 = Duration.Null();

        // Act
        var areEqual = nullDuration1.ValueEquals(nullDuration2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_ForEqualDurations()
    {
        // Arrange
        var duration1 = new Duration(TimeSpan.FromMinutes(5));
        var duration2 = new Duration(TimeSpan.FromMinutes(5));

        // Act
        var areEqual = duration1.Equals(duration2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentDurations()
    {
        // Arrange
        var duration1 = new Duration(TimeSpan.FromMinutes(5));
        var duration2 = new Duration(TimeSpan.FromMinutes(10));

        // Act
        var areEqual = duration1.Equals(duration2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnSameValue_ForEqualDurations()
    {
        // Arrange
        var duration1 = new Duration(TimeSpan.FromMinutes(5));
        var duration2 = new Duration(TimeSpan.FromMinutes(5));

        // Act
        var hash1 = duration1.GetHashCode();
        var hash2 = duration2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnDifferentValues_ForDifferentDurations()
    {
        // Arrange
        var duration1 = new Duration(TimeSpan.FromMinutes(5));
        var duration2 = new Duration(TimeSpan.FromMinutes(10));

        // Act
        var hash1 = duration1.GetHashCode();
        var hash2 = duration2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertTimeSpanToDuration()
    {
        // Arrange
        TimeSpan timeSpan = TimeSpan.FromMinutes(5);

        // Act
        Duration duration = timeSpan;

        // Assert
        Assert.AreEqual(timeSpan, duration.Value);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertNanoSecondsToDuration()
    {
        // Arrange
        long nanoSeconds = 300000000000; // 5 minutes in nanoseconds

        // Act
        Duration duration = nanoSeconds;

        // Assert
        Assert.AreEqual(nanoSeconds, duration.NanoSeconds);
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_ForNonNullDuration()
    {
        // Arrange
        var duration = new Duration(TimeSpan.FromMinutes(5));

        // Act
        var result = duration.ToString();

        // Assert
        Assert.AreEqual("[@Duration 300000000000]", result); // 5 minutes in nanoseconds
    }

    [TestMethod]
    public void ToString_ShouldReturnWildcardForNullDuration()
    {
        // Arrange
        var nullDuration = Duration.Null();

        // Act
        var result = nullDuration.ToString();

        // Assert
        Assert.AreEqual("[@Duration *]", result);
    }
}

