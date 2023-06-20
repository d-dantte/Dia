namespace Axis.Dia.Contracts
{
    public interface IValueEquatable<TSelf, TValue>
    where TSelf : IValueEquatable<TSelf, TValue>, IDiaValue
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
