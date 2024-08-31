namespace Axis.Dia.Core.Contracts
{
    /// <summary>
    /// Contract specifying equality test and hashcode retrieval for the encapsulated references.
    /// </summary>
    public interface IRefEquatable<TSelf>
        where TSelf : IRefEquatable<TSelf>
    {
        /// <summary>
        /// Checks if the encpasulated reference(s) are equal. Typically a <see cref="Object.ReferenceEquals(object?, object?)"/> is used
        /// for this test.
        /// </summary>
        /// <param name="other">The other instance to test</param>
        /// <returns>True if all encapsulated references are equal, false otherwise.</returns>
        bool RefEquals(TSelf other);

        /// <summary>
        /// Gets a hash evaluated from a combination of the hash for all encapsulated references.
        /// </summary>
        /// <returns></returns>
        int RefHash();
    }
}
