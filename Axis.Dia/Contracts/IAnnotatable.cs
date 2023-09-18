using Axis.Dia.Types;

namespace Axis.Dia.Contracts
{
    /// <summary>
    /// Defines the contract for instances that have annotations
    /// </summary>
    public interface IAnnotatable
    {
        /// <summary>
        /// The attribute list
        /// </summary>
        Annotation[] Annotations { get; }
    }
}
