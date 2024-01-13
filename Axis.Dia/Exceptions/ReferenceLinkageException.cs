using Axis.Dia.Contracts;
using Axis.Luna.Extensions;
using System.Collections.Immutable;

namespace Axis.Dia.Exceptions
{
    /// <summary>
    /// Raised if <see cref="Types.ReferenceValue"/> instances are found within a dia value graph that cannot be linked to other
    /// values within the graph.
    /// </summary>
    public class ReferenceLinkageException : Exception
    {
        public IDiaValue RootValue { get; }

        public ImmutableArray<IDiaReference> References { get; }


        public ReferenceLinkageException(
            string message,
            IDiaValue root,
            IEnumerable<IDiaReference> references)
        : base(message)
        {
            RootValue = root ?? throw new ArgumentNullException(nameof(root));
            References = references
                .ThrowIfNull(
                    () => new ArgumentNullException(nameof(references)))
                .ThrowIfAny(
                    item => item is null,
                    _ => new ArgumentException($"Null reference found in '{nameof(references)}'"))
                .ApplyTo(ImmutableArray.CreateRange);
        }


        public ReferenceLinkageException(
            IDiaValue root,
            IEnumerable<IDiaReference> references)
        : this("Unlinkable references were found in the given root value", root, references)
        {
        }
    }
}
