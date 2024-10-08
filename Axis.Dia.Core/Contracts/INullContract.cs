using Axis.Dia.Core.Types;

namespace Axis.Dia.Core.Contracts
{
    public interface INullable
    {
        /// <summary>
        /// Indicates if this is a null value for the type
        /// </summary>
        bool IsNull { get; }
    }

    public interface INullContract<TSelf>
        : INullable
        where TSelf : struct, INullContract<TSelf>
    {
        /// <summary>
        /// Gets the null value for the type - usually the null value, with the possibility of attributes
        /// <p/>
        /// NOTE: Null == Default when there are no attributes.
        /// </summary>
        static abstract TSelf Null(params Types.Attribute[] attributes);
    }
}
