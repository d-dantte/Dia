using Axis.Dia.Core.Types;
using System.Collections.Immutable;

namespace Axis.Dia.Core.Tests.Types;

[TestClass]
public class BlobTests
{
    [TestMethod]
    public void Constructor_WithValidBytesAndAttributes_ShouldInitializeBlob()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };
        var attributes = new  Core.Types.Attribute[] { new  Core.Types.Attribute("key", "value") };

        // Act
        var blob = new Blob(bytes, attributes);

        // Assert
        Assert.IsFalse(blob.IsNull);
        Assert.IsFalse(blob.IsDefault);
        Assert.AreEqual(3, blob.Value?.Length);
        Assert.AreEqual(1, blob.Attributes.Count);
        Assert.IsTrue(blob.Attributes.Contains(new  Core.Types.Attribute("key", "value")));
    }

    [TestMethod]
    public void Constructor_WithNullBytes_ShouldSetValueToNull()
    {
        // Act
        var blob = new Blob(null);
        var blob2 = Blob.Null();

        // Assert
        Assert.IsTrue(blob.IsNull);
        Assert.IsTrue(blob2.IsNull);
        Assert.IsTrue(blob.ValueEquals(blob2));
    }

    [TestMethod]
    public void Constructor_WithNoAttributes_ShouldInitializeEmptyAttributes()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };

        // Act
        var blob = new Blob(bytes);

        // Assert
        Assert.AreEqual(0, blob.Attributes.Count);
    }

    [TestMethod]
    public void Default_ShouldReturnBlobWithDefaultState()
    {
        // Act
        var defaultBlob = Blob.Default;

        // Assert
        Assert.IsTrue(defaultBlob.IsDefault);
        Assert.IsTrue(defaultBlob.IsEmpty);
        Assert.IsNull(defaultBlob.Value);
        Assert.AreEqual(AttributeSet.Default, defaultBlob.Attributes);
    }

    [TestMethod]
    public void Null_ShouldReturnBlobWithNullValue()
    {
        // Arrange
        var attributes = new  Core.Types.Attribute[] { new  Core.Types.Attribute("key", "value") };

        // Act
        var nullBlob = Blob.Null(attributes);

        // Assert
        Assert.IsTrue(nullBlob.IsNull);
        Assert.AreEqual(1, nullBlob.Attributes.Count);
    }

    [TestMethod]
    public void IsEmpty_ShouldReturnTrue_WhenValueIsNullOrEmpty()
    {
        // Act
        var emptyBlob = Blob.Of(Array.Empty<byte>());
        var nullBlob = Blob.Null();

        // Assert
        Assert.IsTrue(emptyBlob.IsEmpty);
        Assert.IsTrue(nullBlob.IsEmpty);
    }

    [TestMethod]
    public void IsEmpty_ShouldReturnFalse_WhenBlobHasValue()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };

        // Act
        var blob = Blob.Of(bytes);

        // Assert
        Assert.IsFalse(blob.IsEmpty);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForEqualBlobs()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };
        var blob1 = Blob.Of(bytes);
        var blob2 = Blob.Of(bytes);

        // Act
        var areEqual = blob1.ValueEquals(blob2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_ForDifferentBlobs()
    {
        // Arrange
        var blob1 = Blob.Of(new byte[] { 1, 2, 3 });
        var blob2 = Blob.Of(new byte[] { 4, 5, 6 });

        // Act
        var areEqual = blob1.ValueEquals(blob2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnFalse_WhenBlobIsNullAndOtherIsNot()
    {
        // Arrange
        var nullBlob = Blob.Null();
        var blob = Blob.Of(new byte[] { 1, 2, 3 });

        // Act
        var areEqual = nullBlob.ValueEquals(blob);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void ValueEquals_ShouldReturnTrue_ForBothNullBlobs()
    {
        // Arrange
        var nullBlob1 = Blob.Null();
        var nullBlob2 = Blob.Null();

        // Act
        var areEqual = nullBlob1.ValueEquals(nullBlob2);

        // Assert
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnTrue_ForEqualBlobs()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };
        var blob1 = Blob.Of(bytes);
        var blob2 = Blob.Of(bytes);

        // Act
        var areEqual = blob1.Equals(blob2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentBlobs()
    {
        // Arrange
        var blob1 = Blob.Of(new byte[] { 1, 2, 3 });
        var blob2 = Blob.Of(new byte[] { 4, 5, 6 });

        // Act
        var areEqual = blob1.Equals(blob2);

        // Assert
        Assert.IsFalse(areEqual);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnSameValue_ForEqualBlobs()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };
        var blob1 = Blob.Of(bytes);
        var blob2 = Blob.Of(bytes);

        // Act
        var hash1 = blob1.GetHashCode();
        var hash2 = blob2.GetHashCode();

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_ShouldReturnDifferentValues_ForDifferentBlobs()
    {
        // Arrange
        var blob1 = Blob.Of(new byte[] { 1, 2, 3 });
        var blob2 = Blob.Of(new byte[] { 4, 5, 6 });

        // Act
        var hash1 = blob1.GetHashCode();
        var hash2 = blob2.GetHashCode();

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertArrayToBlob()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };

        // Act
        Blob blob = bytes;

        // Assert
        Assert.AreEqual(3, blob.Value?.Length);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertListToBlob()
    {
        // Arrange
        var byteList = new List<byte> { 1, 2, 3 };

        // Act
        Blob blob = byteList;

        // Assert
        Assert.AreEqual(3, blob.Value?.Length);
    }

    [TestMethod]
    public void ImplicitOperator_ShouldConvertImmutableArrayToBlob()
    {
        // Arrange
        var byteArray = ImmutableArray.Create(new byte[] { 1, 2, 3 });

        // Act
        Blob blob = byteArray;

        // Assert
        Assert.AreEqual(3, blob.Value?.Length);
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var blob = Blob.Of(new byte[] { 1, 2, 3 });

        // Act
        var result = blob.ToString();

        // Assert
        Assert.AreEqual("[@Blob length: 3]", result);
    }

    [TestMethod]
    public void ToString_ShouldReturnWildcardForNullBlob()
    {
        // Arrange
        var nullBlob = Blob.Null();

        // Act
        var result = nullBlob.ToString();

        // Assert
        Assert.AreEqual("[@Blob length: *]", result);
    }
}
