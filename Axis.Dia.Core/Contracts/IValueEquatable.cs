namespace Axis.Dia.Core.Contracts
{
    /// <summary>
    /// Contract specifying equality test and hashcode retrieval for the encapsulated values.
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public interface IValueEquatable<TSelf>
        where TSelf : IValueEquatable<TSelf>
    {
        /// <summary>
        /// Tests that the encapsulated values are equal
        /// </summary>
        /// <param name="other">The instance to test</param>
        /// <returns>True if values are equal, false otherwise</returns>
        bool ValueEquals(TSelf other);

        /// <summary>
        /// Combined has of all encapsulated values
        /// </summary>
        int ValueHash();
    }
}
