using Axis.Dia.Core.Contracts;
using Axis.Dia.Core.Types;

namespace Axis.Dia.PathQuery.Accessors
{
    public readonly struct IndexAccessor :
        IAccessor<Sequence, int>,
        IDefaultContract<IndexAccessor>
    {
        public Sequence Source { get; }

        public int Key { get; }

        public bool IsDefault => Source.IsDefault && Key == 0;

        public static IndexAccessor Default => default;

        public IndexAccessor(Sequence source, int key)
        {
            Source = source;
            Key = key;
        }
    }
}
