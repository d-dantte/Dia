using Axis.Dia.Core.Types;

namespace Axis.Dia.Core
{
    public interface INullValueFactory<TSelf>
    where TSelf : INullValueFactory<TSelf>
    {
        /// <summary>
        /// Creates a null instance of the type
        /// </summary>
        /// <param name="annotations">any available annotation</param>
        /// <returns>The newly created null instance</returns>
        static abstract TSelf Null(params Types.Attribute[] annotations);
    }
}
