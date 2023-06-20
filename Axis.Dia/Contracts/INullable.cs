using Axis.Dia.Types;

namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Defines the nullability contract for Dia types
    /// </summary>
    /// <typeparam name="TSelf"></typeparam>
    public interface INullable<TSelf> where TSelf : INullable<TSelf>
    {
        /// <summary>
        /// Creates a null instance of the type
        /// </summary>
        /// <param name="attributes">any available annotation</param>
        /// <returns>The newly created null instance</returns>
        static abstract TSelf Null(params Annotation[] attributes);
    }
}
