namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Contract for <see cref="IDiaValue"/> instances to test equality of values, without comparing annotations
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public interface IValueEquatable<TSelf>
    where TSelf : IValueEquatable<TSelf>, IDiaValue
    {
        /// <summary>
        /// Evaluates equality based only on the encapsulated value - i.e, annotations are
        /// not considered.
        /// </summary>
        /// <param name="other">the other value</param>
        /// <returns>true if the encapsulated values are equal, false otherwise</returns>
        bool ValueEquals(TSelf other);
    }
}
