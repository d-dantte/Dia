namespace Axis.Dia.Core
{
    public interface IDefaultContract<TSelf>
    where TSelf : struct, IDefaultContract<TSelf>
    {
        /// <summary>
        /// Indicates if this is the default value for the type
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// Gets the default value for the type - usually the null value, with no attributes
        /// </summary>
        static abstract TSelf Default { get; }
    }
}
